from pathlib import Path

def load_items(root_item):
    items = []

    for class_dir in Path(root_item).iterdir():

        if class_dir.is_dir():

            items.append({
                "tag": class_dir.name,
                "path": class_dir,
                "use_cnt": 0
            })

    return items


def load_backgrounds(root_background):
    backgrounds = []

    for img_path in Path(root_background).glob("*"):

        if img_path.suffix.lower() in [".png", ".jpg", ".jpeg"]:

            backgrounds.append({
                "path": img_path
            })

    return backgrounds