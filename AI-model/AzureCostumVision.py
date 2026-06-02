from azure.cognitiveservices.vision.customvision.prediction import CustomVisionPredictionClient
from msrest.authentication import ApiKeyCredentials
from fastapi import FastAPI, UploadFile, File
from typing import List
import numpy as np
import random
import cv2
import os


# Maak een FastAPI-applicatie aan
app = FastAPI()

# Haal de prediction key en endpoint op uit environment variables
# prediction_key = key
prediction_key = os.getenv("PREDICTION_KEY")

# prediction_endpoint = endpoint
prediction_endpoint = os.getenv("ENDPOINT_PREDICTION")

# Azure Custom Vision project ID
project_id = "c83cc105-d64c-4c3b-8ca9-c25f2da4ef62"

# Naam van de gepubliceerde iteratie (modelversie)
publish_iteration_name = "Iteration1"

# Authenticatie voor Azure Custom Vision API
credentials = ApiKeyCredentials(in_headers={
    "Prediction-key": prediction_key
})

# Client voor objectdetectie
predictor = CustomVisionPredictionClient(
    prediction_endpoint,
    credentials
)

def get_color():
    """Genereert een willekeurige kleur (BGR) voor bounding boxes"""
    return random.randint(0, 255), random.randint(0, 255), random.randint(0, 255)


def get_bounding_box(prediction, image_bytes):
    """
    Tekent bounding boxes op de afbeelding op basis van voorspellingen
    """
    np_arr = np.frombuffer(image_bytes, np.uint8)
    image = cv2.imdecode(np_arr, cv2.IMREAD_COLOR)

    for obj in prediction.predictions:
        # Filter voorspellingen met lage betrouwbaarheid
        if obj.probability < 0.6:
            continue

        box = obj.bounding_box

        # Zet relatieve coördinaten om naar absolute pixels
        x = int(box.left * image.shape[1])
        y = int(box.top * image.shape[0])
        w = int(box.width * image.shape[1])
        h = int(box.height * image.shape[0])

        color = get_color()

        # Teken rechthoek rond object
        cv2.rectangle(image, (x, y), (x + w, y + h), color, 5)

        # Maak labeltekst
        label = f"{obj.tag_name} ({obj.probability:.2f})"

        # Plaats tekst op afbeelding
        cv2.putText(image, label, (x, y - 10),
                    cv2.FONT_HERSHEY_SIMPLEX,
                    2, color, 4)

    # Encodeer afbeelding terug naar JPG bytes
    _, buffer = cv2.imencode(".jpg", image)
    image_bytes = buffer.tobytes()
    return image_bytes


def get_predict(prediction, file_name, content_type):
    """
    Zet Azure voorspellingen om naar een gestructureerd JSON-object
    """
    predict = {}
    predict["file name"] = file_name
    predict["content type"] = content_type

    list_predictions = []

    for obj in prediction.predictions:
        # Filter op minimale betrouwbaarheid
        if obj.probability >= 0.6:
            product = {}
            product["product name"] = obj.tag_name
            product["probability"] = obj.probability
            list_predictions.append(product)

    predict["predictions"] = list_predictions

    # predict["bounding_box_image"] = get_bounding_box(prediction)
    return predict


@app.post("/predict")
async def predict(images: List[UploadFile] = File(...)):
    """
    API endpoint voor objectdetectie op meerdere afbeeldingen
    """
    results = []

    for image in images:
        image_bytes = await image.read()

        # Stuur afbeelding naar Azure Custom Vision model
        prediction = predictor.detect_image(
            project_id,
            publish_iteration_name,
            image_bytes
        )

        # Verwerk voorspellingen
        predict_result = get_predict(
            prediction,
            image.filename,
            image.content_type
        )

        results.append(predict_result)

    return {"results": results}

