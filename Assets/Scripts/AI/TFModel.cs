using System;
using TensorFlow;
using UnityEngine;

class TFModel {

    static string INPUT_NODE = "conv2d_1_input";
    static string OUTPUT_NODE = "dense_2/Softmax";

    TFGraph _graph = new TFGraph();
    TFSession _session;


    // Create a new Tensorflow Model from a serialized byte array containing the model's graph_def
    public TFModel(byte[] graph_def) {
        // Import graph definition and cache a session
        _graph.Import(graph_def);
        _session = new TFSession(_graph);
    }

    // Predict a direction given input matrices
    public Vector2 Predict(GameObject go) {
        Array input = GenerateInputMatrix(go);
        long[] dims = new long[]{ GameManager.NUM_CHANNELS, GameManager.GRID_LENGTH, GameManager.GRID_WIDTH };
        int size = GameManager.NUM_CHANNELS * GameManager.GRID_LENGTH * GameManager.GRID_WIDTH;

        var runner = _session.GetRunner();
        runner.AddInput(_graph[INPUT_NODE][0], input);
        runner.Fetch(_graph[OUTPUT_NODE][0]);

        // Fetch the results from output and return appropriate vector
        var output = runner.Run();
        float[,] result = (float[,]) output[0].GetValue();
        return argmax(result);
    }


    // Returns the maximum argument and converts it into its appropriate direction
    private Vector2 argmax(float [,] result) {
        float maxValue = 0;
        int maxIndex = 0;
        for (int i = 0; i < 9; i++) {
            if (result[0, i] > maxValue) {
                maxValue = result[0, i];
                maxIndex = i;
            }
        }

        if (maxIndex == 0) {
            return new Vector2(0, 0);
        } else if (maxIndex == 1) {
            return new Vector2(1, 1);
        } else if (maxIndex == 2) {
            return new Vector2(1, 0);
        } else if (maxIndex == 3) {
            return new Vector2(1, -1);
        } else if (maxIndex == 4) {
            return new Vector2(0, -1);
        } else if (maxIndex == 5) {
            return new Vector2(-1, -1);
        } else if (maxIndex == 6) {
            return new Vector2(-1, 0);
        } else if (maxIndex == 7) {
            return new Vector2(-1, 1);
        } else {
            return new Vector2(0, 1);
        }

    }

    // Helper that generates the input matrix from a given Player object
    private Array GenerateInputMatrix(GameObject go) {
        // The order that these matrices are concatenated MATTERS! P -> O -> E
        int[,] pMatrix = GameManager.Instance.GeneratePlayerMatrix(go);
		int[,] oMatrix = GameManager.Instance.GenerateObstacleMatrix();
		int[,] eMatrix = GameManager.Instance.GenerateEnemyMatrix(go);

        Array input = Array.CreateInstance(typeof(float), 1, GameManager.NUM_CHANNELS, GameManager.GRID_WIDTH, GameManager.GRID_LENGTH);

        // Copy three matrices to create 3-channeled input array
        for (int row = 0; row < input.GetLength(1); row++) {
            for (int col = 0; col < input.GetLength(2); col++) {
                input.SetValue(Convert.ToSingle(pMatrix[row, col]), 0, 0, row, col);
            }
        }

        for (int row = 0; row < input.GetLength(1); row++) {
            for (int col = 0; col < input.GetLength(2); col++) {
                input.SetValue(Convert.ToSingle(oMatrix[row, col]), 0, 1, row, col);
            }
        }

        for (int row = 0; row < input.GetLength(1); row++) {
            for (int col = 0; col < input.GetLength(2); col++) {
                input.SetValue(Convert.ToSingle(eMatrix[row, col]), 0, 2, row, col);
            }
        }

        Debug.Log(input.Length);
        return input;
    }
}