// using CsvHelper;
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Runtime.Serialization.Formatters.Binary;

// using Amazon;
// using Amazon.Runtime;
// using Amazon.S3;
// using Amazon.S3.Model;

// using Accord.MachineLearning;
// using Accord.Math.Optimization;
// using Accord.Statistics.Models.Regression;
// using Accord.Statistics.Models.Regression.Fitting;

// using UnityEngine;
// using UnityEngine.UI;

// /// <summary>
// /// Generates a training set from S3 then fits it to a Logistic Regression model!
// /// </summary>
// public class Train : MonoBehaviour {

// 	const string DATE_FORMAT = "yyyyMMdd-HHmm";
// 	const string MODEL_PREFIX = "model_";
// 	const string TSET_PREFIX = "training-sets";

// 	public Button GetTrainingSetBtn = null;
// 	public Button TrainModelBtn = null;
// 	public List<Feature> features = new List<Feature>();
// 	public List<string> keys = new List<string>();

// 	AWSClient _client;

// 	// choo-choo
// 	public void Start() {
// 		_client = GameObject.Find("AWS").GetComponent<AWSClient>();

// 		// Buttons
// 		GetTrainingSetBtn.onClick.AddListener(() => {
// 			LatestTrainingSet();
// 		});

// 		TrainModelBtn.onClick.AddListener(() => {
// 			TrainModel();
// 		});
// 	}

// 	// Go through each training set segment and collect all features.
// 	void LatestTrainingSet() {
// 		Debug.Log("Begin downloading training set...");
// 		_client.ListObjects(AWSClient.BUCKET_NAME, TSET_PREFIX, ListObjectsHandler);
// 	}

// 	// LET'S DO THIS
// 	void TrainModel() {
// 		Debug.Log("Begin training model...");
// 		float beginTime = Time.time;
// 		int m = features.Count;
// 		double[][] X = new double[m][];
// 		bool[] yW = new bool[m];
// 		bool[] yA = new bool[m];
// 		bool[] yS = new bool[m];
// 		bool[] yD = new bool[m];

// 		// Extract X and y values from training set
// 		for (int i = 0; i < m; i++) {
// 			X[i] = features[i].Xvals();
// 			yW[i] = features[i].W == 1;
// 			yA[i] = features[i].A == 1;
// 			yS[i] = features[i].S == 1;
// 			yD[i] = features[i].D == 1;
// 		}

// 		// Calculate mean and standard deviation from X values then apply it to transform to Z-Score
// 		double[] featureSums = new double[m];
// 		double[] sqDiffSums = new double[m];
// 		double[] mean = new double[m];
// 		double[] std = new double[m];

// 		// mean
// 		for (int i = 0; i < m; i++) {
// 			for (int j = 0; j < Feature.NUM_FEATURES; j++) {
// 				featureSums[j] += X[i][j];
// 			}
// 		}
// 		for (int i = 0; i < featureSums.Length; i++) {
// 			mean[i] = featureSums[i] / m;
// 		}

// 		// std
// 		for (int i = 0; i < m; i++) {
// 			for (int j = 0; j < Feature.NUM_FEATURES; j++) {
// 				sqDiffSums[j] += Math.Pow(X[i][j] - mean[j], 2);
// 			}
// 		}
// 		for (int i = 0; i < featureSums.Length; i++) {
// 			std[i] = Math.Sqrt(sqDiffSums[i] / (m - 1));
// 		}

// 		// Z-score the input data
// 		for (int i = 0; i < m; i++) {
// 			for (int j = 0; j < Feature.NUM_FEATURES; j++) {
// 				X[i][j] = (X[i][j] - mean[j]) / std[j];
// 			}
// 		}

// 		Debug.Log("Training logistic regression model with " + features.Count.ToString() + " examples");
// 		var learner = new IterativeReweightedLeastSquares<LogisticRegression>() {
// 			Tolerance = 1e-4,
// 			MaxIterations = 100,
// 			Regularization = 0
// 		};
// 		LogisticRegression modelW = learner.Learn(X, yW);
// 		LogisticRegression modelA = learner.Learn(X, yA);
// 		LogisticRegression modelS = learner.Learn(X, yS);
// 		LogisticRegression modelD = learner.Learn(X, yD);

// 		Debug.Log("Training model took " + (Time.time - beginTime).ToString() + " seconds.");
// 		AIModel model = new AIModel() {
// 			wModel = modelW,
// 			aModel = modelA,
// 			sModel = modelS,
// 			dModel = modelD,

// 			mean = mean,
// 			std = std
// 		};

// 		// serialize model
// 		string filename = ModelFileName();
// 		FileStream stream = new FileStream(filename, FileMode.Create);
// 		BinaryFormatter formatter = new BinaryFormatter();
// 		formatter.Serialize(stream, model);
// 		stream.Dispose();

// 		// store on s3
// 		_client.PostObject(AWSClient.BUCKET_NAME, "models/" + filename, filename);
// 	}

// 	// A callback that handles a ListObjectsAsync request. Stores each object key.
// 	void ListObjectsHandler(AmazonServiceResult<ListObjectsRequest, ListObjectsResponse> cb) {
// 		if (cb.Exception != null) {
// 			Debug.Log(cb.Exception);
// 		}

// 		ListObjectsResponse resp = cb.Response;
// 		foreach (S3Object obj in resp.S3Objects) {
// 			keys.Add(obj.Key);
// 		}

// 		Debug.Log("Found Objects. Going through each...");
// 		foreach (string key in keys) {
// 			_client.GetObject(AWSClient.BUCKET_NAME, key, GetObjectHandler);
// 		}
// 	}

// 	// A callback that handles a GetObjectAsync request. Parses the bytes result into our Feature class.
// 	void GetObjectHandler(AmazonServiceResult<GetObjectRequest, GetObjectResponse> cb) {
// 		if (cb.Exception != null) {
// 			Debug.Log(cb.Exception);
// 			return;
// 		}

// 		GetObjectResponse resp = cb.Response;
// 		if (resp.ResponseStream != null) {
// 			using (StreamReader sr = new StreamReader(resp.ResponseStream)) {
// 				CsvReader csv = new CsvReader(sr);
// 				csv.Configuration.RegisterClassMap<FeatureMap>();
// 				features.AddRange(csv.GetRecords<Feature>());
// 				Debug.Log("Feature total: " + features.Count.ToString());
// 			}
// 		}}

// 	string ModelFileName() {
// 		return MODEL_PREFIX + DateTime.Now.ToString(DATE_FORMAT) + ".dat";
// 	}
// }
