using UnityEngine;

public class DesertEagle : IWeapon {

    static int DAMAGE_DEAGLE = 100;
    static int AMMO_DEAGLE = 10;
    static float RANGE_DEAGLE = 100f;
    static string AUDIO_PATH = "Audio/deagle_shoot";

    int ammo = AMMO_DEAGLE;

    public int Damage {
        get { return DAMAGE_DEAGLE; }
    }

    public int MaxAmmo {
        get { return AMMO_DEAGLE; }
    }

    public float Range 
    {
        get { return RANGE_DEAGLE; }
    }

    public bool Shoot() {
        ammo = Mathf.Clamp(ammo - 1, 0, MaxAmmo);
        if (ammo == 0) {
            return false;
        }

        return true;
    }

    public void Reload() {

    }

    public AudioClip GetGunAudio() {
        return Resources.Load(AUDIO_PATH) as AudioClip;
    }
}
