using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple log console that displays on ALL clients, not just the host 
/// </summary>
public class DebugConsole : Singleton<DebugConsole> {

    static Text debugText;

    public void Log(string msg) {
        debugText.text += (" || " + msg);
    }

    void Start() {
        debugText = GameObject.Find("DebugText").GetComponent<Text>();
    }
}
