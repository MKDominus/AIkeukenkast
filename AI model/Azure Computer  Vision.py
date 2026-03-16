import cv2
from azure.ai.vision.imageanalysis import ImageAnalysisClient
from azure.ai.vision.imageanalysis.models import VisualFeatures
from azure.core.credentials import AzureKeyCredential

client = ImageAnalysisClient(
    endpoint="YOUR_ENDPOINT",
    credential=AzureKeyCredential("YOUR_KEY")
)

image_path = "test.jpg"

with open(image_path, "rb") as f:
    image_data = f.read()

result = client.analyze(
    image_data=image_data,
    visual_features=[
        VisualFeatures.READ,
        VisualFeatures.TAGS,
        VisualFeatures.OBJECTS
    ],
    language="en"
)

image = cv2.imread(image_path)

for obj in result.objects.list:
    box = obj.bounding_box

    x = int(box.x)
    y = int(box.y)
    w = int(box.width)
    h = int(box.height)

    cv2.rectangle(image, (x, y), (x + w, y + h), (0, 255, 0), 2)

    label = f"{obj.tags[0].name} ({obj.tags[0].confidence:.2f})"

    cv2.putText(
        image,
        label,
        (x, y - 10),
        cv2.FONT_HERSHEY_SIMPLEX,
        0.5,
        (0, 255, 0),
        2
    )

cv2.imwrite("output_with_boxes.jpg", image)