using UnityEngine;

/// <summary>
/// A class that stores and calculates features to be used for the model.
/// </summary>
public class Feature : MonoBehaviour {

	// X, Y position on the map
	public int PosX { get; set; }
	public int PosY { get; set; }

	// Distance from center
	public int CenterX { get; set; }
	public int CenterY { get; set; }

	// Distance from nearest Player/AI
	public int OpponentX { get; set; }
	public int OpponentY { get; set; }

	// Is AI in motion?
	public bool InMotion { get; set; }

	// Time in ms while stationary/in motion
	public int TimeSinceStartOrStop { get; set; }

	// Time since last direction change
	public int TimeSinceLastDirection { get; set; }

	public static Feature GeneratePlayerFeatures(GameObject go) {
		Feature f = new Feature ();


		return f;
	}
}
