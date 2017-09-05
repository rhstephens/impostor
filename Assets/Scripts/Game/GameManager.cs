using System.Collections.Generic;
using System.Collections;
using UnityEngine.Collections;
using UnityEngine;
/// <summary>
/// The game manager that players will interact with for client-side tasks
/// </summary>
public class GameManager : Singleton<GameManager> {

	Dictionary<int, GameObject> _playerList = new Dictionary<int, GameObject>();
	Dictionary<int, GameObject> _aiList = new Dictionary<int, GameObject>();
	AWSClient _client = null;
	AIModel model;

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
}
