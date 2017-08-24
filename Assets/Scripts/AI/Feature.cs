using UnityEngine;

/// <summary>
/// A class that stores and calculates features to be used for the model.
/// </summary>
public class Feature {

	// X, Y position on the map
	public float PosX { get; set; }
	public float PosY { get; set; }

	// Distance from center
	public float CenterDist { get; set; }

	// Distance from nearest Player/AI
	public float OpponentX { get; set; }
	public float OpponentY { get; set; }

	// Is AI in motion?
	public int InMotion { get; set; }

	// Time in seconds while stationary/in motion
	public float TimeSinceStartOrStop { get; set; }

	// Time in seconds since last direction change
	public float TimeSinceLastDirection { get; set; }

	// True y output (see README.md)
	public int W { get; set; }
	public int A { get; set; }
	public int S { get; set; }
	public int D { get; set; }

	public static Feature GeneratePlayerFeatures(GameObject player, bool inMotion, float lastStop, float lastDirection, Vector2 direction) {
		Feature f = new Feature();
		Vector3 playerPos = player.transform.position;

		// Player specific features
		f.PosX = playerPos.x;
		f.PosY = playerPos.y;

		// Always considered to be distance from point (0, 0, 0)
		f.CenterDist = Vector3.Distance(Vector3.zero, playerPos);

		// Nearest opponent
		GameObject opp = GameManager.Instance.FindClosestPlayer(player);
		if (!opp) {
			f.OpponentX = 0;
			f.OpponentY = 0;
		} else {
			Vector3 oppPos = opp.transform.position;
			f.OpponentX = oppPos.x - playerPos.x;
			f.OpponentY = oppPos.y - playerPos.y;
		}

		// Time, Direction and Motion (supplied by the AI Controller)
		f.InMotion = inMotion ? 1 : 0;
		f.TimeSinceStartOrStop = lastStop;
		f.TimeSinceLastDirection = lastDirection;

		// y outputs
		f.W = direction.y == 1 ? 1 : 0;
		f.A = direction.x == -1 ? 1 : 0;
		f.S = direction.y == -1 ? 1 : 0;
		f.D = direction.x == 1 ? 1 : 0;

		return f;
	}
}
