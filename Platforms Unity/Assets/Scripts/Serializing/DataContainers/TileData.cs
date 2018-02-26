using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Serializing {

    [XmlInclude(typeof(PressureTileData))]
    [XmlInclude(typeof(TriggerTileData))]
    [XmlInclude(typeof(GoalTileData))]
    [XmlInclude(typeof(SlideTileData))]
    [XmlInclude(typeof(PlayerOnlyTileData))]
    public class TileData {

        private static readonly Dictionary<Type, Type> DataLinks = new Dictionary<Type, Type>() {
            {typeof(Tile), typeof(TileData) },
            {typeof(Goal), typeof(GoalTileData) },
            {typeof(PressureTile), typeof(PressureTileData) },
            {typeof(TriggerTile), typeof(TriggerTileData) },
            {typeof(SlideTile), typeof(SlideTileData) },
            {typeof(PlayerOnlyTile), typeof(PlayerOnlyTileData) }
        };

        public static Type GetLinkedDataType(Type objectType) {
            return DataLinks[objectType];
        }

        [XmlAttribute] public string type;
        [XmlAttribute] public int x;
        [XmlAttribute] public int z;
        [XmlAttribute] public bool moveUpAtStart;

        protected TileData() { }
        public TileData(Tile t) {
            type = t.GetType().FullName;
            x = t.coordinates.x;
            z = t.coordinates.z;
            moveUpAtStart = t.MoveUpAtStart;
        }
    }

    public class PressureTileData : TileData {
        [XmlArray] public UnityEventData[] onEnterEventData;
        [XmlArray] public UnityEventData[] onExitEventData;

        protected PressureTileData() : base() { }
        public PressureTileData(PressureTile t) : base(t) {
            onEnterEventData = UnityEventData.GetEventData(t.OnEnterEvent);
            onExitEventData = UnityEventData.GetEventData(t.OnExitEvent);
        }
    }

    public class TriggerTileData : TileData {
        protected TriggerTileData() : base() { }
        public TriggerTileData(TriggerTile t) : base(t) { }
    }

    public class GoalTileData : TileData {
        protected GoalTileData() : base() { }
        public GoalTileData(Goal t) : base(t) { }
    }

    public class SlideTileData : TileData {
        protected SlideTileData() : base() { }
        public SlideTileData(SlideTile t) : base(t) { }
    }

    public class PlayerOnlyTileData : TileData {
        protected PlayerOnlyTileData() : base() { }
        public PlayerOnlyTileData(PlayerOnlyTile t) : base(t) { }
    }
}