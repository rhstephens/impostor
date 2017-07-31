using UnityEngine;
using UnityEngine.Networking;

public class Window : BaseDestructible {

    public GameObject debris;

	public override void OnHit(int damage) {
        if (!isServer) return;
        Debug.Log("Breaking window on server");
        RpcBreakWindow();
    }

    [ClientRpc]
    void RpcBreakWindow() {
        GameObject shards = Instantiate(debris);
        shards.transform.position = transform.position;
        NetworkServer.Spawn(shards);
        Destroy(gameObject);
    }
}
