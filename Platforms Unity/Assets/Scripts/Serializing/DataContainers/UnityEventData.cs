using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Serializing {

    public class UnityEventData {

        [XmlAttribute] public string typeName;
        [XmlAttribute] public string[] typeArgs;
        [XmlAttribute] public string methodName;

        public UnityEventData() { } // default ctor necessary for xml
        public UnityEventData(Object target, string methodName) {
            if (target.GetType() == typeof(Tile)) {
                Tile t = target as Tile;
                Init(target, methodName, t.coordinates.x.ToString(), t.coordinates.z.ToString());
            } else if (target.GetType() == typeof(Tile)) {
                Block b = target as Block;
                Init(target, methodName, b.Coordinates.x.ToString(), b.Coordinates.z.ToString());
            } else if (target.GetType() == typeof(Tile)) {
                Portal p = target as Portal;
                Init(target, methodName, p.Edge.TileOne.x.ToString(), p.Edge.TileOne.z.ToString(), p.Edge.TileTwo.x.ToString(), p.Edge.TileTwo.z.ToString());
            } else {
                UnityEngine.Debug.LogWarning("No corresponding type found for " + target);
            }
        }

        private void Init(Object obj, string methodName, params string[] typeArgs) {
            typeName = obj.GetType().FullName;
            this.methodName = methodName;
            this.typeArgs = typeArgs;
        }

        public static UnityEventData[] GetEventData(UnityEvent e) {
            UnityEventData[] data = new UnityEventData[e.GetPersistentEventCount()];
            for (int i = 0; i < data.Length; i++)
                data[i] = new UnityEventData(e.GetPersistentTarget(i), e.GetPersistentMethodName(i));
            return data;
        }
    }
}