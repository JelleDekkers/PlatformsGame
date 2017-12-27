using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Serializing {

    [XmlInclude(typeof(PlayerBlockData))]
    [XmlInclude(typeof(MoveableBlockData))]
    [XmlInclude(typeof(LaserSourceBlockData))]
    [XmlInclude(typeof(LaserDiverterBlockData))]
    [XmlInclude(typeof(LaserRecieverBlockData))]
    public class BlockData {

        private static readonly Dictionary<Type, Type> DataLinks = new Dictionary<Type, Type>() {
            {typeof(Block), typeof(BlockData) },
            {typeof(BlockMoveable), typeof(MoveableBlockData) },
            {typeof(Player), typeof(PlayerBlockData) },
            {typeof(LaserSource), typeof(LaserSourceBlockData) },
            {typeof(LaserDiverter), typeof(LaserDiverterBlockData) },
            {typeof(LaserReciever), typeof(LaserRecieverBlockData) }
        };

        public static Type GetLinkedDataType(Type objectType) {
            return DataLinks[objectType];
        }

        [XmlAttribute] public string objectType;
        [XmlAttribute] public int x;
        [XmlAttribute] public int z;
        [XmlAttribute] public float Roty;

        protected BlockData() { }
        public BlockData(Block t) {
            objectType = t.GetType().FullName;
            x = t.tileStandingOn.coordinates.x;
            z = t.tileStandingOn.coordinates.z;
            Roty = t.transform.eulerAngles.y;
        }
    }

    public class PlayerBlockData : BlockData {
        protected PlayerBlockData() : base() { }
        public PlayerBlockData(Player b) : base(b) { }
    }

    public class MoveableBlockData : BlockData {
        protected MoveableBlockData() : base() { }
        public MoveableBlockData(BlockMoveable b) : base(b) { }
    }

    public class LaserSourceBlockData : BlockData {
        [XmlAttribute] public bool isActiveOnStart;

        protected LaserSourceBlockData() : base() { }
        public LaserSourceBlockData(LaserSource b) : base(b) {
            isActiveOnStart = b.IsActiveOnStart;
        }
    }

    public class LaserDiverterBlockData : BlockData {
        protected LaserDiverterBlockData() : base() { }
        public LaserDiverterBlockData(LaserDiverter b) : base(b) { }
    }

    public class LaserRecieverBlockData : BlockData {
        protected LaserRecieverBlockData() : base() { }
        public LaserRecieverBlockData(LaserReciever b) : base(b) { }
    }
}
