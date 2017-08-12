using System.Collections.Generic;
using UnityEngine.Collections;
using UnityEngine;
/// <summary>
/// The game manager that players will interact with for client-side tasks
/// </summary>
public class GameManager : Singleton<GameManager> {

    Dictionary<int, GameObject> _playerList = new Dictionary<int, GameObject>();
    int _playerId = -1;

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

    // Find all current Players in the game and add to player list
    public void PopulatePlayerList() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
            RegisterPlayer(go);
        }
    }

    // Register this client's local player for quick access to the game object
    public void SetLocalPlayer(int id) {
        _playerId = id;
    }

    public GameObject GetLocalPlayer() {
        return FindPlayer(_playerId);
    }
}
