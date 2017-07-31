using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Contract for any destructble object
/// </summary>
public abstract class BaseDestructible : NetworkBehaviour {


    public abstract void OnHit(int damage);
}
