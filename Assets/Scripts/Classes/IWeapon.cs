/// <summary>
/// Simple weapon interface
/// </summary>
public interface IWeapon {

    float Damage { get; }
    float Range { get; }

    void SoundBlast();
    void MuzzleFlash();
}
