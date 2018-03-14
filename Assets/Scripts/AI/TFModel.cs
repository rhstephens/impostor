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
        long[,,] input = GenerateInputMatrix(go);
        int size = GameManager.NUM_CHANNELS * GameManager.GRID_LENGTH * GameManager.GRID_WIDTH;

        var runner = _session.GetRunner();
        runner.AddInput(_graph[INPUT_NODE][0], new TensorFlow.TFTensor(TFDataType.Float, input, size));
        runner.Fetch(_graph[OUTPUT_NODE][0]);

        var output = runner.Run();

        // Fetch the results from output:
        TFTensor result = output[0];
        Debug.Log("123");
        return Vector2.zero;
    }

    // Helper that generates the input matrix from a given Player object
    private long[,,] GenerateInputMatrix(GameObject go) {
        // The order that these matrices are concatenated MATTERS! P -> O -> E
        int[,] pMatrix = GameManager.Instance.GeneratePlayerMatrix(go);
		int[,] oMatrix = GameManager.Instance.GenerateObstacleMatrix();
		int[,] eMatrix = GameManager.Instance.GenerateEnemyMatrix();

        long[,,] input = new long[GameManager.NUM_CHANNELS, GameManager.GRID_WIDTH, GameManager.GRID_LENGTH];

        // Copy three matrices to create 3-channeled input nd-array
        for (int row = 0; row < input.GetLength(1); row++) {
            for (int col = 0; col < input.GetLength(2); col++) {
                input[0, row, col] = (long)pMatrix[row, col];
            }
        }

        for (int row = 0; row < input.GetLength(1); row++) {
            for (int col = 0; col < input.GetLength(2); col++) {
                input[1, row, col] = (long)oMatrix[row, col];
            }
        }

        for (int row = 0; row < input.GetLength(1); row++) {
            for (int col = 0; col < input.GetLength(2); col++) {
                input[2, row, col] = (long)eMatrix[row, col];
            }
        }

        return input;
    }
}