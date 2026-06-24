import os
from PIL import Image
from concurrent.futures import ThreadPoolExecutor, as_completed

def resize_image(input_path, output_path, target_height):
    # Open the input image
    img = Image.open(input_path)

    # Get the original width and height
    w, h = img.size

    # Calculate the scaling factor based on the target height
    scale = target_height / h
    new_width = int(w * scale)

    # Resize the image while preserving the aspect ratio
    img = img.resize((new_width, target_height), Image.LANCZOS)

    # Create the output directory if it does not already exist
    os.makedirs(os.path.dirname(output_path), exist_ok=True)

    # Save the resized image to the output path
    img.save(output_path)


def collect_images(input_root):
    # Store all discovered image file paths
    image_paths = []

    # Recursively walk through all directories and files
    for root, dirs, files in os.walk(input_root):
        for file in files:
            # Check if the file has a supported image extension
            if file.lower().endswith(
                (".png", ".jpg", ".jpeg", ".webp")
            ):
                # Add the full file path to the list
                image_paths.append(
                    os.path.join(root, file)
                )

    return image_paths


def process_folder(
    input_root,
    output_root,
    target_height,
    max_workers=6
):
    # Collect all image files from the input directory
    image_paths = collect_images(input_root)

    futures = []

    # Create a thread pool for parallel image processing
    with ThreadPoolExecutor(max_workers=max_workers) as executor:

        for input_path in image_paths:

            # Preserve the original directory structure
            relative_path = os.path.relpath(
                input_path,
                input_root
            )

            # Build the corresponding output file path
            output_path = os.path.join(
                output_root,
                relative_path
            )

            # Submit the resize task to the thread pool
            futures.append(
                executor.submit(
                    resize_image,
                    input_path,
                    output_path,
                    target_height
                )
            )

        # Monitor task completion and display progress
        for i, _ in enumerate(as_completed(futures), 1):
            print(f"Progress: {i}/{len(futures)}")

if __name__ == "__main__":
    process_folder(
        "custom vission/background",
        "custom vission/background_resize",
        target_height=1448
    )