using System;
using System.Collections.Generic;
using UnityEngine;
using Serializing;

public static class LevelBuilder  {

    public static void BuildLevelObjects(Level level, LevelData levelData, Transform transform) {
        Dictionary<Tile, TileData> tilesWithEvents = new Dictionary<Tile, TileData>();
        BuildTiles(level, levelData, ref tilesWithEvents, transform);
        BuildBlocks(level, levelData, transform);
        BuildPortals(level, levelData, transform);
        AssignEvents(ref tilesWithEvents);
    }

    private static void BuildTiles(Level level, LevelData levelData, ref Dictionary<Tile, TileData> tilesWithEvents, Transform transform) {
        for (int i = 0; i < levelData.tiles.Length; i++) {
            TileData data = levelData.tiles[i];
            Type type = Type.GetType(data.type);

            IntVector2 coordinates = new IntVector2(data.x, data.z);
            Vector3 position = new Vector3(coordinates.x + Tile.SIZE.x * 0.5f, 0, coordinates.z + Tile.SIZE.z * 0.5f);
#if UNITY_EDITOR
            Tile tile = UnityEditor.PrefabUtility.InstantiatePrefab(PrefabManager.TilesDataLink.GetPrefabByType(type)) as Tile;

            tile.name = tile.GetType().FullName + " " + coordinates;
#else
            Tile tile = Instantiate(PrefabManager.TilesDataLink.GetPrefabByType(type));
#endif
            tile.transform.SetParent(transform);
            tile.transform.position = position;
            level.Tiles.Add(new IntVector2(data.x, data.z), tile);
            tile.Deserialize(data);

            if (tile.GetType() == typeof(PressureTile))
                tilesWithEvents.Add(tile, data);
        }
    }

    private static void BuildBlocks(Level level, LevelData levelData, Transform transform) {
        for (int i = 0; i < levelData.blocks.Length; i++) {
            BlockData data = levelData.blocks[i];
            Type type = Type.GetType(data.objectType);
            IntVector2 coordinates = new IntVector2(data.x, data.z);
            Tile tile = level.Tiles[coordinates];
            Vector3 position = new Vector3(tile.transform.position.x, Block.POSITION_OFFSET.y, tile.transform.position.z);
            Quaternion rotation = Quaternion.Euler(0, data.Roty, 0);
#if UNITY_EDITOR
            Block block = UnityEditor.PrefabUtility.InstantiatePrefab(PrefabManager.BlocksDataLink.GetPrefabByType(type)) as Block;
            block.name = type.FullName + " " + coordinates;
#else
            Block b = GameObject.Instantiate(PrefabManager.BlocksDataLink.GetPrefabByType(type));
#endif
            block.transform.position = position;
            block.transform.rotation = rotation;
            block.transform.SetParent(transform);
            tile.SetOccupant(block);
            block.SetTileStandingOn(tile);
            block.Deserialize(data);
        }
    }

    private static void BuildPortals(Level level, LevelData levelData, Transform transform) {
        for (int i = 0; i < levelData.portals.Length; i++) {
            PortalData data = levelData.portals[i];
            TileEdge edge = new TileEdge(new IntVector2(data.edgeCoordinates.edgeOneX, data.edgeCoordinates.edgeOneZ),
                                         new IntVector2(data.edgeCoordinates.edgeTwoX, data.edgeCoordinates.edgeTwoZ));
            Vector3 position = edge.TileOne.ToVector3() + Tile.POSITION_OFFSET;
#if UNITY_EDITOR
            Portal portal = UnityEditor.PrefabUtility.InstantiatePrefab(PrefabManager.WallsDataLink.GetPrefabByType(typeof(Portal))) as Portal;

            portal.name = portal.GetType().FullName + " " + edge.ToString();
#else
            Portal portal = GameObject.Instantiate(PrefabManager.WallsDataLink.GetPrefabByType(typeof(Portal)) as Portal;

#endif
            portal.transform.position = position;
            portal.transform.SetParent(transform);
            portal.transform.eulerAngles = Wall.GetCorrespondingRotation(edge);
            portal.SetEdge(edge);
            level.Walls.Add(edge, portal);
            portal.Deserialize(data);
        }

        // portal connections:
        for (int i = 0; i < levelData.portals.Length; i++) {
            PortalData data = levelData.portals[i];
            if (data.connectedPortalCoordinates != null) {
                TileEdge edge = new TileEdge(new IntVector2(data.edgeCoordinates.edgeOneX, data.edgeCoordinates.edgeOneZ),
                                             new IntVector2(data.edgeCoordinates.edgeTwoX, data.edgeCoordinates.edgeTwoZ));
                TileEdge connectionEdge = new TileEdge(new IntVector2(data.connectedPortalCoordinates.edgeOneX, data.connectedPortalCoordinates.edgeOneZ),
                                                   new IntVector2(data.connectedPortalCoordinates.edgeTwoX, data.connectedPortalCoordinates.edgeTwoZ));
                Portal p = level.Walls.GetWall(edge.TileOne, edge.TileTwo) as Portal;
                Portal connectedPortal = level.Walls.GetWall(connectionEdge.TileOne, connectionEdge.TileTwo) as Portal;
                p.SetConnectedPortal(connectedPortal);
            }
        }
    }

    public static void AssignEvents(ref Dictionary<Tile, TileData> tilesWithEvents) {
        foreach (KeyValuePair<Tile, TileData> pair in tilesWithEvents)
            pair.Key.DeserializeEvents(pair.Value);
    }

    public static void ClearLevelObjectsFromScene(Level level) {
        foreach (var i in level.Tiles) {
            if (i.Value != null) {
                if (i.Value.occupant != null)
                    GameObject.DestroyImmediate(i.Value.occupant.gameObject);
                GameObject.DestroyImmediate(i.Value.gameObject);
            }
        }
        level.Tiles.Clear();

        foreach (var i in level.Walls) {
            if (i.Value != null)
                GameObject.DestroyImmediate(i.Value.gameObject);
        }
        level.Walls.Clear();
        level = null;
        Debug.Log("Cleared level");
    }
}
