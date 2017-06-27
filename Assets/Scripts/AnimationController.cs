using UnityEngine;

[RequireComponent(typeof(Animator), typeof(PlayerController))]
public class AnimationController : MonoBehaviour {

    Animator anim;
    PlayerController con;

    static string GUN_DRAWN_PARAM = "gunDrawn";

	void Start () {
        anim = GetComponent<Animator>();
        con = GetComponent<PlayerController>();
    }

    void Update() {
        anim.SetBool(GUN_DRAWN_PARAM, con.gunDrawn);
    }
}
