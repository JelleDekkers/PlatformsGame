using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Serialization {

    [XmlInclude(typeof(LaserSourceBlockData))]
    [XmlInclude(typeof(LaserRecieverData))]
    public class BlockData : DataContainer {

        [XmlElement] public int x;
        [XmlElement] public int z;
        [XmlElement] public float yRot;

        protected BlockData() { }
        public BlockData(Block obj) : base(obj) {
            x = obj.Coordinates.x;
            z = obj.Coordinates.z;
            yRot = obj.transform.eulerAngles.y;
        }
    }

    public class LaserSourceBlockData : BlockData {
        [XmlElement] public bool isActiveOnStart;
        [XmlElement] public bool isLethal;

        protected LaserSourceBlockData() : base() { }
        public LaserSourceBlockData(LaserSource b) : base(b) {
            isActiveOnStart = b.IsActiveOnStart;
            isLethal = b.IsLethal;
        }
    }

    public class LaserRecieverData : BlockData {
        [XmlElement] public UnityEventDataCollection onActivated;
        [XmlElement] public UnityEventDataCollection onDisabled;

        protected LaserRecieverData() : base() { }
        public LaserRecieverData(LaserReciever b) : base(b) {
            onActivated = new UnityEventDataCollection(b.onActivated);
            onDisabled = new UnityEventDataCollection(b.onDisabled);
        }
    }
}
