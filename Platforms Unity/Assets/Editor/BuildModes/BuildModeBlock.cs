using System.Linq;
using UnityEngine;
using UnityEditor;

public class BuildModeBlock : BuildMode {

    public override Object[] Prefabs { get; protected set; }
    public override string[] PrefabNames { get; protected set; }

    public BuildModeBlock() {
        Prefabs = PrefabManager.Blocks.Keys.ToArray();
        PrefabNames = PrefabManager.Blocks.Values.ToArray();
    }

    public override bool IsValidPosition(Vector3 position) {
        IntVector2 coordinates = EditorLevelBuilder.ConvertPositionToGridCoordinate(position);
        return Level.Tiles.GetTile(coordinates) != null && Level.Tiles.GetTile(coordinates).occupant == null;
    }

    public override void Update() {
        IntVector2 coordinates = EditorLevelBuilder.ConvertPositionToGridCoordinate(EditorHelper.GetMousePositionInScene());
        DrawHelperGizmos(IsValidPosition(EditorHelper.GetMousePositionInScene()), coordinates.ToVector3() + Block.POSITION_OFFSET, Block.SIZE);
    }

    public override void Build(int index, Vector3 position) {
        IntVector2 coordinates = EditorLevelBuilder.ConvertPositionToGridCoordinate(position);
        Tile tileStandingOn = Level.Tiles.GetTile(coordinates);
        if (tileStandingOn == null || tileStandingOn.occupant != null)
            return;

        Block block = PrefabUtility.InstantiatePrefab(Prefabs[index]) as Block;
        block.SetTileStandingOn(tileStandingOn);
        tileStandingOn.SetOccupant(block);
        block.name = block.GetType().FullName + coordinates;
        block.transform.position = coordinates.ToVector3() + Block.POSITION_OFFSET;
        block.transform.SetParent(LevelManager.Instance.transform);
        block.transform.eulerAngles = GetCorrespondingRotation();

        Undo.RegisterCreatedObjectUndo(block.gameObject, "Created: " + block.name);
        EditorUtility.SetDirty(block);
        EditorUtility.SetDirty(LevelManager.Instance.gameObject);
        SceneView.RepaintAll();
    }

    public override void Remove(Vector3 position) {
        IntVector2 coordinates = EditorLevelBuilder.ConvertPositionToGridCoordinate(position);
        Tile tileStandingOn = Level.Tiles.GetTile(coordinates);
        if (tileStandingOn == null || tileStandingOn.occupant == null)
            return;

        Block block = tileStandingOn.occupant;
        tileStandingOn.SetOccupant(null);
        Object.DestroyImmediate(block.gameObject);

        EditorUtility.SetDirty(tileStandingOn);
        EditorUtility.SetDirty(LevelManager.Instance.gameObject);
        SceneView.RepaintAll();
    }

    private Vector3 GetCorrespondingRotation() {
        IntVector2 nearestCoordinates = EditorLevelBuilder.ConvertPositionToGridCoordinate(EditorHelper.GetMousePositionInScene());
        Vector3 difference = nearestCoordinates.ToVector3() - EditorHelper.GetMousePositionInScene();

        difference.x = Mathf.Abs(difference.x);
        difference.x -= 0.5f;
        difference.z = Mathf.Abs(difference.z);
        difference.z -= 0.5f;

        IntVector2 nearestEdge = GetNearestEdge(difference);
        if (nearestEdge.x == 0 && nearestEdge.z == 1)
            return new Vector3(0, 0, 0);
        else if (nearestEdge.x == 0 && nearestEdge.z == -1)
            return new Vector3(0, 180, 0);
        else if (nearestEdge.x == 1 && nearestEdge.z == 0)
            return new Vector3(0, 90, 0);
        else
            return new Vector3(0, 270, 0);
    }

    private IntVector2 GetNearestEdge(Vector3 pos) {
        if (Mathf.Abs(pos.x) > Mathf.Abs(pos.z))
            return new IntVector2(RoundAwayFromZero(pos.x), 0);
        else
            return new IntVector2(0, RoundAwayFromZero(pos.z));
    }

    private int RoundAwayFromZero(float f) {
        if (f >= 0)
            return 1;
        else
            return -1;
    }
}
