using System;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;
using System.Collections.Generic;
using Serializing;
using System.Linq;

public class LevelManager : MonoBehaviour {

    private static LevelManager instance;
    public static LevelManager Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<LevelManager>();
            return instance;
        }
    }

    [SerializeField]
    private Level currentLevel;
    public static Level CurrentLevel {
        get { return Instance.currentLevel; }
        private set { Instance.currentLevel = value; }
    }

    [Header("Debug Options")]
    [SerializeField]
    private bool displayWorldCoordinates;

    public TextAsset levelAsset;

    private const string FILE_EXTENSION = ".xml";
    private const string FOLDER_PATH = "Assets/Resources/Levels/";

    public void CreateNewLevel(string fileName, int chapterIndex, int levelIndex) {
        if (currentLevel != null)
            ClearLevel();

        currentLevel = new Level();
        //SaveLevel(currentLevel, fileName);
        levelAsset = LevelsHandler.AddNewLevel(currentLevel, fileName, chapterIndex, levelIndex);
    }

    public void SaveLevel(Level level) {
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

    public void LoadLevel(TextAsset asset) {
        string dataPath = FOLDER_PATH + "Chapter 1/" + asset.name + FILE_EXTENSION;
        Debug.Log("Trying to load " + dataPath);
        if (File.Exists(dataPath)) {
            var serializer = new XmlSerializer(typeof(LevelData));
            var stream = new FileStream(dataPath, FileMode.Open);
            LevelData data = serializer.Deserialize(stream) as LevelData;
            stream.Close();

            if(currentLevel != null)
                ClearLevel();

            currentLevel = new Level();
            BuildLevel(data);
            Debug.Log("Succesfully loaded " + dataPath);

            if (GameEvents.OnLevelLoaded != null)
                GameEvents.OnLevelLoaded.Invoke();
        } else {
            throw new Exception("No file found at " + dataPath);
        }
    }

    public void ClearLevel() {
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

    private void BuildLevel(LevelData level) {
        // tiles:
        for (int i = 0; i < level.tiles.Length; i++) {
            TileData data = level.tiles[i];
            Type type = Type.GetType(data.objectType);

            IntVector2 coordinates = new IntVector2(data.x, data.z);
            Tile t = Instantiate(PrefabManager.TilesDataLink.GetPrefabByType(type), coordinates.ToVector3(), Quaternion.identity, transform);
            t.name = "Tile " + coordinates; 
            Vector3 location = new Vector3(data.x + Tile.SIZE.x * 0.5f, 0, data.z + Tile.SIZE.z * 0.5f);
            t.transform.position = location;
            currentLevel.Tiles.Add(new IntVector2(data.x, data.z), t);
            t.OnDeserialize(data);
        }

        // blocks:
        for (int i = 0; i < level.blocks.Length; i++) {
            BlockData data = level.blocks[i];
            Type type = Type.GetType(data.objectType);
            IntVector2 coordinates = new IntVector2(data.x, data.z);
            Tile tile = currentLevel.Tiles[coordinates];
            Block b = Instantiate(PrefabManager.BlocksDataLink.GetPrefabByType(type),
                                  new Vector3(tile.transform.position.x, Block.POSITION_OFFSET.y, tile.transform.position.z),
                                  Quaternion.Euler(0, data.Roty, 0),
                                  transform);
            tile.SetOccupant(b);
            b.SetTileStandingOn(tile);
        }

        // portals:
        for (int i = 0; i < level.portals.Length; i++) {
            PortalData data = level.portals[i];
            TileEdge edge = new TileEdge(new IntVector2(data.edgeCoordinates.edgeOneX, data.edgeCoordinates.edgeOneZ),
                                         new IntVector2(data.edgeCoordinates.edgeTwoX, data.edgeCoordinates.edgeTwoZ));
            Portal p = Instantiate(PrefabManager.Portal, edge.TileOne.ToVector3() + Tile.POSITION_OFFSET, Quaternion.identity, transform);
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
        if (displayWorldCoordinates)
            DisplayWorldCoordinates();
    }
#endif
}


/*    private void DrawGridOutline() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Level.Instance.transform.position + (Level.Instance.Size / 2).ToVector3(0), Level.Instance.Size.ToVector3(0));
    }

    private void DrawGridLines() {
        Gizmos.color = Color.grey;
        Vector3 startPos = transform.position + GridWorldPositionOffset.ToVector3(0);
        for (int z = 0; z < Level.Instance.Size.z + 1; z++)
            Gizmos.DrawLine(new Vector3(startPos.x, startPos.y, startPos.z + z),
                            new Vector3(startPos.x + Size.x * Tile.SIZE.x, startPos.y, startPos.z + z));

        for (int x = 0; x < Level.Instance.Size.x + 1; x++)
            Gizmos.DrawLine(new Vector3(startPos.x + x, startPos.y, startPos.z),
                            new Vector3(startPos.x + x, startPos.y, startPos.z + Size.z * Tile.SIZE.z));
    }

    private void DisplayGridCoordinates() {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.gray;
        Vector3 startPos = transform.position + GridWorldPositionOffset.ToVector3(0);
        for (int x = 0; x < Size.x; x++) {
            for (int z = 0; z < Size.z; z++) {
                string coordinate = "(" + (x + GridWorldPositionOffset.x) + ", " + (z + GridWorldPositionOffset.z) + ")";
                UnityEditor.Handles.Label(new Vector3(startPos.x + (x * Tile.SIZE.x) + (Tile.SIZE.x / 2),
                                                      startPos.y,
                                                      startPos.z + (z * Tile.SIZE.z) + (Tile.SIZE.z / 2)), coordinate, style);
            }
        }
    }

    private void DisplayWorldCoordinates() {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.gray;
        for (int x = -100; x <= 100; x+=10) {
            for (int z = -100; z <= 100; z+=10) {
                style.normal.textColor = (x == 0 && z == 0) ? Color.white : Color.gray;
                UnityEditor.Handles.Label(new Vector3(x, 0, z), "(" + x + ", " + z + ")", style);
            }
        }
    }

    private void OnDrawGizmos() {
        if (drawGridOutline)
            DrawGridOutline();
        if (drawGridLines)
            DrawGridLines();
        if (displayGridCoordinates)
            DisplayGridCoordinates();
        if (displayWorldCoordinates)
            DisplayWorldCoordinates();
    }
*/
