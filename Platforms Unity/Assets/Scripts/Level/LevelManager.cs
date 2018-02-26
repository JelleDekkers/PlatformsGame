using System;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;
using System.Collections.Generic;
using Serializing;

public class LevelManager : MonoBehaviour {

    private static LevelManager instance;
    public static LevelManager Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<LevelManager>();
            return instance;
        }
    }

    [SerializeField]//, HideInInspector]
    private Level currentLevel;
    public static Level CurrentLevel {
        get { return Instance.currentLevel; }
        private set { Instance.currentLevel = value; }
    }

    public TextAsset levelAsset;

    [SerializeField]
    private bool displayWorldCoordinatesForDebugging;

    private const string FILE_EXTENSION = ".xml";
    private const string FOLDER_PATH = "Assets/Resources/Levels/";

    public void SaveLevelToFile(Level level) {
        string dataPath = FOLDER_PATH + levelAsset.name + FILE_EXTENSION;
        Debug.Log("Trying to save " + dataPath);
        LevelData data = new LevelData(level);
        var serializer = new XmlSerializer(typeof(LevelData));
        var stream = new FileStream(dataPath, FileMode.Create);
        serializer.Serialize(stream, data);
        stream.Close();
        Debug.Log("Succesfully Saved " + levelAsset.name + " to " + dataPath);
    }

    public bool LevelRequirementsHaveBeenMet() {
        return Player.Instance != null && Goal.Instance != null;
    }

    public void LoadLevelFromFile(TextAsset asset) {
        string dataPath = FOLDER_PATH + asset.name + FILE_EXTENSION;
        Debug.Log("Trying to load " + dataPath);
        if (File.Exists(dataPath)) {
            var serializer = new XmlSerializer(typeof(LevelData));
            var stream = new FileStream(dataPath, FileMode.Open);
            LevelData data = serializer.Deserialize(stream) as LevelData;
            stream.Close();

            if(currentLevel != null)
                ClearLevelFromScene();

            currentLevel = new Level();
            BuildLevelInScene(data);
            Debug.Log("Succesfully loaded " + dataPath);

            if (GameEvents.OnLevelLoaded != null)
                GameEvents.OnLevelLoaded.Invoke();
        } else {
            throw new Exception("No file found at " + dataPath);
        }
    }

    public void ClearLevelFromScene() {
        foreach (var i in currentLevel.Tiles) {
            if (i.Value != null) {
                if (i.Value.occupant != null)
                    DestroyImmediate(i.Value.occupant.gameObject);
                DestroyImmediate(i.Value.gameObject);
            }
        }
        currentLevel.Tiles.Clear();

        foreach (var i in currentLevel.Walls) {
            if(i.Value != null)
                DestroyImmediate(i.Value.gameObject);
        }
        currentLevel.Walls.Clear();
        currentLevel = null;
        Debug.Log("Cleared level");
    }

    private void BuildLevelInScene(LevelData level) {
        Dictionary<Tile, TileData> tilesWithEvents = new Dictionary<Tile, TileData>();
        BuildTiles(level, ref tilesWithEvents);
        BuildBlocks(level);
        BuildPortals(level);
        AssignEvents(ref tilesWithEvents);
    }

    private void BuildTiles(LevelData level, ref Dictionary<Tile, TileData> tilesWithEvents) {
        for (int i = 0; i < level.tiles.Length; i++) {
            TileData data = level.tiles[i];
            Type type = Type.GetType(data.type);

            IntVector2 coordinates = new IntVector2(data.x, data.z);
            Vector3 location = new Vector3(coordinates.x + Tile.SIZE.x * 0.5f, 0, coordinates.z + Tile.SIZE.z * 0.5f);
            Tile t = Instantiate(PrefabManager.TilesDataLink.GetPrefabByType(type), location, Quaternion.identity, transform);
            t.name = "Tile " + coordinates + " (" + type.ToString() + ")";
            t.transform.position = location;
            currentLevel.Tiles.Add(new IntVector2(data.x, data.z), t);
            t.Deserialize(data);

            if (t.GetType() == typeof(PressureTile))
                tilesWithEvents.Add(t, data);
        }
    }

    private void BuildBlocks(LevelData level) {
        for (int i = 0; i < level.blocks.Length; i++) {
            BlockData data = level.blocks[i];
            Type type = Type.GetType(data.objectType);
            IntVector2 coordinates = new IntVector2(data.x, data.z);
            Tile tile = currentLevel.Tiles[coordinates];
            Block b = Instantiate(PrefabManager.BlocksDataLink.GetPrefabByType(type),
                                  new Vector3(tile.transform.position.x, Block.POSITION_OFFSET.y, tile.transform.position.z),
                                  Quaternion.Euler(0, data.Roty, 0),
                                  transform);
            b.name = "Block " + coordinates + " (" + type.ToString() + ")";
            tile.SetOccupant(b);
            b.SetTileStandingOn(tile);
        }
    }

    private void BuildPortals(LevelData level) {
        for (int i = 0; i < level.portals.Length; i++) {
            PortalData data = level.portals[i];
            TileEdge edge = new TileEdge(new IntVector2(data.edgeCoordinates.edgeOneX, data.edgeCoordinates.edgeOneZ),
                                         new IntVector2(data.edgeCoordinates.edgeTwoX, data.edgeCoordinates.edgeTwoZ));
            Portal p = Instantiate(PrefabManager.WallsDataLink.GetPrefabByType(typeof(Portal)) as Portal, edge.TileOne.ToVector3() + Tile.POSITION_OFFSET, Quaternion.identity, transform);
            p.name = "Wall " + edge.TileOne + " " + edge.TileTwo + " (" + p.GetType().FullName.ToString() + ")";
            p.SetIsActiveOnStart(data.isActiveOnStart);
            p.transform.eulerAngles = Wall.GetCorrespondingRotation(edge);
            p.SetEdge(edge);
            currentLevel.Walls.Add(edge, p);
        }

        // portal connections:
        for (int i = 0; i < level.portals.Length; i++) {
            PortalData data = level.portals[i];
            if (data.connectedPortalCoordinates != null) {
                TileEdge edge = new TileEdge(new IntVector2(data.edgeCoordinates.edgeOneX, data.edgeCoordinates.edgeOneZ),
                                             new IntVector2(data.edgeCoordinates.edgeTwoX, data.edgeCoordinates.edgeTwoZ));
                TileEdge connectionEdge = new TileEdge(new IntVector2(data.connectedPortalCoordinates.edgeOneX, data.connectedPortalCoordinates.edgeOneZ),
                                                   new IntVector2(data.connectedPortalCoordinates.edgeTwoX, data.connectedPortalCoordinates.edgeTwoZ));
                Portal p = currentLevel.Walls.GetWall(edge.TileOne, edge.TileTwo) as Portal;
                Portal connectedPortal = currentLevel.Walls.GetWall(connectionEdge.TileOne, connectionEdge.TileTwo) as Portal;
                p.SetConnectedPortal(connectedPortal);
            }
        }
    }

    public void AssignEvents(ref Dictionary<Tile, TileData> tilesWithEvents) {
        foreach (KeyValuePair<Tile, TileData> pair in tilesWithEvents)
            pair.Key.DeserializeEvents(pair.Value);
    }

#if UNITY_EDITOR
    private void DisplayWorldCoordinates() {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.gray;
        for (int x = -10; x <= 10; x += 1) {
            for (int z = -10; z <= 10; z += 1) {
                style.normal.textColor = (x == 0 && z == 0) ? Color.white : Color.gray;
                UnityEditor.Handles.Label(new Vector3(x + 0.4f, 0, z + 0.4f), "(" + x + ", " + z + ")", style);
            }
        }
    }

    private void OnDrawGizmos() {
        if(displayWorldCoordinatesForDebugging)
            DisplayWorldCoordinates();
    }
#endif
}
