using System;
using System.Xml.Serialization;

namespace Serialization {

    /// <summary>
    /// Base class for all data containers. Contains all minimum necessary information needed for serialization
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(TileData))]
    [XmlInclude(typeof(BlockData))]
    [XmlInclude(typeof(PortalData))]
    [XmlInclude(typeof(LogicObjectData))]
    public abstract class DataContainer {

        [XmlAttribute] public string objectTypeName;
        [XmlAttribute] public int guid;

        protected DataContainer() { }
        public DataContainer(UnityEngine.Object obj) {
            objectTypeName = obj.GetType().FullName;

            ISerializableGameObject serializableObj = obj as ISerializableGameObject;
            if (serializableObj.Guid != null && serializableObj.Guid.ID != 0)
                guid = serializableObj.Guid.ID;
            else
                guid = obj.GetInstanceID();
        }
    }

}