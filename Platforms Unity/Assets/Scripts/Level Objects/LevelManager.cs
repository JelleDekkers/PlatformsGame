using UnityEngine;
using System.Collections.Generic;
using Serialization;

public class LevelManager : MonoBehaviour {

    private static LevelManager instance;
    public static LevelManager Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<LevelManager>();
            return instance;
        }
    }

    [SerializeField]//, HideInInspector]
    private Level currentLevel;
    public static Level CurrentLevel {
        get { return Instance.currentLevel; }
        set { Instance.currentLevel = value; }
    }

    [SerializeField]
    private TextAsset levelAsset;
    public static TextAsset LevelAsset { get { return Instance.levelAsset; } }

    [SerializeField]
    private LevelBuilder builder;
    public static LevelBuilder Builder { get { return Instance.builder; } }

    [SerializeField]
    private bool displayWorldCoordinatesForDebugging;

    public static bool SaveToFileRequirementsHaveBeenMet() {
        return Player.Instance != null && Goal.Instance != null;
    }

    public void SetNewLevelAsset(TextAsset asset) {
        levelAsset = asset;
        builder.ClearLevel();
    }

    public void SaveCurrentLevelToFile() {
        if (LevelAsset == null) {
            Debug.Log("Can't save, no file found");
            return;
        }

        LevelData data = new LevelData(CurrentLevel, transform);
        LevelSerializer.SaveToFile(data, LevelAsset.name);
    }

    public void ReloadCurrentLevel() {
        LoadLevelFromFile(levelAsset);
    }

    public void LoadLevelFromFile(TextAsset asset) {
        Builder.ClearLevel();
        LevelData data = LevelSerializer.LoadLevelFromFile(asset);
        currentLevel = new Level();
        Builder.BuildLevelObjects(data, ref currentLevel);
    }

#if UNITY_EDITOR
    private void DisplayWorldCoordinates() {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.gray;
        for (int x = -10; x <= 10; x += 1) {
            for (int z = -10; z <= 10; z += 1) {
                style.normal.textColor = (x == 0 && z == 0) ? Color.white : Color.gray;
                UnityEditor.Handles.Label(new Vector3(x + 0.4f, 0, z + 0.4f), "(" + x + ", " + z + ")", style);
            }
        }
    }

    private void OnDrawGizmos() {
        if(displayWorldCoordinatesForDebugging)
            DisplayWorldCoordinates();
    }
#endif
}
