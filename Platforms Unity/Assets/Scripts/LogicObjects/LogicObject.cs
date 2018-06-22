using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serialization;

[System.Serializable]
public abstract class LogicObject : MonoBehaviour, ISerializableGameObject {

    protected const string MENU_PATH = "Level Tools/Logic Objects/";

    private GUID guid;
    public GUID Guid {
        get {
            if (guid == null || guid.ID == 0)
                guid = new GUID(GetInstanceID(), this);
            return guid;
        }
        set {
            guid = value;
        }
    }

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
