using System;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

[Serializable]
public class GUID {

    [SerializeField]
    private int id;
    public int ID {
        get { return id; } 
        private set {id = value; }
    }

    public GUID(int id, Object obj) {
        ID = id;
        AddObjectToTable(id, obj);
    }
    
    public override string ToString() {
        return ID.ToString();
    }

    private static Dictionary<int, Object> ObjectsTable = new Dictionary<int, Object>();

    public static void PrintTable() {
        foreach(var v in ObjectsTable) {
            Debug.Log(v.Key + " " + v.Value);
        }
    }

    public static Object GetObjectByGUID(int guid) {
        return ObjectsTable[guid];
    }

    public static void AddObjectToTable(int guid, Object obj) {
        if (ObjectsTable == null)
            ObjectsTable = new Dictionary<int, Object>();

        ObjectsTable.Add(guid, obj);
    }

    public static void ClearTable() {
        ObjectsTable.Clear();
    }
}
