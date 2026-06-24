from image_clean_and_segmentation import process_folder as clean_and_segmentation
from resize_image import process_folder as resize_image
from image_generation import process_image as ai_image_generation

if __name__ == "__main__":
    print("start ai image generation")
    input_root = "../sort_by_product"
    resize_root = "../product_with_front_top_view"
    ai_image_generation()

    print("start resizing")
    input_root = "../product_with_front_top_view"
    resize_root = "../resize_images"
    target_height = 800
    resize_image(input_root,resize_root,target_height)

    print("start cleaning background")
    output_root = "../segment_images"
    clean_and_segmentation(resize_root,output_root)

