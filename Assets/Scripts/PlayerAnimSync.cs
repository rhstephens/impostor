using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator), typeof(PlayerController))]
public class PlayerAnimSync : NetworkBehaviour {

    Animator anim;
    PlayerController con;

    bool gunDrawn;
    static string GUN_DRAWN_PARAM = "gunDrawn";

    void Start () {
        anim = GetComponent<Animator>();
    }

    void Update() {
        if (Input.GetButtonDown("Fire2")) {
            gunDrawn = !gunDrawn;
        }
        PerformAnimations();
    }

    void PerformAnimations() {
        if (!isLocalPlayer) {
            return;
        }
        anim.SetBool(GUN_DRAWN_PARAM, gunDrawn);
    }
}
