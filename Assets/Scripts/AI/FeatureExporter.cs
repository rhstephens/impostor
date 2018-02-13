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
	AWSClient _client;

	public FeatureExporter() {
		_client = GameManager.Instance.Client;
	}

	public void AddPlayerMatrix(List<int[,]> matrix) {
		_playerMatrices.Add(matrix);
	}

	public void AddObstacleMatrix(List<int[,]> matrix) {
		_obstacleMatrices.Add(matrix);
	}

	public void AddEnemyMatrix(List<int[,]> matrix) {
		_enemyMatrices.Add(matrix);
	}

	// Iterates through the list of Features and stores them on S3 in multiple files.
	public void ExportFeatures() {
		// write to temp file
		using (StreamWriter sw = new StreamWriter(FileName())) {
			CsvWriter csv = new CsvWriter(sw);
			csv.Configuration.RegisterClassMap<FeatureMap>();
			csv.Configuration.HasHeaderRecord = false;
			csv.WriteRecords(_features);
		}

		_client.PostObject(AWSClient.BUCKET_NAME, S3Key(), FileName());
	}

	string S3Key(string filePrefix) {
		return "training_sets/" + FolderName() + filePrefix + ".csv";
	}

	string FolderName() {
		return FOLDER_PREFIX + DateTime.Now.ToString(DATE_FORMAT) + "/";
	}
}
