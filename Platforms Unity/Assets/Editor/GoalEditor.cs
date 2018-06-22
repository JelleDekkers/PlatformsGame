using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Goal))]
public class GoalEditor : Editor {
    Goal obj;

    private void OnEnable() {
        obj = (Goal)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Finish Level")) {
            obj.GoalReached();
        }
        GUI.enabled = true;
    }
}