using UnityEngine;

/// <summary>
/// A class that stores and calculates features to be used for the model.
/// </summary>
public class Feature {

	// X, Y position on the map
	public float PosX { get; set; }
	public float PosY { get; set; }

	// Distance from center
	public float CenterX { get; set; }
	public float CenterY { get; set; }

	// Distance from nearest Player/AI
	public float OpponentX { get; set; }
	public float OpponentY { get; set; }

	// Is AI in motion?
	public bool InMotion { get; set; }

	// Time in seconds while stationary/in motion
	public float TimeSinceStartOrStop { get; set; }

	// Time in seconds since last direction change
	public float TimeSinceLastDirection { get; set; }

	public static Feature GeneratePlayerFeatures(GameObject player, bool inMotion, float lastStop, float lastDirection) {
		Feature f = new Feature ();
		Vector3 playerPos = player.transform.position;

		// Player specific features
		f.PosX = playerPos.x;
		f.PosY = playerPos.y;

		// Always considered to be (0, 0)
		f.CenterX = 0;
		f.CenterY = 0;

		// Nearest opponent
		GameObject opp = GameManager.Instance.FindClosestPlayer(player);
		if (!opp) {
			f.OpponentX = 0;
			f.OpponentY = 0;
		} else {
			Vector3 oppPos = opp.transform.position;
			f.OpponentX = Mathf.Abs(oppPos.x - playerPos.x);
			f.OpponentY = Mathf.Abs(oppPos.y - playerPos.y);
		}

		// Time, Direction and Motion (supplied by the AI Controller)
		f.InMotion = inMotion;
		f.TimeSinceStartOrStop = lastStop;
		f.TimeSinceLastDirection = lastDirection;

		return f;
	}
}
