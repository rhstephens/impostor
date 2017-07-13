using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour {

    static int MAX_HEALTH = 100;

    Text healthText;

    [SyncVar (hook = "OnHealthChanged")]
    int health = MAX_HEALTH;

	
	void Start () {
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
	}

    // Inflict damage on server.
    public void InflictDamage(int dmg) {
        if (!isServer) return;
        Debug.Log("Dealing damage on server");
        health = Mathf.Clamp((health - dmg), 0, MAX_HEALTH);
    }

    void OnHealthChanged(int newHealth) {
        health = newHealth;
        UpdateHealthText();
    }

    void UpdateHealthText() {
        if (isLocalPlayer) {
            healthText.text = "Health: " + health.ToString();
        }
    }
}
