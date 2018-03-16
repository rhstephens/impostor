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

        TFTensor inputTensor = new TensorFlow.TFTensor(input);

        var runner = _session.GetRunner();
        runner.AddInput(_graph[INPUT_NODE][0], inputTensor);
        runner.Fetch(_graph[OUTPUT_NODE][0]);

        // Fetch the results from output:
        var output = runner.Run();
        TFTensor result = output[0];
        Debug.Log(result);
        return Vector2.zero;
    }

    // Helper that generates the input matrix from a given Player object
    private Array GenerateInputMatrix(GameObject go) {
        // The order that these matrices are concatenated MATTERS! P -> O -> E
        int[,] pMatrix = GameManager.Instance.GeneratePlayerMatrix(go);
		int[,] oMatrix = GameManager.Instance.GenerateObstacleMatrix();
		int[,] eMatrix = GameManager.Instance.GenerateEnemyMatrix();

        Array input = Array.CreateInstance(typeof(double), GameManager.NUM_CHANNELS, GameManager.GRID_WIDTH, GameManager.GRID_LENGTH);

        // Copy three matrices to create 3-channeled input array
        for (int row = 0; row < input.GetLength(1); row++) {
            for (int col = 0; col < input.GetLength(2); col++) {
                input.SetValue(Convert.ToDouble(pMatrix[row, col]), 0, row, col);
            }
        }

        for (int row = 0; row < input.GetLength(1); row++) {
            for (int col = 0; col < input.GetLength(2); col++) {
                input.SetValue(Convert.ToDouble(oMatrix[row, col]), 1, row, col);
            }
        }

        for (int row = 0; row < input.GetLength(1); row++) {
            for (int col = 0; col < input.GetLength(2); col++) {
                input.SetValue(Convert.ToDouble(eMatrix[row, col]), 2, row, col);
            }
        }

        return input;
    }
}