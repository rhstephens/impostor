
public class DesertEagle : IWeapon {

    static int DAMAGE_DEAGLE = 100;
    static int AMMO_DEAGLE = 1;
    static float RANGE_DEAGLE = 10f;

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

    public void SoundBlast() {
        //TODO: implement custom sound
    }

    public void MuzzleFlash() {
        //TODO: implement custom muzzle flash
    }
}
