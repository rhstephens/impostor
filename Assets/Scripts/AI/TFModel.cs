using TensorFlow;

class TFModel {

    static string INPUT_NODE = "conv2d_1_input";
    static string OUTPUT_NODE = "dense_2/Softmax";

    TFGraph _graph = new TFGraph();
    TFSession _session;

    // Create a new Tensorflow Model from a serialized byte array containing the model's graph_def
    public TFModel(byte[] graph_def) {
        // Import graph definition and cache a session
        _graph.Import(modelBytes);
        _session = new TFSession(graph);
    }

    // Predict a direction
    public Vector2 Predict() {
        var runner = _session.GetRunner();

        runner.AddInput(_graph[INPUT_NODE][0], new TensorFlow.TFTensor(TFDataType.Float, new long[]{1,44,60},1*44*60*4));
        runner.Fetch(_graph[OUTPUT_NODE][0]);

        var output = runner.Run();

        // Fetch the results from output:
        TFTensor result = output[0];
    }
}