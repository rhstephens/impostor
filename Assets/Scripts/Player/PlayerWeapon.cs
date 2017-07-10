using UnityEngine;
using UnityEngine.Networking;

public class PlayerWeapon : NetworkBehaviour {

    public LayerMask toHit;

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
        Vector2 direction = cam.ScreenToWorldPoint(Input.mousePosition) - gunLocation.transform.position;
        Debug.DrawRay(gunLocation.transform.position, direction, Color.red);
        hit = Physics2D.Raycast(gunLocation.transform.position, direction, weapon.Range, toHit);
        if (hit) {
            int id = hit.transform.gameObject.GetInstanceID();

            if (hit.transform.tag == "Player") {
                Debug.Log(hit.transform.gameObject.GetComponent<NetworkInstanceId>().ToString());
                GameManager.Instance.RegisterPlayer(hit.transform.gameObject);
                CmdShootPlayer(id, weapon.Damage);
            }
        }

    }

    [Command]
    void CmdShootPlayer(int id, int dmg) {
        // Find the unique player that was hit.
        GameObject hit = GameManager.Instance.FindPlayer(id);

        PlayerHealth hp = hit.GetComponent<PlayerHealth>();
        if (hp) {
            hp.InflictDamage(dmg);
        }
    }
}
