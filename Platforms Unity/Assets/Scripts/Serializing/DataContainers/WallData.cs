using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Serialization {

    public class PortalData : DataContainer {

        [XmlElement] public EdgeData edgeCoordinates;
        [XmlElement] public bool isActiveOnStart;
        [XmlElement] public EdgeData connectedPortalCoordinates;

        private PortalData() { }
        public PortalData(Portal portal) : base(portal) {
            edgeCoordinates = new EdgeData(portal.Edge);
            isActiveOnStart = portal.IsActiveOnStart;
            if (portal.ConnectedPortal != null)
                connectedPortalCoordinates = new EdgeData(portal.ConnectedPortal.Edge);
        }
    }

    public class EdgeData {

        [XmlElement] public int oneX, oneZ, twoX, twoZ;

        private EdgeData() { }
        public EdgeData(TileEdge edge) {
            oneX = edge.TileOne.x;
            oneZ = edge.TileOne.z;
            twoX = edge.TileTwo.x;
            twoZ = edge.TileTwo.z;
        }

        public TileEdge ToTileEdge() {
            return new TileEdge(new IntVector2(oneX, oneZ), new IntVector2(twoX, twoZ));
        }
    }
}