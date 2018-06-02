using UnityEngine;
using UnityEditor;
using Serialization;

[CustomEditor(typeof(LevelManager), true)]
public class LevelManagerEditor : Editor {

    private LevelManager levelManager;
    private TextAsset levelAssetPrevFrame;
    private string tooltip;

    private void OnEnable() {
        levelManager = (LevelManager)target;
        levelAssetPrevFrame = LevelManager.LevelAsset;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (levelAssetPrevFrame != LevelManager.LevelAsset)
            OnLevelAssetChanged();

        GUI.enabled = LevelManager.SaveToFileRequirementsHaveBeenMet() && LevelManager.LevelAsset != null;
        tooltip = (GUI.enabled) ? "" : "A level requires one Player Block and at least one Goal Tile and levelAsset musnt't be null";
        if (GUILayout.Button(new GUIContent("Save", tooltip)))
            levelManager.SaveCurrentLevelToFile();

        GUI.enabled = LevelManager.SaveToFileRequirementsHaveBeenMet();
        if (GUILayout.Button(new GUIContent("Save As", tooltip)))
            LevelCreator.CreateWizard();

        GUI.enabled = LevelManager.LevelAsset != null;
        if (GUILayout.Button(new GUIContent("Reload Level")))
            levelManager.ReloadCurrentLevel();

        GUI.enabled = LevelManager.CurrentLevel != null;
        if (GUILayout.Button(new GUIContent("Empty Level")))
            LevelManager.Builder.ClearLevel();

        GUI.enabled = true;

        levelAssetPrevFrame = LevelManager.LevelAsset;
    }

    private void OnLevelAssetChanged() {
        if (LevelManager.LevelAsset.GetType() != typeof(TextAsset)) {
            Debug.LogWarning(LevelManager.LevelAsset.GetType() + " is not a TextAsset!");
            levelManager.SetNewLevelAsset(null);
            return;
        }

        if (LevelManager.LevelAsset != null) 
            levelManager.ReloadCurrentLevel();
    }
}

