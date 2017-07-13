using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// A singleton class to govern Network related tasks. Unfortunately, unity doesn't support generic typed classes
///     for NetworkBehaviour so I could not abstract out the singleton pattern.
/// </summary>
public class NetworkManager : NetworkBehaviour {

    protected static NetworkManager instance;

    // Returns the instance of this singleton
    public static NetworkManager Instance {
        get {
            if (instance == null) {
                instance = (NetworkManager)FindObjectOfType(typeof(NetworkManager));

                if (instance == null) {
                    Debug.LogError("An instance of " + typeof(NetworkManager) +
                       " is needed in the scene, but there is none.");
                }
            }

            return instance;
        }
    }
}
