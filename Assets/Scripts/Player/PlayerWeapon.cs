using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeapon : NetworkBehaviour {

    IWeapon weapon;
    PlayerController con;
    RaycastHit2D hit;

    [SerializeField]
    GameObject gunLocation;
    [SerializeField]
    Camera cam;

    void Start() {
        weapon = GameManager.Instance.GetPlayerWeapon();
        con = GetComponent<PlayerController>();
    }

    void Update() {
        CheckTrigger();
    }

    void CheckTrigger() {
        if (!isLocalPlayer) {
            return;
        }

        if (Input.GetButtonDown("Fire1") && con.gunDrawn) {
            FireShot();
        }
    }

    void FireShot() {
        hit = Physics2D.Raycast(gunLocation.transform.position, cam.ScreenToWorldPoint(Input.mousePosition), weapon.Range);
        if (hit) {
            Debug.Log("Hit something: " + hit.transform.name);

        }

    }
}
