using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Serialization {
    
    [XmlInclude(typeof(PressureTileData))]
    [XmlInclude(typeof(TriggerTileData))]
    public class TileData : DataContainer {

        [XmlElement] public int x;
        [XmlElement] public int z;
        [XmlElement] public bool moveUpAtStart;

        protected TileData() { }
        public TileData(Tile obj) : base(obj) {
            x = obj.coordinates.x;
            z = obj.coordinates.z;
            moveUpAtStart = obj.MoveUpAtStart;
        }
    }

    public class PressureTileData : TileData {
        public UnityEventDataCollection onEnterEventData;
        public UnityEventDataCollection onExitEventData;

        protected PressureTileData() { }
        public PressureTileData(PressureTile t) : base(t) {
            onEnterEventData = new UnityEventDataCollection(t.OnEnterEvent);
            onExitEventData = new UnityEventDataCollection(t.OnExitEvent);
        }
    }

    public class TriggerTileData : TileData {
        public UnityEventDataCollection onEnterEventData;

        protected TriggerTileData() { }
        public TriggerTileData(TriggerTile t) : base(t) {
            onEnterEventData = new UnityEventDataCollection(t.OnEnterEvent);
        }
    }
}