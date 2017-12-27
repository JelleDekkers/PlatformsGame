using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class LogicObject : MonoBehaviour {

    private const string MENU_PATH = "Editor/LogicObjects/";
    protected const string ICON_PATH = "Logic Objects/";

    protected abstract string IconTextireName { get; }

    [MenuItem(MENU_PATH + "Counter", false, 0)]
    public static void CreateCounter() {
        GameObject g = new GameObject {
            name = "Counter"
        };
        g.AddComponent<Counter>();
    }

    [MenuItem(MENU_PATH + "Timer", false, 1)]
    public static void CreateTimer() {
        GameObject g = new GameObject {
            name = "Timer"
        };
        g.transform.position = Vector3.zero;
        g.AddComponent<Timer>();
    }

    //bridge

    private void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, ICON_PATH + IconTextireName, true);
    }
}
