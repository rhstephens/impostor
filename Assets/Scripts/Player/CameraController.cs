using UnityEngine;
using UnityEngine.Networking;

public class CameraController : NetworkBehaviour {

    Camera playerCamera;

    void Start() {
        if (!isLocalPlayer) return;
        if (Camera.main) {
            Camera.main.gameObject.SetActive(false);
        }
        playerCamera = GetComponentInChildren<Camera>();
        playerCamera.enabled = true;
    }
}
