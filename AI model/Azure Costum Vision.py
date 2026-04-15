from azure.cognitiveservices.vision.customvision.prediction import CustomVisionPredictionClient
from msrest.authentication import ApiKeyCredentials

prediction_key = "key"
prediction_endpoint = "endpoint"
project_id = "73d538ad-13a2-49dc-a269-f6ae01da0f17"
publish_iteration_name  = "Iteration1"


credentials = ApiKeyCredentials(in_headers={
    "Prediction-key": prediction_key
})

predictor = CustomVisionPredictionClient(prediction_endpoint, credentials)



with open("test.jpeg", "rb") as image_contents:
    results = predictor.detect_image(
        project_id,
        publish_iteration_name,
        image_contents.read()
    )

for prediction in results.predictions:
    print(prediction.tag_name, prediction.probability)