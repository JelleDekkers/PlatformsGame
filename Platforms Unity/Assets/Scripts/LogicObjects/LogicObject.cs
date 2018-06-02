using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Serialization;

[System.Serializable]
public abstract class LogicObject : MonoBehaviour, ISerializableGameObject {

    protected const string MENU_PATH = "Level Tools/Logic Objects/";

    public GUID Guid { get; set; }

    #region Serialization
    public virtual DataContainer Serialize() {
        return new LogicObjectData(this);
    }

    public virtual object Deserialize(DataContainer data) {
        LogicObjectData parsedData = data as LogicObjectData;
        return parsedData;
    }
    #endregion

}
