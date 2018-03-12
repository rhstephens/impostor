using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;


/// <summary>
/// A singleton class to govern Network related and server-side tasks. Unfortunately, unity doesn't support generic typed classes
///     for NetworkBehaviour so I could not abstract out the singleton pattern.
/// </summary>
public class NetworkManager : NetworkBehaviour {

    protected static NetworkManager instance;

    // Returns the instance of this singleton
    public static NetworkManager Instance {
        get {
            if (instance == null) {
                instance = (NetworkManager)FindObjectOfType(typeof(NetworkManager));

                if (instance == null) {
                    Debug.LogError("An instance of " + typeof(NetworkManager) +
                       " is needed in the scene, but there is none.");
                }
            }

            return instance;
        }
    }

	AWSClient _client;
	TFModel model = null;

	public void Start() {
		_client = GameManager.Instance.Client;
		GetLatestModel();
	}

	public Vector2 PredictDirection(Feature x) {
		if (model == null) {
			Debug.Log("AIModel not yet instantiated...");
			return Vector2.zero;
		}

		Vector2 newDirection = model.PredictDirection(x);
		Debug.Log("Direction is X: " + newDirection.x.ToString() + " Y: " + newDirection.y.ToString());
		return newDirection;
	}

	//////////////////////////////////////////////
	// Convolutional Neural Network Model
	//////////////////////////////////////////////

	// The Tensorflow graph has
	// input node: conv2d_1_input
	// output node: dense_2/Softmax

	byte[] ReadStream(Stream input) {
		using (MemoryStream ms = new MemoryStream()) {
			input.CopyTo(ms);
			return ms.ToArray();
		}
	}

	// Downloads latest model from s3 and references it locally on the server.
	void GetLatestModel() {
		Debug.Log("Getting latest model...");
		_client.ListObjects(AWSClient.BUCKET_NAME, "models/", ListModelsHandler);
	}

	// callback that handles retrieving a list of models on AWS.
	void ListModelsHandler(AmazonServiceResult<ListObjectsRequest, ListObjectsResponse> cb) {
		if (cb.Exception != null) {
			Debug.Log(cb.Exception);
		}

		string latestKey = "";
		ListObjectsResponse resp = cb.Response;
		foreach (S3Object obj in resp.S3Objects) {
			latestKey = obj.Key;
		}

		// retrieve latest model and store it.
		_client.GetObject(AWSClient.BUCKET_NAME, latestKey, GetModelObjectHandler);
	}

	void GetModelObjectHandler(AmazonServiceResult<GetObjectRequest, GetObjectResponse> cb) {
		GetObjectResponse resp = cb.Response;
		if (resp.ResponseStream != null) {
			byte[] modelBytes;
			using (Stream sr = resp.ResponseStream) {
				modelBytes = ReadStream(sr);
			}
			model = new TFModel(modelBytes);
		}
		if (model != null) {
			Debug.Log("Retrieved model " + resp.Key);
		}
	}
}
