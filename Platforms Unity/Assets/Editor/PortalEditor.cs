﻿using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Portal), true)]
public class PortalEditor : DraggableEditorObject<Portal> {

    private TileEdge snappedTobeforeDrag;
    
    protected TileEdge snappedToWhileDragging;

    protected override void Awake() {
        base.Awake();
        runInPlayMode = false;
        dragging = !LevelManager.CurrentLevel.Walls.ContainsWall(obj.Edge);
        //if (obj.Edge != null)
        //    snappedTobeforeDrag = obj.Edge;
    }

    protected override void OnDrag() {
        base.OnDrag();
        snappedTobeforeDrag = obj.Edge;
        if(LevelManager.CurrentLevel.Walls.ContainsWall(snappedTobeforeDrag))
            LevelManager.CurrentLevel.Walls.RemoveWall(snappedTobeforeDrag);   
    }

    protected override void Drag() {
        base.Drag();
        snappedToWhileDragging = new TileEdge(MousePosCoordinates, GetNearestNeighbourCoordinates(MousePosCoordinates));
        obj.transform.position = snappedToWhileDragging.TileOne.ToVector3(0) + Tile.POSITION_OFFSET;
        obj.transform.eulerAngles = Wall.GetCorrespondingRotation(snappedToWhileDragging);
        SceneView.RepaintAll();

        if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
            OnMouseClick();
    }

    protected override void OnMouseClick() {
        if (canBePlaced)
            PlaceObject();
        else
            CancelDrag();
    }

    private IntVector2 GetNearestNeighbourCoordinates(IntVector2 original) {
        Vector3 dif = MousePos - original.ToVector3(0);
        dif.x -= 0.5f;
        dif.z -= 0.5f;

        if (Mathf.Abs(dif.x) > Mathf.Abs(dif.z)) 
            return MousePosCoordinates + new IntVector2(RoundAwayFromZero(dif.x), 0);
        else 
            return MousePosCoordinates + new IntVector2(0, RoundAwayFromZero(dif.z));
    }

    private int RoundAwayFromZero(float f) {
        if (f >= 0)
            return 1;
        else
            return -1;
    }

    protected override void CancelDrag() {
        base.CancelDrag();
        if(snappedTobeforeDrag == null) {
            DestroyImmediate(obj.gameObject);
        } else {
            obj.transform.position = snappedTobeforeDrag.TileOne.ToVector3(0) + Tile.POSITION_OFFSET;
            obj.transform.eulerAngles = Wall.GetCorrespondingRotation(snappedTobeforeDrag);
            obj.SetEdge(snappedTobeforeDrag);
            LevelManager.CurrentLevel.Walls.AddWall(snappedTobeforeDrag, obj);
        }
        SceneView.RepaintAll();
    }

    protected override void PlaceObject() {
        obj.transform.position = snappedToWhileDragging.TileOne.ToVector3(0) + Tile.POSITION_OFFSET;
        obj.transform.eulerAngles = Wall.GetCorrespondingRotation(snappedToWhileDragging);
        obj.SetEdge(snappedToWhileDragging);
        if(LevelManager.CurrentLevel.Walls.ContainsWall(snappedTobeforeDrag))
            LevelManager.CurrentLevel.Walls.RemoveWall(snappedTobeforeDrag);
        LevelManager.CurrentLevel.Walls.AddWall(snappedToWhileDragging, obj);
        obj.transform.SetParent(LevelManager.Instance.transform);
        base.PlaceObject();
    }

    private void OnDestroy() {
        if (Application.isEditor && !Application.isPlaying && obj == null && snappedTobeforeDrag != null) {
            Debug.Log("remove wall");
            LevelManager.CurrentLevel.Walls.RemoveWall(snappedTobeforeDrag);
        }
    }
}