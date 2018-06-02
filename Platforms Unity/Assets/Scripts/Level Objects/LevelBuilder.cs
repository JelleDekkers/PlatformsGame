using System;
using System.Collections.Generic;
using UnityEngine;
using Serialization;

using Object = UnityEngine.Object;

public class LevelBuilder : MonoBehaviour {

    private Dictionary<Type, GameObject> spawnableObjectsTable;
    public Dictionary<Type, GameObject> SpawnableObjectsTable {
        get {
            if (spawnableObjectsTable == null)
                spawnableObjectsTable = FillSpawnableObjectsTable();
            return spawnableObjectsTable;
        }
    }

    public GameObject[] spawnableObjects;

    private Action OnObjectsInstantiatedDone;

    private Dictionary<Type, GameObject> FillSpawnableObjectsTable() {
        var spawnableObjectsTable = new Dictionary<Type, GameObject>();
        foreach (GameObject gObject in spawnableObjects) {
            ISerializableGameObject serializable = gObject.GetInterface<ISerializableGameObject>();
            spawnableObjectsTable.Add(serializable.GetType(), gObject);
        }
        return spawnableObjectsTable;
    }

    private UnityEngine.Object GetMatchedGameObject(Type type) {
        return SpawnableObjectsTable[type];
    }

    public void BuildLevelObjectsOOP(LevelData data, ref Level level) {
        List<DataContainer> dataContainers = new List<DataContainer>();
        //uiteindelijk via 1 DataContainer array?
        dataContainers.AddRange(data.tiles);
        dataContainers.AddRange(data.blocks);
        dataContainers.AddRange(data.portals);

        Dictionary<ISerializableGameObject, DataContainer> instantiatedObjectsCache = new Dictionary<ISerializableGameObject, DataContainer>();

        for(int i = 0; i < dataContainers.Count; i++) {
            Type parsedType = Type.GetType(dataContainers[i].objectTypeName);
            Object match = GetMatchedGameObject(parsedType);
            if (match != null) {
                GameObject gObject = null;
#if UNITY_EDITOR
                gObject = UnityEditor.PrefabUtility.InstantiatePrefab(match) as GameObject;
#else
                gObject = Instantiate(match) as GameObject;
#endif
                Object obj = null;
                ISerializableGameObject serializableObject = gObject.GetComponentWithInterface<ISerializableGameObject>(out obj);
                serializableObject.Guid = new GUID(dataContainers[i].guid, obj);
                gObject.transform.SetParent(transform);
                instantiatedObjectsCache.Add(serializableObject, dataContainers[i]);
            } else {
                Debug.Log("no corresponding type found for " + parsedType);
            }
        }

        // Second loop in case during deserialization references to other instances are needed
        foreach(var obj in instantiatedObjectsCache)
            obj.Key.Deserialize(obj.Value);
    }

    public void BuildLevelObjects(LevelData data, ref Level level) {
        GUID.ClearTable();
        BuildTiles(data.tiles, ref level);
        BuildBlocks(data.blocks, ref level);
        BuildPortals(data.portals, ref level);
        //AssignEvents(ref tilesWithEvents);

        if (OnObjectsInstantiatedDone != null)
            OnObjectsInstantiatedDone.Invoke();
        
        if (GameEvents.OnLevelLoaded != null)
            GameEvents.OnLevelLoaded.Invoke();
    }


    public void ClearLevel() {
        Transform transform = LevelManager.Instance.transform;
        for (int i = transform.childCount - 1; i >= 0; i--) {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
#else
            GameObject.Destroy(transform.GetChild(i).gameObject);
#endif
        }
        GUID.ClearTable();
        LevelManager.CurrentLevel = null;
    }

    private void BuildTiles(DataContainer[] tilesData, ref Level level) {
        for (int i = 0; i < tilesData.Length; i++) {
            TileData data = tilesData[i] as TileData;
            Type parsedType = Type.GetType(data.objectTypeName);
            Object match = GetMatchedGameObject(parsedType);

            if (match == null) {
                Debug.Log("no corresponding type found for " + parsedType);
                return;
            }

            IntVector2 coordinates = new IntVector2(data.x, data.z);
            Vector3 position = new Vector3(coordinates.x + Tile.SIZE.x * 0.5f, 0, coordinates.z + Tile.SIZE.z * 0.5f);

            Tile tile = null;
            GameObject gObject = null;
#if UNITY_EDITOR
            gObject = UnityEditor.PrefabUtility.InstantiatePrefab(match) as GameObject;
            tile = gObject.GetComponent<Tile>();
            tile.name = tile.GetType().FullName + " " + coordinates;
#else
            g = Instantiate(match) as GameObject;
            tile = g.GetComponent<Tile>();
#endif
            tile.transform.position = position;
            tile.transform.SetParent(transform);
            level.Tiles.Add(new IntVector2(data.x, data.z), tile);
            OnObjectsInstantiatedDone += () => tile.Deserialize(data);
        }
    }

    private void BuildBlocks(DataContainer[] blocks, ref Level level) {
        for (int i = 0; i < blocks.Length; i++) {
            BlockData data = blocks[i] as BlockData;
            Type parsedType = Type.GetType(data.objectTypeName);
            Object match = GetMatchedGameObject(parsedType);

            if (match == null) {
                Debug.Log("no corresponding type found for " + parsedType);
                return;
            }

            IntVector2 coordinates = new IntVector2(data.x, data.z);
            Tile tile = level.Tiles[coordinates];
            Vector3 position = new Vector3(tile.transform.position.x, Block.POSITION_OFFSET.y, tile.transform.position.z);

            Block block = null;
            GameObject gObject = null;
#if UNITY_EDITOR
            gObject = UnityEditor.PrefabUtility.InstantiatePrefab(match) as GameObject;
            block = gObject.GetComponent<Block>();
            block.name = parsedType.FullName + " " + coordinates;
#else
            gObject = Instantiate(match) as GameObject;
            block = gameObject.GetComponent<Block>();
#endif
            block.transform.position = position;
            block.transform.SetParent(transform);
            tile.SetOccupant(block);
            block.SetTileStandingOn(tile);
            OnObjectsInstantiatedDone += () => block.Deserialize(data);
        }
    }

    private void BuildPortals(DataContainer[] portals, ref Level level) {
        for (int i = 0; i < portals.Length; i++) {
            PortalData data = portals[i] as PortalData;
            Type parsedType = Type.GetType(data.objectTypeName);
            Object match = GetMatchedGameObject(parsedType);

            TileEdge edge = new TileEdge(new IntVector2(data.edgeCoordinates.oneX, data.edgeCoordinates.oneZ),
                                         new IntVector2(data.edgeCoordinates.twoX, data.edgeCoordinates.twoZ));
            Vector3 position = edge.TileOne.ToVector3() + Tile.POSITION_OFFSET;

            Portal portal = null;
            GameObject gObject = null;
#if UNITY_EDITOR
            gObject = UnityEditor.PrefabUtility.InstantiatePrefab(match) as GameObject;
            portal = gObject.GetComponent<Portal>();
            portal.name = portal.GetType().FullName + " " + edge.ToString();
#else
            gObject = Instantiate(match) as GameObject;
            portal = gObject.GetComponent<Portal>();
#endif
            portal.transform.position = position;
            portal.transform.SetParent(transform);
            portal.transform.eulerAngles = Wall.GetCorrespondingRotation(edge);
            portal.SetEdge(edge);
            level.Walls.Add(edge, portal);
            OnObjectsInstantiatedDone += () => portal.Deserialize(data);
        }

        // portal connections:
        //for (int i = 0; i < portals.Length; i++) {
        //    PortalData data = portals[i] as PortalData;
        //    if (data.connectedPortalGuid != null) {
        //        TileEdge edge = new TileEdge(new IntVector2(data.edgeCoordinates.oneX, data.edgeCoordinates.oneZ),
        //                                        new IntVector2(data.edgeCoordinates.twoX, data.edgeCoordinates.twoZ));
        //        TileEdge connectionEdge = new TileEdge(new IntVector2(data.connectedPortalGuid.oneX, data.connectedPortalGuid.oneZ),
        //                                            new IntVector2(data.connectedPortalGuid.twoX, data.connectedPortalGuid.twoZ));
        //        Portal p = level.Walls.GetWall(edge.TileOne, edge.TileTwo) as Portal;
        //        Portal connectedPortal = level.Walls.GetWall(connectionEdge.TileOne, connectionEdge.TileTwo) as Portal;
        //        p.SetConnectedPortal(connectedPortal);
        //    }
        //}
    }

    //public void AssignEvents(ref Dictionary<Tile, TileData> tilesWithEvents) {
    //    foreach (KeyValuePair<Tile, TileData> pair in tilesWithEvents)
    //        pair.Key.DeserializeEvents(pair.Value);
    //}
}
