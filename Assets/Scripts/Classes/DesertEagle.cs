
public class DesertEagle : IWeapon {

    static float DAMAGE_DEAGLE = 100f;
    static float RANGE_DEAGLE = 10f;

    public float Damage {
        get { return DAMAGE_DEAGLE; }
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
