using System.Linq;
using UnityEngine;
using UnityEditor;

public class BuildModeWall : BuildMode {

    public override Object[] Prefabs { get; protected set; }
    public override string[] PrefabNames { get; protected set; }

    public BuildModeWall() {
        Prefabs = PrefabManager.Walls.Keys.ToArray();
        PrefabNames = PrefabManager.Walls.Values.ToArray();
    }

    public override bool IsValidPosition(Vector3 position) {
        IntVector2 coordinatesOne = LevelBuilder.ConvertPositionToGridCoordinate(position);
        IntVector2 coordinatesTwo = GetNearestNeighbourCoordinates(coordinatesOne);
        TileEdge edge = new TileEdge(coordinatesOne, coordinatesTwo);
        return !Level.Walls.ContainsWall(edge);
    }

    public override void Update() {
        IntVector2 coordinates = LevelBuilder.ConvertPositionToGridCoordinate(EditorHelper.GetMousePositionInScene());
        DrawHelperGizmos(IsValidPosition(EditorHelper.GetMousePositionInScene()), coordinates.ToVector3() + Block.POSITION_OFFSET, Block.SIZE);
    }

    public override void Build(int index, Vector3 position) {
        IntVector2 coordinatesOne = LevelBuilder.ConvertPositionToGridCoordinate(position);
        IntVector2 coordinatesTwo = GetNearestNeighbourCoordinates(coordinatesOne);
        TileEdge edge = new TileEdge(coordinatesOne, coordinatesTwo);
        if (Level.Walls.ContainsWall(edge))
            return;

        Portal portal = PrefabUtility.InstantiatePrefab(Prefabs[index]) as Portal;

        portal.transform.position = coordinatesOne.ToVector3(0) + Tile.POSITION_OFFSET;
        portal.transform.eulerAngles = Wall.GetCorrespondingRotation(edge);
        portal.SetEdge(edge);
        Level.Walls.AddWall(edge, portal);
        portal.transform.SetParent(LevelManager.Instance.transform);

        Undo.RegisterCreatedObjectUndo(portal.gameObject, "Created: " + portal.name);
        EditorUtility.SetDirty(portal);
        EditorUtility.SetDirty(LevelManager.Instance.gameObject);
        SceneView.RepaintAll();
    }

    public override void Remove(Vector3 position) {
        IntVector2 coordinatesOne = LevelBuilder.ConvertPositionToGridCoordinate(position);
        IntVector2 coordinatesTwo = GetNearestNeighbourCoordinates(coordinatesOne);
        TileEdge edge = new TileEdge(coordinatesOne, coordinatesTwo);
        if (!Level.Walls.ContainsWall(edge))
            return;

        Portal p = Level.Walls.GetWall(edge) as Portal;
        Level.Walls.RemoveWall(edge);
        Object.DestroyImmediate(p.gameObject);
        EditorUtility.SetDirty(LevelManager.Instance.gameObject);
        SceneView.RepaintAll();
    }

    private IntVector2 GetNearestNeighbourCoordinates(IntVector2 coordinates) {
        Vector3 dif = EditorHelper.GetMousePositionInScene() - coordinates.ToVector3(0);
        dif.x -= 0.5f;
        dif.z -= 0.5f;

        IntVector2 mouseCoordinates = EditorHelper.GetMousePositionInScene().Rounded().ToIntVector2();
        if (Mathf.Abs(dif.x) > Mathf.Abs(dif.z))
            return mouseCoordinates + new IntVector2(RoundAwayFromZero(dif.x), 0);
        else
            return mouseCoordinates + new IntVector2(0, RoundAwayFromZero(dif.z));
    }

    private int RoundAwayFromZero(float f) {
        if (f >= 0)
            return 1;
        else
            return -1;
    }

    public override void DrawHelperGizmos(bool isValidPosition, Vector3 position, Vector3 size) {
        IntVector2 coordinatesOne = LevelBuilder.ConvertPositionToGridCoordinate(position);
        IntVector2 coordinatesTwo = GetNearestNeighbourCoordinates(coordinatesOne);

        position = coordinatesTwo.ToVector3() - coordinatesOne.ToVector3();
        //Vector3 wireFrameSize = new Vector3(0.3f, 1.4f, 1f);

        base.DrawHelperGizmos(isValidPosition, coordinatesOne.ToVector3() + Block.POSITION_OFFSET, Block.SIZE);
        base.DrawHelperGizmos(isValidPosition, coordinatesTwo.ToVector3() + Block.POSITION_OFFSET, Block.SIZE);
    }
}
