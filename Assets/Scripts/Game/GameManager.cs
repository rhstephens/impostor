using System.Collections.Generic;
using System.Collections;
using UnityEngine.Collections;
using UnityEngine;

/// <summary>
/// The game manager that players will interact with for client-side tasks
/// </summary>
public class GameManager : Singleton<GameManager> {

	public Transform topLeft;
	public Transform topRight;
	public Transform bottomLeft;
	public Transform bottomRight;

	Dictionary<int, GameObject> _playerList = new Dictionary<int, GameObject>();
	Dictionary<int, GameObject> _aiList = new Dictionary<int, GameObject>();
	Dictionary<int, GameObject> _obsList = new Dictionary<int, GameObject>();
	AWSClient _client = null;
    int _playerId = -1;

	public AWSClient Client {
		get {
			if (_client == null) {
				_client = GameObject.Find("AWS").GetComponent<AWSClient>();
			}
			return _client;
		}
	}

	public void Start() {
		PopulateAIList();
	}

	public IWeapon GetPlayerWeapon() {
        //TODO: Make this smarter... much smarter
        return new DesertEagle();
    }

    public void KillPlayer(GameObject go) {
        //TODO: Player Death Sound Effect (willhelm scream)

        // Re-enable main Camera for dead player
        Camera.main.enabled = true;
        Destroy(go);
    }

    // Find player by unique InstanceID
    public GameObject FindPlayer(int id) {
        if (!_playerList.ContainsKey(id)) {
            return null;
        }
        return _playerList[id];
    }

	// Add instanceId -> Player mapping for quick GameObject search
	void RegisterPlayer(GameObject go) {
		int id = go.GetInstanceID();
		if (_playerList.ContainsKey(id)) {
			_playerList[id] = go;
		} else {
			_playerList.Add(id, go);
		}
	}

	// Add instanceId -> AI mapping for quick GameObject search
	void RegisterAI(GameObject go) {
		int id = go.GetInstanceID();
		if (_aiList.ContainsKey(id)) {
			_aiList[id] = go;
		} else {
			_aiList.Add(id, go);
		}
	}

    // Find all current Players in the game and add to player list
    public void PopulatePlayerList() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
            RegisterPlayer(go);
        }
    }

	// Find all AI in the game and add to player list
	public void PopulateAIList() {
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("AI")) {
			RegisterAI(go);
		}
	}

	// Find all Obstacles in the game and add to obstacle list
	public void PopulateObstacleList() {
		foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject))) {
			if (go.layer == LayerMask.NameToLayer("Obstacle") || go.layer == LayerMask.NameToLayer("Wall")) {

			}
		}
	}

	// Finds and returns the closest player/AI to given a player/AI. Null if there is none
	public GameObject FindClosestPlayer(GameObject player) {
		GameObject closestPlayer = null;
		float closestDistance = 1000f;

		foreach (KeyValuePair<int, GameObject> pair in _aiList) {
			GameObject ai = pair.Value;
			if (!ai.activeInHierarchy) {
				continue;
			}
			if (ai.Equals(player)) {
				continue;
			}
			float distance = Vector3.Distance(player.transform.position, ai.transform.position);
			if (distance < closestDistance) {
				closestDistance = distance;
				closestPlayer = ai;
			}
		}

		foreach (KeyValuePair<int, GameObject> pair in _playerList) {
			GameObject p = pair.Value;
			if (!p.activeInHierarchy) {
				continue;
			}
			if (p.Equals(player)) {
				continue;
			}
			float distance = Vector3.Distance(player.transform.position, p.transform.position);
			if (distance < closestDistance) {
				closestDistance = distance;
				closestPlayer = p;
			}
		}
		return closestPlayer;
	}

    // Register this client's local player for quick access to the game object
    public void SetLocalPlayer(int id) {
        _playerId = id;
    }

    public GameObject GetLocalPlayer() {
        return FindPlayer(_playerId);
    }

	//////////////////////////////////////////////
	// Input data for Convolutional Neural Network
	//////////////////////////////////////////////

	public const int GRID_LENGTH = 200;
	public const int GRID_WIDTH = 100;
	public const int NUM_CHANNELS = 3;
	public const int YLABEL_LENGTH = 9;

	// Determines the grid location (x, y) of the given object position.
	public Vector2 gridLocation(Vector2 pos) {
		// Length and Width of our grid. Could change dynamically, so we must calculate these each time
		float len = Mathf.Abs(topRight.position.x - topLeft.position.x);
		float wid = Mathf.Abs(topLeft.position.y - bottomLeft.position.y);

		// How much space one grid unit takes up
		float stepsizeX = len / GRID_LENGTH;
		float stepsizeY = wid / GRID_WIDTH;

		// Calculate the grid position and clamp it within bounds
		int posX = Mathf.FloorToInt(Mathf.Abs(pos.x - topLeft.position.x) / stepsizeX);
		int posY = Mathf.FloorToInt(Mathf.Abs(pos.y - topLeft.position.y) / stepsizeY);
		posX = Mathf.Clamp(posX, 0, GRID_LENGTH - 1);
		posY = Mathf.Clamp(posY, 0, GRID_WIDTH - 1);

		return new Vector2(posX, posY);
	}

	// Generates a matrix respresentation of the map. Grids that contain the current player are 1, the rest are 0.
	public int[,] GeneratePlayerMatrix(GameObject player) {
		int[,] matrix = new int[GRID_WIDTH, GRID_LENGTH];

		// Let matrix be all zeroes except at the Player's position
		Vector2 playerGridLoc = gridLocation(new Vector2(player.transform.position.x, player.transform.position.y));
		matrix[(int)playerGridLoc.y, (int)playerGridLoc.x] = 1;

		return matrix;
	}

	// Generates a matrix respresentation of the map. Grids that contain an obstacle are 1, the rest are 0.
	public int[,] GenerateObstacleMatrix() {
		int[,] matrix = new int[GRID_WIDTH, GRID_LENGTH];

		// Length and Width of our grid. Could change dynamically, so we must calculate these each time
		float len = Mathf.Abs(topRight.position.x - topLeft.position.x);
		float wid = Mathf.Abs(topLeft.position.y - bottomLeft.position.y);

		// How much space one grid unit takes up
		float stepsizeX = len / GRID_LENGTH;
		float stepsizeY = wid / GRID_WIDTH;

		// Fire a ray into every grid to detect an obstacle or wall
		for (int i = 0; i < GRID_WIDTH; i++) {
			for (int j = 0; j < GRID_LENGTH; j++) {
				float xPos = topLeft.position.x + (j * stepsizeX) + (stepsizeX / 2);
				float yPos = topLeft.position.y - ((i * stepsizeY) + (stepsizeY / 2));
				Vector2 rayPoint = new Vector2(xPos, yPos);

				RaycastHit2D hit = Physics2D.Raycast(rayPoint, Vector2.zero);
				if (hit.collider) {
					int mask = hit.collider.gameObject.layer;
					if (mask == LayerMask.NameToLayer("Obstacle") || mask == LayerMask.NameToLayer("Wall")) {
						matrix[i, j] = 1;
					}
				}
			}
		}

		return matrix;
	}

	// Generates a matrix respresentation of the map. Grids that contain an enemy Player or AI are 1, the rest are 0.
	public int[,] GenerateEnemyMatrix() {
		int[,] matrix = new int[GRID_WIDTH, GRID_LENGTH];
		Vector2 playerGridLoc;

		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
			playerGridLoc = gridLocation(new Vector2(go.transform.position.x, go.transform.position.y));
			matrix[(int)playerGridLoc.y, (int)playerGridLoc.x] = 1;
		}

		foreach (GameObject go in GameObject.FindGameObjectsWithTag("AI")) {
			playerGridLoc = gridLocation(new Vector2(go.transform.position.x, go.transform.position.y));
			matrix[(int)playerGridLoc.y, (int)playerGridLoc.x] = 1;
		}

		// ensure local player isn't added to the enemy matrix
		playerGridLoc = gridLocation(this.GetLocalPlayer().transform.position);
		matrix[(int)playerGridLoc.y, (int)playerGridLoc.x] = 0;

		return matrix;
	}
}
