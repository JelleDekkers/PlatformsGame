using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Serialization {

    [XmlInclude(typeof(LaserSourceBlockData))]
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
}
