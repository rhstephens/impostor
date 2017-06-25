using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This script will control how the local client interacts with other player's clients through the server
/// </summary>
public class PlayerNetworkSync : NetworkBehaviour {

    public float smoothFactor = 5f;

    [SyncVar]
    Vector3 syncPos;

    [SerializeField]
    Behaviour[] disabledComponents;

    void Start() {
        if (!isLocalPlayer) {
            foreach (Behaviour component in disabledComponents) {
                component.enabled = false;
            }
        }
    }

    void FixedUpdate() {
        TransmitMyPosition();
        SmoothPositions();
    }

    // This gets called on all NON LOCAL clients to smooth out their movement
    void SmoothPositions() {
        if (!isLocalPlayer) {
            transform.position = Vector3.Lerp(transform.position, syncPos, smoothFactor * Time.deltaTime);
        }
    }

    [Command]
    void CmdServerPosition(Vector3 pos) {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitMyPosition() {
        // Only transmit position if we are the local player
        if (isLocalPlayer) CmdServerPosition(transform.position);
    }
	
}
