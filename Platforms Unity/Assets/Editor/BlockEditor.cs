using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Block), true)]
public class BlockEditor : DraggableEditorObject<Block>, IDuplicatable {

    protected Tile tileStandingOnBeforeDrag;
    protected Tile tileCurrentlySnappedTo;

    private float heightOffset = 0.5f;

    protected override void Awake() {
        base.Awake();
        runInPlayMode = false;
        dragging = (obj.tileStandingOn == null);
    }

    protected override void OnMouseClick() {
        if (canBePlaced) {
            PlaceObject();
            if (Event.current.modifiers == EventModifiers.Shift)
                Duplicate();
        } else {
            CancelDrag();
        }
    }

    protected override void OnDrag() {
        base.OnDrag();
        if (obj.tileStandingOn != null) {
            tileStandingOnBeforeDrag = obj.tileStandingOn;
            obj.tileStandingOn.SetOccupant(null);
            EditorUtility.SetDirty(obj.tileStandingOn);
            obj.tileStandingOn = null;
        }
    }

    protected override void Drag() {
        base.Drag();

        tileCurrentlySnappedTo = LevelManager.CurrentLevel.Tiles.GetTile(MousePosCoordinates);

        if (tileCurrentlySnappedTo != null) {
            obj.transform.position = new Vector3(tileCurrentlySnappedTo.transform.position.x, heightOffset, tileCurrentlySnappedTo.transform.position.z);
            obj.transform.eulerAngles = GetCorrespondingRotation();
            if (tileCurrentlySnappedTo.occupant == null) {
                canBePlaced = true;
            } else {
                obj.transform.position = EditorHelper.GetMousePositionInScene().Rounded() + BLOCK_OFFSET;
                canBePlaced = false;
            }
        } else {
            obj.transform.position = EditorHelper.GetMousePositionInScene().Rounded() + BLOCK_OFFSET;
            canBePlaced = false;
        }

        if (Event.current.button == 0 && Event.current.type == EventType.mouseUp)
            OnMouseClick();
    }

    protected override void CancelDrag() {
        base.CancelDrag();
        if (tileStandingOnBeforeDrag == null) {
            DestroyImmediate(obj.gameObject);
        } else {
            obj.tileStandingOn = tileStandingOnBeforeDrag;
            obj.tileStandingOn.SetOccupant(obj);
            obj.transform.position = new Vector3(obj.tileStandingOn.transform.position.x, heightOffset, obj.tileStandingOn.transform.position.z);
            EditorUtility.SetDirty(obj.tileStandingOn);
        }
        SceneView.RepaintAll();
    }

    protected override void PlaceObject() {
        obj.tileStandingOn = tileCurrentlySnappedTo;
        obj.tileStandingOn.SetOccupant(obj);
        obj.transform.SetParent(LevelManager.Instance.transform);
        EditorUtility.SetDirty(obj.tileStandingOn);
        base.PlaceObject();
    }

    private Vector3 GetCorrespondingRotation() {
        Vector3 difference = tileCurrentlySnappedTo.coordinates.ToVector3() - MousePos;
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

    public void Duplicate() {
        Object prefabRoot = PrefabUtility.GetPrefabParent(Selection.activeGameObject);
        Object duplicatedObj;

        if (prefabRoot != null)
            duplicatedObj = PrefabUtility.InstantiatePrefab(prefabRoot);
        else
            duplicatedObj = Instantiate(Selection.activeGameObject);

        GameObject dup = duplicatedObj as GameObject;
        Selection.activeGameObject = dup;
        dup.transform.position = MousePos;
    }
}
