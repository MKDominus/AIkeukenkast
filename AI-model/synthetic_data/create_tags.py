from pathlib import Path
import time


def create_tags_and_collect_images(trainer, project_id, root_dir):
    """
    Create Azure Custom Vision tags and collect image paths.

    Behavior:
    - If tag creation fails -> skip silently
    - If tag already exists -> print info message
    - Image errors -> skip silently

    Returns:
        items (list)
        tag_map (dict)
    """

    items = []
    tag_map = {}

    root = Path(root_dir)

    for class_dir in root.iterdir():

        if not class_dir.is_dir():
            continue

        label = class_dir.name

        # -------------------------
        # Create tag safely
        # -------------------------
        if label not in tag_map:
            try:
                tag_map[label] = trainer.create_tag(
                    project_id,
                    label
                )
                print(f"[Tag] Created: {label}")

            except Exception:
                # Tag already exists or API error -> try to fetch silently
                try:
                    existing_tags = trainer.get_tags(project_id)
                    tag_map[label] = next(
                        t for t in existing_tags if t.name == label
                    )
                    print(f"[Tag] Already exists: {label}")

                except Exception:
                    # Fully silent fallback
                    continue

        # -------------------------
        # Collect images
        # -------------------------
        for img_path in class_dir.glob("*.png"):

            try:
                items.append({
                    "tag": label,
                    "path": img_path
                })
            except Exception:
                pass

        time.sleep(0.5)

    return items, tag_map