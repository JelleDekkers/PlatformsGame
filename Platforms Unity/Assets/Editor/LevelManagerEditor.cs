using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager), true)]
public class LevelManagerEditor : Editor {

    private LevelManager levelManager;
    private TextAsset levelPrevFrame;

    private void OnEnable() {
        levelManager = (LevelManager)target;
        levelPrevFrame = levelManager.levelAsset;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (levelPrevFrame != levelManager.levelAsset)
            OnLevelAssetChanged();

        if (GUILayout.Button("Create New"))
            LevelCreator.CreateWizard();

        if (levelManager.levelAsset != null) {
            GUI.enabled = levelManager.LevelRequirementsHaveBeenMet();
            string tooltip = (GUI.enabled) ? "" : "A level requires one Player Block and at least one Goal Tile";
            if (GUILayout.Button(new GUIContent("Save", tooltip))) {
                //levelManager.SaveCurrentLevel();
            }
            if (GUILayout.Button(new GUIContent("Save As", tooltip))) {

            }
            GUI.enabled = true;
        }

        levelPrevFrame = levelManager.levelAsset;
    }

    private void OnLevelAssetChanged() {
        levelManager.ClearLevel();
        if(levelManager.levelAsset != null) 
            levelManager.LoadLevel(levelManager.levelAsset);
    }
}

