using UnityEngine;
using System;
using System.Collections.Generic;
using Serializing;
using System.Xml.Serialization;
using System.IO;
using Malee;

[CreateAssetMenu(fileName = "LevelsHandler", menuName = "Tools/Levels Handler", order = 1)]
public class LevelsHandler : ScriptableObjectSingleton<LevelsHandler> {

    private const string FILE_EXTENSION = ".xml";
    private const string FOLDER_PATH = "Assets/Resources/Levels/";

    public static TextAsset AddNewLevel(Level level, string fileName) {
        string dataPath = FOLDER_PATH + fileName + FILE_EXTENSION; 
        Debug.Log("Trying to save " + dataPath);
        LevelData data = new LevelData(level);
        var serializer = new XmlSerializer(typeof(LevelData));
        var stream = new FileStream(dataPath, FileMode.Create);
        serializer.Serialize(stream, data);
        stream.Close();
        Debug.Log("Succesfully added new level: " + fileName + " to " + dataPath);
        UnityEditor.AssetDatabase.Refresh();
        string resourcesPath = "Levels/" + fileName;
        TextAsset asset = Resources.Load(resourcesPath) as TextAsset;
        Debug.Log(asset);
        return asset;
    }

    #region chapters
    [Reorderable]
    public Chapters chapters;

    [System.Serializable]
    public class Chapter {

        public Color backgroundColor;

        [Reorderable(singleLine = true)]
        public LevelDrawer levels;
    }

    [System.Serializable]
    public class LevelCustomDrawer {

        //public bool myBool;
        public TextAsset level;
    }

    [System.Serializable]
    public class LevelDrawer : ReorderableArray<LevelCustomDrawer> { }

    [System.Serializable]
    public class Chapters : ReorderableArray<Chapter> { }
    #endregion
}