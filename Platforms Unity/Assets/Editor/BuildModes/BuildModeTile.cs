using UnityEngine;
using UnityEditor;

public class BuildModeTile : BuildMode {

    public override Object[] Prefabs { get { return PrefabManager.Tiles; } }
    public override string[] PrefabNames { get; protected set; }

    public BuildModeTile() { 
        PrefabNames = GetObjectNames(Prefabs);
    }

    public override bool IsValidPosition(Vector3 position) {
        IntVector2 coordinates = LevelBuilder.ConvertPositionToGridCoordinate(position);
        return Level.Tiles.GetTile(coordinates) == null;
    }

    public override void Update() {
        IntVector2 coordinates = LevelBuilder.ConvertPositionToGridCoordinate(EditorHelper.GetMousePositionInScene());
        DrawHelperGizmos(IsValidPosition(EditorHelper.GetMousePositionInScene()), coordinates.ToVector3(), Tile.SIZE);
    }

    public override void Build(int index, Vector3 position) {
        IntVector2 coordinates = LevelBuilder.ConvertPositionToGridCoordinate(position);
        if (Level.Tiles.GetTile(coordinates) != null)
            return;

        Tile tile = PrefabUtility.InstantiatePrefab(Prefabs[index]) as Tile;
        tile.SetCoordinates(coordinates);
        tile.name = TileEditor.GetTileTypeName(tile, coordinates);
        Level.Tiles.AddTile(tile, coordinates);
        tile.transform.position = new Vector3(coordinates.x + Tile.SIZE.x * 0.5f, 0, coordinates.z + Tile.SIZE.z * 0.5f);
        tile.transform.SetParent(LevelManager.Instance.transform);

        Undo.RegisterCreatedObjectUndo(tile.gameObject, "Created: " + tile.name);
        EditorUtility.SetDirty(tile);
        EditorUtility.SetDirty(LevelManager.Instance.gameObject);
        SceneView.RepaintAll();
    }

    public override void Remove(Vector3 position) {
        IntVector2 coordinates = LevelBuilder.ConvertPositionToGridCoordinate(position);
        if (!Level.Tiles.ContainsCoordinates(coordinates) || Level.Tiles.GetTile(coordinates) == null)
            return;

        if (Level.Tiles.GetTile(coordinates).occupant != null) {
            Debug.LogWarning("Tile " + coordinates + " has occupant of " + Level.Tiles.GetTile(coordinates).occupant.name + ", remove current occupant first.");
            return;
        }

        Tile tile = Level.Tiles.GetTile(coordinates);
        Level.Tiles.RemoveTile(coordinates);
        EditorUtility.SetDirty(LevelManager.Instance.gameObject);
        Object.DestroyImmediate(tile.gameObject);
    }

    public override void DrawHelperGizmos(bool isValidPosition, Vector3 position, Vector3 size) {
        position = new Vector3(position.x + Tile.SIZE.x / 2, position.y, position.z + Tile.SIZE.z / 2);
        base.DrawHelperGizmos(isValidPosition, position, size);
    }
}
