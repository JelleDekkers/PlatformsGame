using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileBuilder : Editor {

    private static TileBuilder instance;
    private LevelManager CurrentLevel { get { return LevelManager.Instance; } }

    private Tile selectedTileMode;
    private bool placingTiles;
    private bool draggingMouse;
    private bool holdingCtrl;
    private GUIStyle guiStyle;

    public const int COLLIDER_LAYER = 8;
    public static LayerMask ColliderLayerMask = 1 << COLLIDER_LAYER;

    [SerializeField] private Tile tile;
    [SerializeField] private Tile goalTile;
    [SerializeField] private Tile pressureTile;
    [SerializeField] private Tile triggerTile;
    [SerializeField] private Tile slideTile;

    private int buttonWidth = 75;

    int tileTypeIndex = 0;
    string[] tileTypes = new string[] { "Regular", "Goal", "Pressure" , "Trigger", "SlideTile" };

    [MenuItem("Editor/Tile Builder")]
    public static void InitGUI() {
        if (instance == null) {
            instance = (TileBuilder)CreateInstance(typeof(TileBuilder));
            instance.Init();
            SceneView.RepaintAll();
            Selection.activeObject = null;
        } else {
            DestroyImmediate(instance);
            SceneView.RepaintAll();
        }
    }

    private void Init() {
        SceneView.onSceneGUIDelegate += RenderSceneGUI;
        guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.white;

        if(LevelManager.Instance == null) {
            Debug.Log("No instance of level found");
            DestroyImmediate(this);
        }
    }

    private void RenderSceneGUI(SceneView sceneview) {
        holdingCtrl = (Event.current.modifiers == EventModifiers.Control);
        
        Handles.BeginGUI();
        if (GUI.Button(new Rect(5, 5, 80, 20), "Build")) {
            Selection.activeObject = null;
            draggingMouse = false;
            placingTiles = !placingTiles;
        }
        Handles.EndGUI();

        if (placingTiles)
            PlaceTilesMode();

        if (Event.current.type == EventType.MouseDown)
            draggingMouse = true;
        else if (Event.current.type == EventType.MouseUp)
            draggingMouse = false;
    }

    private void PlaceTilesMode() {
        Handles.BeginGUI();
        tileTypeIndex = GUI.SelectionGrid(new Rect(90, 5, buttonWidth * tileTypes.Length, 20), tileTypeIndex, tileTypes, tileTypes.Length);
        GUI.Label(new Rect(10, 30, 1000, 20), "Right click to place a tile.", guiStyle);
        GUI.Label(new Rect(10, 50, 1000, 20), "Hold Ctrl and right click to remove.", guiStyle);
        Handles.EndGUI();

        // keep levelBuilder focused:
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));

        IntVector2 coordinates = ConvertToCoordinate(EditorHelper.GetMousePositionInScene());
        DrawTileWire(coordinates.ToVector3(), LevelManager.CurrentLevel.Tiles.GetTile(coordinates) == null);
        
        if (Event.current.isMouse && draggingMouse && Event.current.button == 0) {
            if (!holdingCtrl)
                CreateTile(GetCorrespondingTileType(tileTypeIndex), coordinates);
            else
                RemoveTile(coordinates);
        }   
    }

    private IntVector2 ConvertToCoordinate(Vector3 worldPos) {
        return new IntVector2(worldPos.x.RoundToInt(), worldPos.z.RoundToInt());
    }

    private void CreateTile(Tile tileType, IntVector2 coordinates) {
        if (LevelManager.CurrentLevel.Tiles.GetTile(coordinates) != null)
            return;

        Tile tile = PrefabUtility.InstantiatePrefab(tileType) as Tile;
        tile.SetCoordinates(coordinates);
        tile.name = TileEditor.GetTileTypeName(tile, coordinates);
        LevelManager.CurrentLevel.Tiles.AddTile(tile, coordinates);
        tile.transform.position = new Vector3(coordinates.x + Tile.SIZE.x * 0.5f, 0, coordinates.z + Tile.SIZE.z * 0.5f);
        tile.transform.SetParent(LevelManager.Instance.transform);
    }

    private void RemoveTile(IntVector2 coordinates) {
        if (!LevelManager.CurrentLevel.Tiles.ContainsCoordinates(coordinates) || LevelManager.CurrentLevel.Tiles.GetTile(coordinates) == null)
            return;

        if(LevelManager.CurrentLevel.Tiles.GetTile(coordinates).occupant != null) {
            Debug.LogWarning("Tile " + coordinates + " has occupant of " + LevelManager.CurrentLevel.Tiles.GetTile(coordinates).occupant.name + ", remove occupant first.");
            return;
        }

        Tile tile = LevelManager.CurrentLevel.Tiles.GetTile(coordinates);
        LevelManager.CurrentLevel.Tiles.RemoveTile(coordinates);
        DestroyImmediate(tile.gameObject);
    }


    private Tile GetCorrespondingTileType(int index) {
        switch(index) {
            case 0:
                return tile;
            case 1:
                return goalTile;
            case 2:
                return pressureTile;
            case 3:
                return triggerTile;
            case 4:
                return slideTile;
        }
        return tile;
    }

    private void CleanUp() {
        SceneView.onSceneGUIDelegate -= RenderSceneGUI;
    }

    private void OnDestroy() {
        CleanUp();
    }

    private void DrawTileWire(Vector3 position, bool isValid) {
        Handles.color = (isValid) ? Color.green : Color.red;
        position = new Vector3(position.x + Tile.SIZE.x / 2, position.y, position.z + Tile.SIZE.z / 2);
        Handles.DrawWireCube(position, Tile.SIZE);
        SceneView.RepaintAll();
    }
}


