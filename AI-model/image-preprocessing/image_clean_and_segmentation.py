import os
from rembg import remove
from PIL import Image
from concurrent.futures import ThreadPoolExecutor, as_completed

# =========================
# Single image segmentation
# =========================

def segment_image(input_path, output_path):
    """
    Remove background, convert to RGBA, crop to object bounding box,
    and save the processed image.
    """

    # Load image
    img = Image.open(input_path)

    # Background removal (rembg model)
    out = remove(img)

    # Ensure RGBA format (includes alpha channel)
    out = out.convert("RGBA")

    # Extract alpha channel
    alpha = out.split()[3]

    # Compute bounding box of non-transparent region
    bbox = alpha.getbbox()

    # Crop image to object region if valid
    if bbox:
        out = out.crop(bbox)

    # Ensure output directory exists
    os.makedirs(os.path.dirname(output_path), exist_ok=True)

    # Save processed image
    out.save(output_path)


# =========================
# Collect all images recursively
# =========================

def collect_images(input_root):
    """
    Recursively collect all supported image files from a directory tree.
    """

    image_paths = []

    for root, dirs, files in os.walk(input_root):
        for file in files:
            if file.lower().endswith((".png", ".jpg", ".jpeg", ".webp")):
                image_paths.append(os.path.join(root, file))

    return image_paths


# =========================
# Batch processing with multithreading
# =========================

def process_folder(input_root, output_root, max_workers=6):
    """
    Process an entire dataset folder using parallel background removal.
    """

    image_paths = collect_images(input_root)

    futures = []

    # Thread pool for parallel processing
    with ThreadPoolExecutor(max_workers=max_workers) as executor:

        for input_path in image_paths:

            # Preserve folder structure in output directory
            relative_path = os.path.relpath(input_path, input_root)

            output_path = os.path.join(
                output_root,
                os.path.splitext(relative_path)[0] + ".png"
            )

            # Submit task
            futures.append(
                executor.submit(
                    segment_image,
                    input_path,
                    output_path
                )
            )

        # Progress tracking
        for i, _ in enumerate(as_completed(futures), 1):
            print(f"Progress: {i}/{len(futures)}")


if __name__ == "__main__":
    process_folder(
        "resize_images",
        "segment_images"
    )