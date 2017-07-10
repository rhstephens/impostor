/// <summary>
/// Simple weapon interface
/// </summary>
public interface IWeapon {

    int Damage { get; }
    int MaxAmmo { get; }
    float Range { get; }

    void SoundBlast();
    void MuzzleFlash();
}
