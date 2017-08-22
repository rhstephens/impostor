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
        // Create window debris
        GameObject shards = Instantiate(debris);
        shards.transform.position = transform.position;
        shards.transform.Rotate(0, 0, Random.Range(0, 360));
        
        NetworkServer.Spawn(shards);
        Destroy(gameObject);
    }

    [Command]
    void CmdRegisterAudio() {

    }
}
