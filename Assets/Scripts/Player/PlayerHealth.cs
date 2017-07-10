using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

    static int MAX_HEALTH = 100;

    [SyncVar (hook = "OnHealthChanged")]
    public int health = MAX_HEALTH;

	
	void Start () {
		
	}

    public void InflictDamage(int dmg) {
        health = Mathf.Clamp((health - dmg), 0, MAX_HEALTH);
    }

    public void OnHealthChanged(int newHealth) {
        Debug.Log(transform.tag + "'s health has been changed");
        health = newHealth;
    }
}
