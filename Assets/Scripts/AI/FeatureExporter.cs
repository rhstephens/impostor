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
	const string BUCKET_NAME = "codetroopa-imposter";
	const string FILE_PREFIX = "features_";

	List<Feature> _features = new List<Feature>();
	CsvWriter writer;
	AWSClient _client;

	public FeatureExporter(AWSClient client) {
		_client = client;
	}

	public void AddFeature(Feature f) {
		_features.Add(f);
	}

	// Iterates through the list of Features and stores them on S3 as a CSV file.
	public void ExportFeatures() {
		// write to temp file
		using (StreamWriter sw = new StreamWriter(FileName())) {
			CsvWriter csv = new CsvWriter(sw);
			csv.Configuration.RegisterClassMap<FeatureMap>();
			csv.Configuration.HasHeaderRecord = false;
			csv.WriteRecords(_features);
		}

		_client.PostObject(BUCKET_NAME, S3Key(), FileName());
	}

	string S3Key() {
		return "training-sets/" + FileName();
	}

	string FileName() {
		return FILE_PREFIX + DateTime.Now.ToString(DATE_FORMAT) + ".csv";
	}
}
