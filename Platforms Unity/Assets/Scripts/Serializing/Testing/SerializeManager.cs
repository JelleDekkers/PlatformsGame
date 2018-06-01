using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot]
public class SerializeManager : MonoBehaviour {

    private const string FILE_EXTENSION = ".xml";
    private const string FOLDER_PATH = "Assets/Resources/";
    private const string FILE_NAME = "Test";

    public GameObject[] spawnableObjects;

    public IntGameObjectDictionary guidGameObjectTable = new IntGameObjectDictionary();

    private void Start() {
        CreateFile(GetAllSerializableObjects());
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
            LoadLevelFromFile();
    }

    private LevelTestData GetAllSerializableObjects() {
        List<TestDataContainer> serializables = new List<TestDataContainer>();
        foreach (GameObject g in FindObjectsOfType<GameObject>()) {
            ITestSerializable s = g.GetInterface<ITestSerializable>();
            if (s != null)
                serializables.Add(s.Serialize());
        }

        return new LevelTestData(serializables.ToArray());
    }

    public void CreateFile(LevelTestData data) {
        try {
            string dataPath = FOLDER_PATH + FILE_NAME + FILE_EXTENSION;
            var serializer = new XmlSerializer(typeof(LevelTestData));
            var stream = new FileStream(dataPath, FileMode.Create);
            serializer.Serialize(stream, data);
            stream.Close();
            Debug.Log("<color=green>Succesfully Saved </color>" + FILE_NAME + " to " + dataPath);
        } catch (Exception exception) {
            Debug.LogError("<color=red>Saving Failed: </color>" + exception);
        }
    }

    public void LoadLevelFromFile() {
        try {
            string dataPath = FOLDER_PATH + FILE_NAME + FILE_EXTENSION;
            if (File.Exists(dataPath)) {
                var serializer = new XmlSerializer(typeof(LevelTestData));
                var stream = new FileStream(dataPath, FileMode.Open);
                LevelTestData data = serializer.Deserialize(stream) as LevelTestData;
                stream.Close();
                BuildLevel(data);
            } else {
                throw new Exception("<color=red>No file found at </color>" + dataPath);
            }
        } catch (Exception exception) {
            throw new Exception("<color=red>Failed loading level: </color>" + exception);
        }
    }

    private void BuildLevel(LevelTestData data) {
        foreach (TestDataContainer container in data.serializables) {
            Type parsedType = Type.GetType(container.objectTypeName);
            GameObject match = GetGameObjectMatch(parsedType);
            if (match != null) {
                GameObject gObject = Instantiate(GetGameObjectMatch(parsedType));
                ITestSerializable serializableObject = gObject.GetInterface<ITestSerializable>();
                serializableObject.Deserialize(container);
                guidGameObjectTable.Add(serializableObject.Guid.ID, gObject);
            } else {
                Debug.Log("no corresponding type found for " + parsedType);
            }
        }
    }

    private GameObject GetGameObjectMatch(Type type) {
        for (int i = 0; i < spawnableObjects.Length; i++) {
            if (spawnableObjects[i].GetComponent(type))
                return spawnableObjects[i];
        }
        return null;
    }
}

[System.Serializable]
public class LevelTestData {

    [XmlArray("Data")]
    public TestDataContainer[] serializables;

    public LevelTestData() { }
    public LevelTestData(TestDataContainer[] serializables) {
        this.serializables = serializables;
    }
}