using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator), typeof(PlayerController))]
public class AnimationSync : NetworkBehaviour {

    Animator anim;
    PlayerController con;

    [SyncVar]
    bool syncGunDrawn;

	void Start () {
        anim = GetComponent<Animator>();
        con = GetComponent<PlayerController>();
    }

    void Update() {
        SyncAnim();

        PerformAnimations();
    }

    void PerformAnimations() {
        if (!isLocalPlayer) {
            anim.SetBool(PlayerController.GUN_DRAWN_PARAM, syncGunDrawn);
        }
    }

    [Command]
    void CmdGunDrawn(bool drawn) {
        syncGunDrawn = drawn;
    }

    [ClientCallback]
    void SyncAnim() {
        if (isLocalPlayer) {
            CmdGunDrawn(con.gunDrawn);
        }
    }
}
