using CsvHelper;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// Generates a CSV file containing all Features added during this session. Stores result on S3.
/// </summary>
public class FeatureExporter {

	const string DATE_FORMAT = "yyyyMMdd-HHmm";

	List<Feature> _features = new List<Feature>();
	CsvWriter writer;
	string _fname = "features";

	public FeatureExporter() {
		
	}

	public FeatureExporter(string name) {
		_fname = name;
	}

	public void AddFeature(Feature f) {
		_features.Add(f);
	}

	public void ExportFeatures() {
		using (StreamWriter sw = new StreamWriter(FileName())) {
			CsvWriter csv = new CsvWriter(sw);
			csv.Configuration.RegisterClassMap<FeatureMap>();
			csv.Configuration.HasHeaderRecord = false;
			csv.WriteRecords(_features);
		}
	}

	string FileName() {
		return _fname + DateTime.Now.ToString(DATE_FORMAT) + ".csv";
	}
}
