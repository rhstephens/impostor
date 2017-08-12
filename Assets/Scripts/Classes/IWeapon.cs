/// <summary>
/// Simple weapon interface
/// </summary>
public interface IWeapon {

    int Damage { get; }
    int MaxAmmo { get; }
    float Range { get; }

    // Attempt a shot with this weapon. Must return true if shot successfully, false otherwise.
    bool Shoot();
    void Reload();
}
