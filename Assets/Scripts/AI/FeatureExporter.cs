using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

/// <summary>
/// Generates a CSV file containing all Features added during this session. Stores result on S3.
/// </summary>
public class FeatureExporter {

	const string DATE_FORMAT = "yyyyMMdd-HHmm";
	const string FOLDER_PREFIX = "features_";

	List<int[,]> _playerMatrices = new List<int[,]>();
	List<int[,]> _obstacleMatrices = new List<int[,]>();
	List<int[,]> _enemyMatrices = new List<int[,]>();
	List<int[]> _ylabels = new List<int[]>();
	AWSClient _client;

	public FeatureExporter() {
		_client = GameManager.Instance.Client;
	}

	public void AddPlayerMatrix(int[,] matrix) {
		_playerMatrices.Add(matrix);
	}

	public void AddObstacleMatrix(int[,] matrix) {
		_obstacleMatrices.Add(matrix);
	}

	public void AddEnemyMatrix(int[,] matrix) {
		_enemyMatrices.Add(matrix);
	}

	public void AddLabelledData(int[] ylabel) {
		_ylabels.Add(ylabel);
	}

	// Iterates through the list of Features and stores them on S3 in multiple files.
	public void ExportFeatures() {
		// validation
		validate();

		// write to temp files
		using (StreamWriter sw = new StreamWriter("playerMatrices.csv")) {
			CsvWriter csv = new CsvWriter(sw);
			csv.Configuration.HasHeaderRecord = false;
			foreach (int[,] matrix in _playerMatrices) {
				csv.WriteRecords(matrix);
			}
		}

		using (StreamWriter sw = new StreamWriter("obstacleMatrices.csv")) {
			CsvWriter csv = new CsvWriter(sw);
			csv.Configuration.HasHeaderRecord = false;
			foreach (int[,] matrix in _obstacleMatrices) {
				csv.WriteRecords(matrix);
			}
		}

		using (StreamWriter sw = new StreamWriter("enemyMatrices.csv")) {
			CsvWriter csv = new CsvWriter(sw);
			csv.Configuration.HasHeaderRecord = false;
			foreach (int[,] matrix in _enemyMatrices) {
				csv.WriteRecords(matrix);
			}
		}

		using (StreamWriter sw = new StreamWriter("ylabels.csv")) {
			CsvWriter csv = new CsvWriter(sw);
			csv.Configuration.HasHeaderRecord = false;
			foreach (int[] ylabel in _ylabels) {
				csv.WriteRecords(ylabel);
			}
		}

		// meta data (terrible and unextensible way to write json, but I am lazy)
		using (StreamWriter sw = new StreamWriter("metadata.json")) {
			string data = string.Format("\"matrix_length\": {0}, \"matrix_width\": {1}, \"matrix_count\": {2}, \"ylabel_length\": {3}",
				GameManager.GRID_LENGTH,
				GameManager.GRID_WIDTH,
				_playerMatrices.Count,
				GameManager.YLABEL_LENGTH);
			sw.WriteLine("{" + data + "}");
		}

		// post to S3
		_client.PostObject(AWSClient.BUCKET_NAME, S3Key("playerMatrices.csv"), "playerMatrices.csv");
		_client.PostObject(AWSClient.BUCKET_NAME, S3Key("obstacleMatrices.csv"), "obstacleMatrices.csv");
		_client.PostObject(AWSClient.BUCKET_NAME, S3Key("enemyMatrices.csv"), "enemyMatrices.csv");
		_client.PostObject(AWSClient.BUCKET_NAME, S3Key("ylabels.csv"), "ylabels.csv");
		_client.PostObject(AWSClient.BUCKET_NAME, S3Key("metadata.json"), "metadata.json");
	}

	string S3Key(string filePrefix) {
		return "unprocessed_training_data/" + FolderName() + filePrefix;
	}

	string FolderName() {
		return FOLDER_PREFIX + DateTime.Now.ToString(DATE_FORMAT) + "/";
	}

	private void validate() {
		int playerCount = _playerMatrices.Count;
		int obstacleCount = _obstacleMatrices.Count;
		int enemyCount = _enemyMatrices.Count;

		if (playerCount != obstacleCount || playerCount != enemyCount) {
			throw new System.InvalidOperationException(string.Format("Mismatch between player({0}), obstacle({1}), and enemy({2}) matrix lengths",
			playerCount, obstacleCount, enemyCount));
		}
		
	}
}
