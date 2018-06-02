using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Serialization {

    [XmlRoot("level")]
    public class LevelData {

        [XmlAttribute("date")] public string time;
        [XmlArray] public DataContainer[] data;

        private LevelData() { }
        public LevelData(Level level, Transform transform) {
            time = DateTime.Today.ToString("dd/M/yy") + " " + DateTime.Now.ToString("HH:mm:ss");
            data = GetAllSerializableLevelObjects(transform).ToArray();
        }

        private List<DataContainer> GetAllSerializableLevelObjects(Transform transform) {
            List<DataContainer> dataContainers = new List<DataContainer>();

            for (int i = 0; i < transform.childCount; i++) {
                ISerializableGameObject serializable = transform.GetChild(i).gameObject.GetInterface<ISerializableGameObject>();
                if(serializable != null) 
                    dataContainers.Add(serializable.Serialize());
            }

            return dataContainers;
        }

        //private DataContainer[] GetAllTileData(Level level, ref List<Block> cachedBlocks) {
        //    List<DataContainer> serializables = new List<DataContainer>();
        //    foreach (var pair in level.Tiles) {
        //        serializables.Add(pair.Value.Serialize());
        //        if (pair.Value.occupant != null)
        //            cachedBlocks.Add(pair.Value.occupant);
        //    }
        //    return serializables.ToArray();
        //}

        //private DataContainer[] GetAllPortalData(Level level) {
        //    List<DataContainer> serializables = new List<DataContainer>();
        //    foreach (var pair in level.Walls)
        //        serializables.Add((pair.Value as Portal).Serialize()); 
        //    return serializables.ToArray();
        //}

        //private DataContainer[] GetAllBlockData(List<Block> blocks) {
        //    List<DataContainer> serializables = new List<DataContainer>();
        //    foreach (var block in blocks)
        //        serializables.Add(block.Serialize());
        //    return serializables.ToArray();
        //}
    }
}