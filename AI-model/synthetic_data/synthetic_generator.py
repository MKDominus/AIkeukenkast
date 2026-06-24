"""
Synthetic data generation and automated Azure Custom Vision upload module.
Main function: Takes foreground object images with a transparent (RGBA) channel,
applies random geometric and color augmentations, and randomly pastes them onto
background images to create synthetic data. It generates bounding box labels,
executes the generation concurrently via multi-threading, and asynchronously
batches the uploads to Azure using a background queue.
"""

import io
import os
import time
import random
import threading
import queue
from concurrent.futures import ThreadPoolExecutor, as_completed

import numpy as np
from PIL import Image
import albumentations as A

from azure.cognitiveservices.vision.customvision.training.models import (
    ImageFileCreateBatch,
    ImageFileCreateEntry,
    Region
)


# =========================================================
# Global States (Injected by main.py, not initialized on module load)
# =========================================================

trainer = None  # Azure Custom Vision Training API client instance
PROJECT_ID = None  # Azure Custom Vision Project ID

items = []  # Stores foreground object info (paths, tags, usage counts, etc.)
backgrounds = []  # Stores background image info
tag_map = {}  # Mapping from string tag names to Azure Custom Vision Tag IDs

TARGET_IMAGES = 3000  # Total number of synthetic images to generate
UPLOAD_BATCH_SIZE = 20  # Number of images to bundle per Azure API upload (API limit)

# Thread lock to ensure thread safety when reading/modifying 'use_cnt' across threads
items_lock = threading.Lock()
# Upload queue to decouple "image generation" from "network upload", max buffer 100
upload_queue = queue.Queue(maxsize=100)


# =========================================================
# Augmentations - Based on Albumentations
# =========================================================

def pad_rgba(img, pad=100):
    """
    Pads the RGBA image with transparent pixels.
    Purpose: Prevents the image edges from being cropped out during
    subsequent rotation, translation, or perspective transformations.
    """
    img = img.convert("RGBA")
    w, h = img.size
    # Create a larger, purely transparent background image
    new_img = Image.new("RGBA", (w + 2 * pad, h + 2 * pad), (0, 0, 0, 0))
    # Paste the original image right in the center
    new_img.paste(img, (pad, pad))
    return new_img


# Geometric transformation pipeline (affects shape and position)
geo_transform = A.Compose([
    # Ensure image is at least 1024x1024; pad with transparent pixels (0,0,0,0) if smaller
    A.PadIfNeeded(min_height=1024, min_width=1024, border_mode=0, fill=(0, 0, 0, 0)),
    # Affine transform: scale 0.7-1.1x, translate vertically/horizontally up to 8%,
    # rotate ±45 deg, shear ±5 deg. Trigger probability: 90%
    A.Affine(scale=(0.7, 1.1), translate_percent=(0, 0.08),
             rotate=(-45, 45), shear=(-5, 5), p=0.9),
    # Random perspective transform to simulate different viewing angles. Trigger: 30%
    A.Perspective(scale=(0.02, 0.1), p=0.3)
])

# Color transformation pipeline (affects RGB channels only, preserves transparency)
color_transform = A.Compose([
    # Random brightness and contrast adjustments
    A.RandomBrightnessContrast(0.15, 0.15, p=0.5),
    # Random Hue, Saturation, and Value adjustments
    A.HueSaturationValue(10, 20, 10, p=0.3),
    # Random Gamma correction (adjusts overall light/dark distribution)
    A.RandomGamma((90, 110), p=0.2),
    # 5% chance to convert to grayscale
    A.ToGray(p=0.05),
    # Randomly pick one of three blur effects (Gaussian, Motion, Defocus), overall probability: 20%
    A.OneOf([
        A.GaussianBlur((3, 5), p=1),
        A.MotionBlur(3, p=1),
        A.Defocus(radius=(2, 4), alias_blur=(0.1, 0.3), p=1)
    ], p=0.2),
    # Generate virtual shadows
    A.RandomShadow(p=0.3),
    # Gaussian noise to simulate camera sensor noise
    A.GaussNoise(p=0.15),
    # JPEG compression artifacts to simulate low-quality images
    A.ImageCompression(quality_range=(75, 95), p=0.2)
])


def augment_rgba(img_pil):
    """
    Applies the full augmentation pipeline to the foreground RGBA image.
    Key design: Separates the Alpha channel to prevent color transformations
    from corrupting the transparent edges.
    """
    # 1. Pad edges to prevent cropping during geometric transforms
    img_pil = pad_rgba(img_pil, 200)
    rgba = np.array(img_pil)

    # 2. Apply geometric transformations on the whole image (including Alpha)
    rgba = geo_transform(image=rgba)["image"]

    # 3. Separate RGB and Alpha channels
    rgb = rgba[:, :, :3]
    alpha = rgba[:, :, 3]

    # 4. Apply color transformations ONLY to RGB channels so transparency isn't affected by noise/compression
    rgb = color_transform(image=rgb)["image"]

    # 5. Recombine RGB and Alpha, and return
    return np.dstack([rgb, alpha])


# =========================================================
# Utils
# =========================================================

def iou(a, b):
    """
    Calculates the Intersection over Union (IoU) of two bounding boxes.
    Used for collision detection to prevent generated objects from severely overlapping.
    Parameter format: (x, y, width, height)
    """
    ax, ay, aw, ah = a
    bx, by, bw, bh = b

    # Calculate coordinates of the intersection rectangle
    x1 = max(ax, bx)
    y1 = max(ay, by)
    x2 = min(ax + aw, bx + bw)
    y2 = min(ay + ah, by + bh)

    # If they don't intersect, IoU is 0
    if x2 <= x1 or y2 <= y1:
        return 0.0

    # Calculate intersection area and union area
    inter = (x2 - x1) * (y2 - y1)
    union = aw * ah + bw * bh - inter
    return inter / union


def get_random_image(folder_path):
    """
    Picks a random image file from the specified folder.
    Supports common image extensions.
    """
    exts = ('.jpg', '.jpeg', '.png', '.bmp', '.webp')
    files = [
        f for f in os.listdir(str(folder_path))
        if f.lower().endswith(exts)
    ]
    if not files:
        return None
    return os.path.join(str(folder_path), random.choice(files))


def sample_object(available, temperature=1.5, rare_boost=0.3):
    """
    Samples an object from the available list using a smooth weight and rarity compensation algorithm.
    Purpose: Prevents over-usage of certain items, ensuring a relatively balanced dataset class distribution.
    """
    # Get the usage count for each item
    use_counts = np.array([x["use_cnt"] for x in available])

    # Base weight: Higher usage count = lower weight
    weights = 1.0 / (use_counts + 1)

    # Rarity boost: Calculate the gap between the current item and the most frequently used item
    max_cnt = np.max(use_counts)
    rarity = (max_cnt - use_counts) / (max_cnt + 1)

    # Integrate the rarity boost into the weights
    weights = weights + rare_boost * rarity

    # Apply temperature coefficient: > 1 smooths the probability distribution, increasing randomness
    weights = weights ** (1 / temperature)

    # Normalize array so probabilities sum to 1
    weights = weights / weights.sum()

    # Randomly select an item index based on the calculated probability distribution
    idx = np.random.choice(len(available), p=weights)
    return available[idx]


def sample_object_count():
    """
    Determines how many objects to paste onto a single background image.
    Uses a binomial distribution to generate a number between 1 and 7 (mean around 4).
    """
    return int(np.random.binomial(n=6, p=0.5) + 1)


# =========================================================
# Uploader worker
# =========================================================

def uploader_worker():
    """
    Background daemon thread: Pulls generated images from the upload queue
    and batches them to Azure Custom Vision.
    """
    batch_images = []
    total_uploaded = 0

    while True:
        # Block and wait for newly generated image data from the queue
        entry = upload_queue.get()

        # Receiving 'None' signals that all generation tasks are finished
        if entry is None:
            if batch_images:  # Upload any remaining images that didn't fill a complete batch
                try:
                    batch = ImageFileCreateBatch(images=batch_images)
                    trainer.create_images_from_files(PROJECT_ID, batch)
                except Exception as e:
                    print("[Upload] final error:", e)
            # Mark the task as done and exit the loop
            upload_queue.task_done()
            break

        # Collect entry into the batch list
        batch_images.append(entry)

        # Trigger API upload once the batch size (UPLOAD_BATCH_SIZE) is reached
        if len(batch_images) >= UPLOAD_BATCH_SIZE:
            try:
                batch = ImageFileCreateBatch(images=batch_images)
                trainer.create_images_from_files(PROJECT_ID, batch)
                total_uploaded += len(batch_images)
                print(f"[Upload] {total_uploaded}/{TARGET_IMAGES}")
            except Exception as e:
                print("[Upload] error:", e)

            # Clear the batch buffer for the next round
            batch_images = []
            # Brief sleep to prevent triggering Azure API Rate Limits
            time.sleep(1)

        # Mark the current popped task as processed
        upload_queue.task_done()


# =========================================================
# Image generation
# =========================================================

def generate_single_image(image_idx):
    """
    Core function to generate a single synthetic image. Executed concurrently by the thread pool.
    """
    # 1. Randomly select and load a background image
    bg = Image.open(random.choice(backgrounds)["path"]).convert("RGBA")
    bw, bh = bg.size

    regions = []  # Stores the generated bounding box labels
    # Limit the number of objects per image to between 1 and 10
    cnt = max(1, min(sample_object_count(), 10))

    # Lock and copy the available items list (prevents resource contention with other threads)
    with items_lock:
        available = items.copy()

    # Loop to place 'cnt' objects onto the background
    for _ in range(cnt):
        if not available:
            break

        # Safely sample an item and remove it from the current available list
        # (prevents the exact same object repeating too much on a single image)
        with items_lock:
            obj = sample_object(available)
            available.remove(obj)

        # Grab a random specific image file from the object's folder
        image_path = get_random_image(obj["path"])
        if not image_path:
            continue

        # Load the foreground object and run the custom augmentation pipeline
        item = Image.open(image_path).convert("RGBA")
        item = augment_rgba(item)

        # 2. Crop the transparent empty space around the foreground (Tight Bounding Box)
        arr = np.array(item)
        alpha = arr[:, :, 3]
        ys, xs = np.where(alpha > 0)  # Find coordinates of all non-transparent pixels

        if len(xs) == 0:  # Skip if the image became completely transparent (e.g., extreme augmentation)
            continue

        # Crop out redundant transparent borders using the min/max coordinates
        arr = arr[min(ys):max(ys), min(xs):max(xs)]
        item = Image.fromarray(arr, "RGBA")

        # 3. Random scale adjustment
        w, h = item.size
        scale = random.uniform(0.6, 1.2)
        nw, nh = int(w * scale), int(h * scale)
        item = item.resize((nw, nh))

        placed = False

        # 4. Collision detection & position search (max 100 attempts to find a non-overlapping spot)
        for _ in range(100):
            # Abort this attempt if the object is larger than the background
            if nw >= bw or nh >= bh:
                continue

            # Randomly generate top-left coordinates
            x = random.randint(0, bw - nw)
            y = random.randint(0, bh - nh)

            # Check IoU against already placed objects
            ok = True
            for r in regions:
                # Convert Azure's normalized coordinates back to pixel coordinates for IoU calculation
                old = (
                    int(r.left * bw),
                    int(r.top * bh),
                    int(r.width * bw),
                    int(r.height * bh)
                )
                # If IoU > 0.4, it's heavily overlapping, so we need to find a new position
                if iou((x, y, nw, nh), old) > 0.4:
                    ok = False
                    break

            # Found a valid position
            if ok:
                placed = True
                # Globally increment the usage count for this item (used for balanced weight sampling later)
                with items_lock:
                    for it in items:
                        if it["path"] == obj["path"]:
                            it["use_cnt"] += 1
                            break
                break  # Exit the search loop

        # Skip this object if we failed 100 times to find a valid position
        if not placed:
            continue

        # 5. Paste the item onto the background (using its own Alpha channel as the mask)
        bg.paste(item, (x, y), item)

        # 6. Generate the normalized relative coordinates required by Azure Custom Vision
        regions.append(
            Region(
                tag_id=tag_map[obj["tag"]].id,
                left=x / bw,
                top=y / bh,
                width=nw / bw,
                height=nh / bh
            )
        )

    # 7. Apply color transformations to the final composite image (blends foreground and background tones for realism)
    buffer = io.BytesIO()
    scene = np.array(bg)

    rgb = scene[:, :, :3]
    alpha = scene[:, :, 3]

    rgb = color_transform(image=rgb)["image"]
    scene = np.dstack([rgb, alpha])

    # Save the composite image as a PNG into the memory buffer
    Image.fromarray(scene, "RGBA").save(buffer, format="PNG")
    buffer.seek(0)

    # 8. Construct the upload entry format required by Azure
    entry = ImageFileCreateEntry(
        name=f"img_{image_idx}.png",
        contents=buffer.read(),
        regions=regions
    )

    return image_idx, entry


# =========================================================
# MAIN API (Exposed entry point, called by main.py)
# =========================================================

def run_synthetic_generation(
        trainer_,
        project_id_,
        items_,
        backgrounds_,
        target_images
):
    """
    Starts the synthetic data generation and automated upload task.
    Uses a Producer-Consumer model:
    - Producer: Multiple worker threads generating images concurrently.
    - Consumer: A single background thread collecting and batch uploading images.
    """
    # Inject globally passed parameters
    global trainer, PROJECT_ID, items, tag_map, backgrounds, TARGET_IMAGES

    trainer = trainer_
    PROJECT_ID = project_id_
    items = items_
    tags = trainer.get_tags(PROJECT_ID)
    tag_map = {t.name: t for t in tags}
    backgrounds = backgrounds_
    TARGET_IMAGES = target_images

    print(f"Start generating {TARGET_IMAGES} images...")

    start = time.time()

    # Start the background upload daemon thread
    t = threading.Thread(target=uploader_worker, daemon=True)
    t.start()

    # Dynamically determine concurrency limit (max 32, typically CPU cores + 4)
    workers = min(32, (os.cpu_count() or 1) + 4)

    # Use ThreadPoolExecutor to generate images concurrently
    with ThreadPoolExecutor(max_workers=workers) as ex:
        # Submit all generation tasks and build a Future dictionary mapping
        futures = {
            ex.submit(generate_single_image, i): i
            for i in range(1, TARGET_IMAGES + 1)
        }

        # As soon as an image is generated, push its result to the upload queue
        for f in as_completed(futures):
            try:
                idx, entry = f.result()
                upload_queue.put(entry)  # Push to the uploader thread

                # Dynamic decay mechanism: Every 300 images, decay all items' "use_cnt" by 5%
                # Purpose: Gives previously high-frequency objects a chance to recover weight over time
                if idx % 300 == 0:
                    with items_lock:
                        for it in items:
                            it["use_cnt"] *= 0.95

            except Exception as e:
                print("[Gen error]", e)

    # All images generated and queued; send the 'None' kill signal to the uploader thread
    upload_queue.put(None)
    # Block the main process, waiting for the uploader thread to finish processing the remaining queue
    t.join()

    # Time tracking
    cost = time.time() - start

    print("=====================================")
    print(f"Done in {cost:.2f}s")
    print(f"Avg {cost / TARGET_IMAGES:.2f}s/image")

