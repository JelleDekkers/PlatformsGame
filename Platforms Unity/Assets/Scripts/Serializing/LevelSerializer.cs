using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace Serialization {

    public static class LevelSerializer {

        private const string FILE_EXTENSION = ".xml";
        private const string FOLDER_PATH = "Assets/Resources/Levels/";

        public static TextAsset SaveToFile(LevelData data, string fileName) {
            GUID.ClearTable();
            try {
                string dataPath = FOLDER_PATH + fileName + FILE_EXTENSION;
                var serializer = new XmlSerializer(typeof(LevelData));
                var encoding = Encoding.GetEncoding("UTF-8");
                var stream = new StreamWriter(dataPath, false, encoding);
                serializer.Serialize(stream, data);
                stream.Close();
                Debug.Log("<color=green>Succesfully Saved </color>" + fileName + " to " + dataPath);
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
                string resourcesPath = "Levels/" + fileName;
                TextAsset asset = Resources.Load(resourcesPath) as TextAsset;
                return asset;
            } catch (Exception exception) {
                Debug.LogError("<color=red>Saving Failed: </color>" + exception);
                return null;
            }
        }

        public static LevelData LoadLevelFromFile(TextAsset asset) {
            try {
                string dataPath = FOLDER_PATH + asset.name + FILE_EXTENSION;
                if (File.Exists(dataPath)) {
                    var serializer = new XmlSerializer(typeof(LevelData));
                    var stream = new FileStream(dataPath, FileMode.Open);
                    LevelData data = serializer.Deserialize(stream) as LevelData;
                    stream.Close();
                    return data;
                } else {
                    throw new Exception("<color=red>No file found at </color>" + dataPath);
                }
            } catch (Exception exception) {
                throw new Exception("<color=red>Failed loading level: </color>" + exception);
            }
        }
    }
}