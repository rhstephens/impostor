using CsvHelper.Configuration;

public class FeatureMap : CsvClassMap<Feature> {

	public FeatureMap() {
		Map (m => m.PosX).Index(0);
		Map (m => m.PosY).Index(1);
		Map (m => m.CenterDist).Index(2);
		Map (m => m.OpponentX).Index(3);
		Map (m => m.OpponentY).Index(4);
		Map (m => m.InMotion).Index(5);
		Map (m => m.TimeSinceStartOrStop).Index(6);
		Map (m => m.TimeSinceLastDirection).Index(7);
		Map (m => m.W).Index(8);
		Map (m => m.A).Index(9);
		Map (m => m.S).Index(10);
		Map (m => m.D).Index(11);
    }
}
