using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;
public class GUIDEBUG : MonoBehaviour {

    private void OnGUI() {
        GUI.Label(new Rect(10, Screen.height - 20, 1000, 20), "Version: " + Application.version);
    }
}
