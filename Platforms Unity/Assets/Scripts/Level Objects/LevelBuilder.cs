using System;
using System.Collections.Generic;
using UnityEngine;
using Serialization;

using Object = UnityEngine.Object;

public class LevelBuilder : MonoBehaviour {

    private Dictionary<Type, GameObject> spawnableObjectsTable;
    public Dictionary<Type, GameObject> SpawnableObjectsTable {
        get {
            if (spawnableObjectsTable == null)
                spawnableObjectsTable = FillSpawnableObjectsTable();
            return spawnableObjectsTable;
        }
    }

    public GameObject[] spawnableObjects;

    private Dictionary<Type, GameObject> FillSpawnableObjectsTable() {
        var spawnableObjectsTable = new Dictionary<Type, GameObject>();
        foreach (GameObject gObject in spawnableObjects) {
            ISerializableGameObject serializable = gObject.GetInterface<ISerializableGameObject>();
            spawnableObjectsTable.Add(serializable.GetType(), gObject);
        }
        return spawnableObjectsTable;
    }

    private Object GetMatchedGameObject(Type type) {
        return SpawnableObjectsTable[type];
    }

    public void BuildLevelObjects(LevelData levelData, ref Level level) {
        Dictionary<ISerializableGameObject, DataContainer> instantiatedObjectsCache = new Dictionary<ISerializableGameObject, DataContainer>();
        for (int i = 0; i < levelData.data.Length; i++) {
            Type parsedType = Type.GetType(levelData.data[i].objectTypeName);
            Object match = GetMatchedGameObject(parsedType);
            if (match != null) {
                GameObject gObject = null;
#if UNITY_EDITOR
                gObject = UnityEditor.PrefabUtility.InstantiatePrefab(match) as GameObject;
#else
                gObject = Instantiate(match) as GameObject;
#endif
                Object obj = null;
                ISerializableGameObject serializableObject = gObject.GetComponentWithInterface<ISerializableGameObject>(out obj);
                serializableObject.Guid = new GUID(levelData.data[i].guid, obj);
                gObject.transform.SetParent(transform);
                instantiatedObjectsCache.Add(serializableObject, levelData.data[i]);
            } else {
                Debug.Log("no corresponding type found for " + parsedType);
            }
        }

        // Second loop in case during deserialization references to other instances are needed
        foreach(var obj in instantiatedObjectsCache)
            obj.Key.Deserialize(obj.Value);
    }

    public void ClearLevel() {
        Transform transform = LevelManager.Instance.transform;
        for (int i = transform.childCount - 1; i >= 0; i--) {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
#else
            GameObject.Destroy(transform.GetChild(i).gameObject);
#endif
        }
        GUID.ClearTable();
        LevelManager.CurrentLevel = null;
    }
}
