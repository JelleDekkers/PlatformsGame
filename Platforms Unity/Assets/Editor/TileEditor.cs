using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile), true), CanEditMultipleObjects]
public class TileEditor : DraggableEditorObject<Tile> {

    protected IntVector2 coordinatesBeforeDrag;
    protected IntVector2 coordinatesWhileDragging;

    protected override void Awake() {
        base.Awake();
        runInPlayMode = true;
    }

    private void OnEnable() {
        if (!LevelManager.CurrentLevel.Tiles.ContainsCoordinates(obj.coordinates))
            LevelManager.CurrentLevel.Tiles.AddTile(obj, obj.coordinates);
    }

    protected override void OnDrag() {
        base.OnDrag();
        coordinatesBeforeDrag = obj.coordinates;
        LevelManager.CurrentLevel.Tiles.RemoveTile(obj.coordinates);
    }

    protected override void Drag() {
        base.Drag();
        obj.transform.position = MousePos.Rounded() + Tile.POSITION_OFFSET;
        coordinatesWhileDragging = MousePosCoordinates;
        canBePlaced = !LevelManager.CurrentLevel.Tiles.ContainsCoordinates(MousePosCoordinates);
       
        if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
            OnMouseClick();
    }

    protected override void CancelDrag() {
        base.CancelDrag();
        LevelManager.CurrentLevel.Tiles.AddTile(obj, coordinatesBeforeDrag);
        obj.transform.position = coordinatesBeforeDrag.ToVector3() + Tile.POSITION_OFFSET;
        SceneView.RepaintAll();
    }

    protected override void PlaceObject() {
        LevelManager.CurrentLevel.Tiles.AddTile(obj, coordinatesWhileDragging);
        obj.transform.position = coordinatesWhileDragging.ToVector3() + Tile.POSITION_OFFSET;
        obj.name = Tile.GetTypeName(obj, coordinatesWhileDragging);
        obj.coordinates = coordinatesWhileDragging;
        if(obj.occupant != null)
            obj.occupant.transform.position = new Vector3(obj.transform.position.x, obj.occupant.transform.position.y, obj.transform.position.z);
        base.PlaceObject();
    }

    private void OnDestroy() { 
        if (Application.isEditor && !Application.isPlaying && obj == null) 
            LevelManager.CurrentLevel.Tiles.RemoveTile(obj);
    }
}
