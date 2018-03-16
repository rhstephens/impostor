// using Accord.MachineLearning;
// using Accord.Statistics.Models.Regression;

// using System;
// using System.IO;

// using UnityEngine;

// /// <summary>
// /// Wrapper class for the machine learning model used for AI. Contains 4 logistic regression classifiers for each of the 4 directions of input, WASD.
// /// Provides methods to calculate the AI's next movement
// /// </summary>
// [Serializable()]
// public class AIModel {

// 	// WASD classifiers
// 	public LogisticRegression wModel { get; set; }
// 	public LogisticRegression aModel { get; set; }
// 	public LogisticRegression sModel { get; set; }
// 	public LogisticRegression dModel { get; set; }

// 	// Normalization params
// 	public double[] mean { get; set; }
// 	public double[] std { get; set; }


// 	// Determine direction from predicted WASD output
// 	public Vector2 PredictDirection(Feature x) {
// 		double[] Xvals = x.Xvals();

// 		// convert to Z Score
// 		for (int i = 0; i < Feature.NUM_FEATURES; i++) {
// 			Xvals[i] = (Xvals[i] - mean[i]) / std[i];
// 		}

// 		int up = Convert.ToInt32(wModel.Decide(Xvals));
// 		int left = Convert.ToInt32(aModel.Decide(Xvals));
// 		int down = Convert.ToInt32(sModel.Decide(Xvals));
// 		int right = Convert.ToInt32(dModel.Decide(Xvals));

// 		// This returns a direction vector corresponding to which direction(s) the model predicts
// 		return new Vector2(right - left, up - down);
// 	}
// }
