using UnityEngine;
using System;
using System.Collections.Generic;
using Serializing;
using System.Xml.Serialization;
using System.IO;
using SubjectNerd.Utilities;

[CreateAssetMenu(fileName = "LevelsHandler", menuName = "Tools/Levels Handler", order = 3)]
public class LevelsHandler : ScriptableObjectSingleton<LevelsHandler> {

    public Chapter[] Chapters;

    //[SerializeField]
    //private List<Chapter> chapters;
    //public List<Chapter> Chapters { get { return Instance.chapters; } }

    private const string FILE_EXTENSION = ".xml";
    private const string FOLDER_PATH = "Assets/Resources/Levels/";

    public static TextAsset AddNewLevel(Level level, string fileName, int chapterIndex, int levelIndex) {
        string dataPath = FOLDER_PATH + chapterIndex + "/" + fileName + FILE_EXTENSION; // folder exists, otherwise create new subfolder
        Debug.Log("Trying to save " + dataPath);
        LevelData data = new LevelData(level);
        var serializer = new XmlSerializer(typeof(LevelData));
        var stream = new FileStream(dataPath, FileMode.Create);
        serializer.Serialize(stream, data);
        stream.Close();
        Debug.Log("Succesfully added new level " + fileName + " to " + dataPath);

        // add to Chapters

        throw new NotImplementedException();
    }

    public static bool GetLevelByIndex(out TextAsset asset, int index) {
        throw new NotImplementedException();
    }
}

[Serializable]
public class Chapter {

    public Color color;
    [Reorderable]
    public TextAsset[] levels;

    public int LevelCount { get { return levels.Length; } }
}