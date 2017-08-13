using UnityEngine;
using UnityEngine.Networking;

public class CameraController : NetworkBehaviour {

    Camera playerCamera;

    void Start() {
        if (!isLocalPlayer) {
            AudioListener listener = GetComponentInChildren<AudioListener>();
            GUILayer gui = GetComponentInChildren<GUILayer>();
            FlareLayer flare = GetComponentInChildren<FlareLayer>();

            listener.enabled = false;
            gui.enabled = false;
            flare.enabled = false;
            return;
        }
        if (Camera.main) {
            Camera.main.gameObject.SetActive(false);
        }

        playerCamera = GetComponentInChildren<Camera>();
        playerCamera.enabled = true;
    }
}
