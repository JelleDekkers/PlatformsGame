using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

public class PortalData {
    public EdgeData edgeCoordinates;
    [XmlAttribute] public bool isActiveOnStart;
    public EdgeData connectedPortalCoordinates;

    private PortalData() { }

    public PortalData(Portal portal) {
        edgeCoordinates = new EdgeData(portal.Edge);
        isActiveOnStart = portal.IsActiveOnStart;
        if (portal.ConnectedPortal != null)
            connectedPortalCoordinates = new EdgeData(portal.ConnectedPortal.Edge);
    }
}

public class EdgeData {
    [XmlAttribute] public int edgeOneX, edgeOneZ, edgeTwoX, edgeTwoZ;

    private EdgeData() { }

    public EdgeData(TileEdge edge) {
        edgeOneX = edge.TileOne.x;
        edgeOneZ = edge.TileOne.z;
        edgeTwoX = edge.TileTwo.x;
        edgeTwoZ = edge.TileTwo.z;
    }
}