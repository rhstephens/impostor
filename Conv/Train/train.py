import csv
import boto3
import numpy as np
import json

BUCKET_NAME = 'codetroopa-impostor'
TSET_PREFIX = 'training_sets/'

PLAYER_KEY = 'playerMatrices.csv'
OBSTACLE_KEY = 'obstacleMatrices.csv'
ENEMY_KEY = 'enemyMatrices.csv'
META_KEY = 'metadata.json'


def folder_names():
    response = client.list_objects_v2(
        Bucket=BUCKET_NAME,
        Prefix=TSET_PREFIX,
        Delimiter='/'
    )
    return [x['Prefix'] for x in response['CommonPrefixes']]

# This returns all three matrices needed for training
def get_matrices_from_s3():
    player_matrices = None
    obstacle_matrices = None
    enemy_matrices = None

    folder_keys = folder_names()

    # go through each folder, appending objects to the corresponding matrices
    for folder_prefix in folder_keys:
        print('Retrieving objects from folder: {}'.format(folder_prefix))
        # First, we grab meta-data specific to this folder
        response = client.get_object(Bucket=BUCKET_NAME, Key=folder_prefix + META_KEY)
        meta_data = json.loads(response['Body'].read().decode('utf-8'))
        m_count = meta_data['matrix_count']
        m_length = meta_data['matrix_length']
        m_width = meta_data['matrix_width']

        # init matrices TODO: somehow refactor this out
        if player_matrices is None:
            player_matrices = np.ndarray((0, m_length, m_width))
        if obstacle_matrices is None:
            obstacle_matrices = np.ndarray((0, m_length, m_width))
        if enemy_matrices is None:
            enemy_matrices = np.ndarray((0, m_length, m_width))

        # Now we go through each type of matrix, reshaping into an appropriate format
        response = client.get_object(Bucket=BUCKET_NAME, Key=folder_prefix + PLAYER_KEY)
        player_matrix = np.fromstring(response['Body'].read().decode('utf-8'), sep='\n', dtype=int)
        player_matrices = np.concatenate((player_matrices, player_matrix.reshape((m_count, m_length, m_width))))

        response = client.get_object(Bucket=BUCKET_NAME, Key=folder_prefix + OBSTACLE_KEY)
        obstacle_matrix = np.fromstring(response['Body'].read().decode('utf-8'), sep='\n', dtype=int)
        obstacle_matrices = np.concatenate((obstacle_matrices, obstacle_matrix.reshape((m_count, m_length, m_width))))

        response = client.get_object(Bucket=BUCKET_NAME, Key=folder_prefix + ENEMY_KEY)
        enemy_matrix = np.fromstring(response['Body'].read().decode('utf-8'), sep='\n', dtype=int)
        enemy_matrices = np.concatenate((enemy_matrices, enemy_matrix.reshape((m_count, m_length, m_width))))

    return (player_matrices, obstacle_matrices, enemy_matrices)

if __name__ == '__main__':
    client = boto3.client('s3')

    # First, we go through all objects in our training_sets folder
    (player_matrices, obstacle_matrices, enemy_matrices) = get_matrices_from_s3()
    print(player_matrices.shape)
    print(obstacle_matrices.shape)
    print(enemy_matrices.shape)
