using CsvHelper.Configuration;

public class FeatureMap : CsvClassMap<Feature> {

	// Most features are of floating-point precicion, however we want to store this in a csv to only 3 decimal-places
	const string DISPLAY_FORMAT = "0.###";

	public FeatureMap() {
		Map (m => m.PosX.ToString(DISPLAY_FORMAT)).Index(0);
		Map (m => m.PosY.ToString(DISPLAY_FORMAT)).Index(1);
		Map (m => m.CenterX.ToString(DISPLAY_FORMAT)).Index(2);
		Map (m => m.CenterY.ToString(DISPLAY_FORMAT)).Index(3);
		Map (m => m.OpponentX.ToString(DISPLAY_FORMAT)).Index(4);
		Map (m => m.OpponentY.ToString(DISPLAY_FORMAT)).Index(5);
		Map (m => m.InMotion ? "1" : "0").Index(6);
		Map (m => m.TimeSinceStartOrStop.ToString(DISPLAY_FORMAT)).Index(7);
		Map (m => m.TimeSinceLastDirection.ToString(DISPLAY_FORMAT)).Index(8);
    }
}
