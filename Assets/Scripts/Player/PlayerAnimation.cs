using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator), typeof(PlayerController))]
public class PlayerAnimation : NetworkBehaviour {

    Animator anim;
    PlayerController con;

    [SyncVar(hook = "OnGunDrawn")]
    bool gunDrawnSync;

    bool draw;

    static string GUN_DRAWN_PARAM = "gunDrawn";

    void Start () {
        anim = GetComponent<Animator>();
        con = GetComponent<PlayerController>();
    }

    void Update() {
        if (Input.GetButtonDown("Fire2")) {
            DrawGun();
        }
    }

    // Any time our server changes the 'drawn' variable, we update the animator
    void OnGunDrawn(bool drawn) {
        anim.SetBool(GUN_DRAWN_PARAM, drawn);
    }

    // Send our player's 'draw' variable to the server
    void DrawGun() {
        if (!isLocalPlayer) {
            return;
        }
        draw = !draw;
        con.gunDrawn = draw;
        CmdDrawGun(draw);
    }

    [Command]
    void CmdDrawGun(bool draw) {
        gunDrawnSync = draw;
    }
}
