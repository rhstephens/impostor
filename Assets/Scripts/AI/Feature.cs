using UnityEngine;

/// <summary>
/// A class that stores and calculates features to be used for the model.
/// </summary>
public class Feature {

	public const int NUM_FEATURES = 8;

	// X, Y position on the map
	public double PosX { get; set; }
	public double PosY { get; set; }

	// Distance from center
	public double CenterDist { get; set; }

	// Distance from nearest Player/AI
	public double OpponentX { get; set; }
	public double OpponentY { get; set; }

	// Is AI in motion?
	public int InMotion { get; set; }

	// Time in seconds while stationary/in motion
	public double TimeSinceStartOrStop { get; set; }

	// Time in seconds since last direction change
	public double TimeSinceLastDirection { get; set; }

	// True y output (see README.md)
	public int W { get; set; }
	public int A { get; set; }
	public int S { get; set; }
	public int D { get; set; }

	public double[] Xvals() {
		return new double[] {
			PosX, PosY, CenterDist, OpponentX, OpponentY, InMotion, TimeSinceStartOrStop, TimeSinceLastDirection
		};
	}

	public bool[] Yvals() {
		return new bool[] {
			W == 1, A == 1, S == 1, D == 1
		};
	}

	public static Feature GeneratePlayerFeatures(GameObject player, bool inMotion, float lastStop, float lastDirection, Vector2 yDirection) {
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
		f.W = yDirection.y == 1 ? 1 : 0;
		f.A = yDirection.x == -1 ? 1 : 0;
		f.S = yDirection.y == -1 ? 1 : 0;
		f.D = yDirection.x == 1 ? 1 : 0;

		return f;
	}
}
