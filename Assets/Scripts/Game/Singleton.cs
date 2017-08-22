using UnityEngine;

/// <summary>
/// Base Singleton class. Any class that inherits this will adopt the Singleton design pattern.
/// </summary>
/// <typeparam name="T">Type of singleton instance</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    protected static T instance;

    // Returns the instance of this singleton
    public static T Instance {
        get {
            if (instance == null) {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null) {
                    Debug.LogError("An instance of " + typeof(T) +
                       " is needed in the scene, but there is none.");
                }
            }

            return instance;
        }
    }
}
