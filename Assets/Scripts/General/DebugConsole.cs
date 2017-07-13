using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugConsole : Singleton<DebugConsole> {

    static Text debugText;

    public void Log(string msg) {
        debugText.text += (" || " + msg);
    }

    void Start() {
        debugText = GameObject.Find("DebugText").GetComponent<Text>();
    }
}
