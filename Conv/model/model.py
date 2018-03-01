import boto3
import os

from keras.models import load_model


BUCKET_NAME = 'codetroopa-impostor'
MODEL_PREFIX = 'models/'

def file_name():
    return '{}/model.h5'.format(os.path.dirname(__file__))


class Model:
    def __init__(self):
        self.model = self.instantiate_model()
        self.client = boto3.resource('s3')

    def instantiate_model(self):
        # Models are saved on S3 using a timestamp. S3 stores files sorted lexographically by its name.
        #   As such, the last key returned by list_objects_v2 will be the latest model.
        latest_model_filename = self.file_names_by_prefix(MODEL_PREFIX)[-1]

        keras_model = load_model(file_name)

        return load_model(file_name)

    def file_names_by_prefix(self, prefix):
        response = self.client.list_objects_v2(
            Bucket=BUCKET_NAME,
            Prefix=prefix,
            Delimiter='/'
        )
        return [x['Prefix'] for x in response['CommonPrefixes']]

    def predict(self, input_matrix):
        pass
