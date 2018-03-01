import numpy as np
import os
import pickle
import boto3
from datetime import datetime

from sklearn.model_selection import train_test_split
from keras.models import Sequential
from keras.layers import Dense, Dropout, Activation, Flatten, Convolution2D, MaxPooling2D
from keras import backend as K
K.set_image_dim_ordering('th')


BUCKET_NAME = 'codetroopa-impostor'

if __name__ == "__main__":
    # Data that gets saved is in the format of: data = { 'xlabels': x_data, 'ylabels': y_labels }
    x_labels = None
    y_labels = None

    # Load the local training set
    with open('{}/training_set.pkl'.format(os.path.dirname(__file__)), 'rb') as f:
        data = pickle.load(f)

    x_labels = data['xlabels']
    y_labels = data['ylabels']
    shape = x_labels.shape

    X_train, X_test, y_train, y_test = train_test_split(x_labels, y_labels, test_size=0.15)
    assert(X_train.shape[0] == y_train.shape[0])
    assert(X_test.shape[0] == y_test.shape[0])

    # Begin training. For now, we are replicating a proven architecture used for image classification
    model = Sequential()

    model.add(Convolution2D(32, (3, 3), activation='relu', input_shape=(shape[1], shape[2], shape[3])))
    model.add(Convolution2D(32, (3, 3), activation='relu'))
    model.add(MaxPooling2D(pool_size=(2,2)))
    model.add(Dropout(0.25))

    model.add(Flatten())
    model.add(Dense(128, activation='relu'))
    model.add(Dropout(0.5))
    model.add(Dense(9, activation='softmax'))

    model.compile(loss='categorical_crossentropy', optimizer='adam', metrics=['accuracy'])
    model.fit(X_train, y_train, batch_size=32, nb_epoch=10, verbose=True)

    score = model.evaluate(X_test, y_test, verbose=1)
    print('Loss: {}'.format(score[0]))
    print('Accuracy: {}'.format(score[1]))

    # store model on S3 for later use
    client = boto3.client('s3')
    fname = '{}/model.h5'.format(os.path.dirname(__file__))
    model.save(fname)

    with open(fname, 'rb') as f:
        client.put_object(
            Bucket=BUCKET_NAME,
            Key='models/model_{}.h5'.format(datetime.now().strftime('%Y%m%d-%H%M')),
            Body=f
        )
    print('Successfully stored model on S3')
