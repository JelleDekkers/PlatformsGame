using System;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;
using System.Collections.Generic;
using Serializing;

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
        private set { Instance.currentLevel = value; }
    }

    public TextAsset levelAsset;

    [SerializeField]
    private bool displayWorldCoordinatesForDebugging;

    private const string FILE_EXTENSION = ".xml";
    private const string FOLDER_PATH = "Assets/Resources/Levels/";

    public void SaveLevelToFile(Level level) {
        string dataPath = FOLDER_PATH + levelAsset.name + FILE_EXTENSION;
        Debug.Log("Trying to save " + dataPath);
        LevelData data = new LevelData(level);
        var serializer = new XmlSerializer(typeof(LevelData));
        var stream = new FileStream(dataPath, FileMode.Create);
        serializer.Serialize(stream, data);
        stream.Close();
        Debug.Log("Succesfully Saved " + levelAsset.name + " to " + dataPath);
    }

    public bool LevelRequirementsHaveBeenMet() {
        return Player.Instance != null && Goal.Instance != null;
    }

    public void LoadLevelFromFile(TextAsset asset) {
        string dataPath = FOLDER_PATH + asset.name + FILE_EXTENSION;
        Debug.Log("Trying to load " + dataPath);
        if (File.Exists(dataPath)) {
            var serializer = new XmlSerializer(typeof(LevelData));
            var stream = new FileStream(dataPath, FileMode.Open);
            LevelData data = serializer.Deserialize(stream) as LevelData;
            stream.Close();

            if(currentLevel != null)
                LevelBuilder.ClearLevelObjectsFromScene(currentLevel);

            currentLevel = new Level();
            LevelBuilder.BuildLevelObjects(currentLevel, data, transform);
            Debug.Log("Succesfully loaded " + dataPath);

            if (GameEvents.OnLevelLoaded != null)
                GameEvents.OnLevelLoaded.Invoke();
        } else {
            throw new Exception("No file found at " + dataPath);
        }
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
