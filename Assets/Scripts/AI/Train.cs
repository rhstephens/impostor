using CsvHelper;
using System.Collections.Generic;
using System.IO;

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

using Accord.MachineLearning;
using Accord.Statistics.Models.Regression;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generates a training set from S3 then fits it to a Logistic Regression model!
/// </summary>
public class Train : MonoBehaviour {

	const string BUCKET_NAME = "codetroopa-imposter";
	const string TSET_PREFIX = "training-sets";

	public Button GetTrainingSetBtn = null;
	public Button TrainModelBtn = null;
	public List<Feature> features = new List<Feature>();
	public List<string> keys = new List<string>();

	AWSClient _client;

	// choo-choo
	public void Start() {
		_client = GameObject.Find("AWS").GetComponent<AWSClient>();

		// Buttons
		GetTrainingSetBtn.onClick.AddListener(() => {
			LatestTrainingSet();
		});

		TrainModelBtn.onClick.AddListener(() => {
			TrainModel();
		});
	}

	// Go through each training set segment and collect all features.
	void LatestTrainingSet() {
		Debug.Log("Begin downloading training set...");
		_client.ListObjects(BUCKET_NAME, TSET_PREFIX, ListObjectsHandler);
	}

	// LET'S DO THIS
	void TrainModel() {
		int m = features.Count;
		double[][] X = new double[m][];

		// Extract X values from training set
		for (int i = 0; i < m; i++) {
			X[i] = features[i].Xvals();
		}

		// Extract y values from training set


		LogisticRegression model;
	}

	// A callback that handles a ListObjectsAsync request. Stores each object key.
	void ListObjectsHandler(AmazonServiceResult<ListObjectsRequest, ListObjectsResponse> cb) {
		if (cb.Exception != null) {
			Debug.Log(cb.Exception);
		}

		ListObjectsResponse resp = cb.Response;
		foreach (S3Object obj in resp.S3Objects) {
			keys.Add(obj.Key);
		}

		Debug.Log("Found Objects. Going through each...");
		foreach (string key in keys) {
			_client.GetObject(BUCKET_NAME, key, GetObjectHandler);
		}
	}

	// A callback that handles a GetObjectAsync request. Parses the bytes result into our Feature class.
	void GetObjectHandler(AmazonServiceResult<GetObjectRequest, GetObjectResponse> cb) {
		if (cb.Exception != null) {
			Debug.Log(cb.Exception);
			return;
		}

		GetObjectResponse resp = cb.Response;
		if (resp.ResponseStream != null) {
			using (StreamReader sr = new StreamReader(resp.ResponseStream)) {
				CsvReader csv = new CsvReader(sr);
				csv.Configuration.RegisterClassMap<FeatureMap>();
				features.AddRange(csv.GetRecords<Feature>());
				Debug.Log("Feature total: " + features.Count.ToString());
			}
		}
	}
}
