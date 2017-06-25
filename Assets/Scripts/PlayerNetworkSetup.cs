using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] disabledComponents;

    void Start() {
        if (!isLocalPlayer) {
            foreach (Behaviour component in disabledComponents) {
                component.enabled = false;
            }
        }
    }
	
}
