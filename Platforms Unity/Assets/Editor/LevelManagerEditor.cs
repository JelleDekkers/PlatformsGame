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

        GUI.enabled = levelManager.LevelRequirementsHaveBeenMet();
        string tooltip = (GUI.enabled) ? "" : "A level requires one Player Block and at least one Goal Tile";
        if (LevelManager.Instance.levelAsset != null) {
            if (GUILayout.Button(new GUIContent("Save", tooltip))) {
                levelManager.SaveLevelToFile(LevelManager.CurrentLevel);
            }
        }
        if (GUILayout.Button(new GUIContent("Save As", tooltip))) 
            LevelCreator.CreateWizard();
        if (GUILayout.Button(new GUIContent("Reload")))
            Reload();

        GUI.enabled = true;

        levelPrevFrame = levelManager.levelAsset;
    }

    private void OnLevelAssetChanged() {
        levelManager.ClearLevelFromScene();
        if(levelManager.levelAsset != null) 
            levelManager.LoadLevelFromFile(levelManager.levelAsset);
    }

    private void Reload() {
        levelManager.ClearLevelFromScene();
        levelManager.LoadLevelFromFile(levelManager.levelAsset);
    }
}

