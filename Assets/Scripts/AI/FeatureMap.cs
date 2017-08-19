using CsvHelper.Configuration;

public class FeatureMap : CsvClassMap<Feature> {

	public FeatureMap() {
		Map (m => m.PosX).Index (0);
		Map (m => m.PosY).Index (1);
    }
}
