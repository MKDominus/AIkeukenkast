from azure.cognitiveservices.vision.customvision.training import (
    CustomVisionTrainingClient
)
from msrest.authentication import ApiKeyCredentials

from dataset_loader import (
    load_items,
    load_backgrounds
)

from create_tags import create_tags_and_collect_images
from synthetic_generator import run_synthetic_generation


ENDPOINT = "https://tzorgcostum.cognitiveservices.azure.com/"
TRAINING_KEY = "key"
PROJECT_ID = "dce83bdd-1ba0-4aa2-9ff7-adb31c5119e2"


def main():

    credentials = ApiKeyCredentials(
        in_headers={"Training-key": TRAINING_KEY}
    )

    trainer = CustomVisionTrainingClient(
        ENDPOINT,
        credentials
    )

    # # 创建 Azure tags
    # _, tag_map = create_tags_and_collect_images(
    #     trainer,
    #     PROJECT_ID,
    #     "../clean_sort_by_productname"
    # )

    # 读取素材
    items = load_items("../segment_images")

    backgrounds = load_backgrounds(
        "background_resize"
    )

    # 开始生成+上传
    run_synthetic_generation(
        trainer,
        PROJECT_ID,
        items,
        backgrounds,
        5
    )


if __name__ == "__main__":
    main()