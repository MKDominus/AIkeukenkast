import time
import base64
from pathlib import Path
from concurrent.futures import ThreadPoolExecutor, as_completed

from openai import OpenAI
# =========================
# Azure OpenAI 配置
# =========================
endpoint = "https://my-ai-keukenkastje0-resource.services.ai.azure.com/openai/v1"
api_key = "APIKEY"
client = OpenAI(base_url=endpoint, api_key=api_key)

# =====================================
# 文件夹
# =====================================

INPUT_DIR = Path("sort_by_productname")
OUTPUT_DIR = Path("product_with_front_top_view")

OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

# =====================================
# 配置
# =====================================

MAX_WORKERS = 4
MAX_RETRIES = 5

SUPPORTED_EXT = {
    ".jpg",
    ".jpeg",
    ".png",
    ".webp"
}

# =====================================
# Prompt
# =====================================

REMOVE_BG_PROMPT = """
Remove the background completely.

STRICT REQUIREMENTS:
- Preserve product exactly
- Preserve packaging shape
- Preserve label artwork
- Preserve all text
- Preserve colors exactly
- Preserve proportions exactly
- No modifications

OUTPUT:
- Transparent background
- PNG with alpha channel
- No shadows
- No reflections
- No floor
- Product only
"""

FRONT_TOP_PROMPT = """
Create a photorealistic product image.

Use the uploaded product as the exact reference.

STRICT REQUIREMENTS:
- Preserve packaging shape
- Preserve label artwork
- Preserve all text
- Preserve colors exactly
- Preserve proportions exactly
- Preserve brand appearance exactly
- No redesign
- No modifications

Camera viewpoint:
Front-top perspective view.
Camera elevated approximately 45 degrees.
Looking slightly downward at the product.

IMPORTANT:
- Only change camera position
- Maintain strict geometric consistency

OUTPUT:
- Transparent background
- PNG with alpha channel
- No shadows
- No reflections
- No floor
- Product only
"""
# =====================================
# 调用图片编辑
# =====================================

def call_image_edit(img_path, prompt):

    for attempt in range(MAX_RETRIES):

        try:

            with open(img_path, "rb") as img_file:

                result = client.images.edit(
                    model="gpt-image-2",
                    image=img_file,
                    prompt=prompt
                )

            image_base64 = result.data[0].b64_json

            return base64.b64decode(image_base64)

        except Exception as e:

            print(
                f"Retry {attempt + 1}/{MAX_RETRIES} : "
                f"{img_path.name}"
            )

            if attempt == MAX_RETRIES - 1:
                raise

            time.sleep(5)

# =====================================
# 处理单张图片
# =====================================

def process_image(img_path):

    try:

        relative_dir = img_path.parent.relative_to(INPUT_DIR)

        target_dir = OUTPUT_DIR / relative_dir
        target_dir.mkdir(parents=True, exist_ok=True)

        # ------------------------------
        # 去背景版本
        # ------------------------------

        nobg_path = (
            target_dir /
            f"{img_path.stem}_original_nobg.png"
        )

        if not nobg_path.exists():

            nobg_bytes = call_image_edit(
                img_path,
                REMOVE_BG_PROMPT
            )

            with open(nobg_path, "wb") as f:
                f.write(nobg_bytes)

        # ------------------------------
        # Front-top版本
        # ------------------------------

        front_top_path = (
            target_dir /
            f"{img_path.stem}_Front-top.png"
        )

        if not front_top_path.exists():

            front_top_bytes = call_image_edit(
                img_path,
                FRONT_TOP_PROMPT
            )

            with open(front_top_path, "wb") as f:
                f.write(front_top_bytes)

        print(f"✓ Finished: {img_path}")

        return True

    except Exception as e:

        print(f"✗ Failed: {img_path}")
        print(e)

        return False

# =====================================
# 主函数
# =====================================

def generate_images():

    image_files = [
        p
        for p in INPUT_DIR.rglob("*")
        if p.is_file()
        and p.suffix.lower() in SUPPORTED_EXT
    ]

    print(f"Found {len(image_files)} images")

    success = 0
    failed = 0

    with ThreadPoolExecutor(
        max_workers=MAX_WORKERS
    ) as executor:

        futures = [
            executor.submit(process_image, img)
            for img in image_files
        ]

        for future in as_completed(futures):

            if future.result():
                success += 1
            else:
                failed += 1

    print("\n========================")
    print("Completed")
    print(f"Success : {success}")
    print(f"Failed  : {failed}")
    print("========================")

# =====================================
# Main
# =====================================

if __name__ == "__main__":

    start = time.time()

    generate_images()

    print(
        f"\nTotal Time: "
        f"{time.time() - start:.1f}s"
    )