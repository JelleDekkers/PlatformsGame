using UnityEngine;
using UnityEditor;

public class DraggableEditorObject<T> : Editor where T : MonoBehaviour {

    protected T obj;
    /// <summary>
    /// Enables editing stuff during play
    /// </summary>
    protected bool runInPlayMode = false;
    protected bool dragging;
    protected GUIStyle guiStyle;
    protected Ray ray;
    protected RaycastHit hit;
    protected Vector3 MousePos { get { return EditorHelper.GetMousePositionInScene(); } }
    protected IntVector2 MousePosCoordinates { get { return EditorHelper.GetMousePositionInScene().Rounded().ToIntVector2(); } }
    protected bool canBePlaced = true;
    protected bool canBePlacedPrevFrame = true;
    protected readonly Vector3 BLOCK_OFFSET = new Vector3(0.5f, 0.5f, 0.5f);

    private bool draggingPrevFrame;
    
    protected virtual void Awake() {
        obj = (T)target;
    }

    protected virtual void OnSceneGUI() {
        if (EditorApplication.isPlaying && !runInPlayMode)
            return;

        ExplanationGUI();

        if (Event.current.keyCode == KeyCode.Space && Event.current.type == EventType.KeyDown)
            ToggleSnapMode();

        if (Event.current.keyCode == KeyCode.Escape && Event.current.type == EventType.KeyDown && dragging)
            CancelDrag();

        if (dragging)
            Drag();

        draggingPrevFrame = dragging;
        canBePlacedPrevFrame = canBePlaced;
    }

    protected virtual void OnMouseClick() {
        if (canBePlaced) 
            PlaceObject();
        else 
            CancelDrag();
    }

    protected virtual void Drag() {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));

        if (draggingPrevFrame != dragging)
            OnDrag();

        ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        hit = new RaycastHit();
    }

    protected virtual void OnDrag() { }

    protected virtual void CancelDrag() {
        dragging = false;
    }

    protected virtual void ToggleSnapMode() {
        dragging = !dragging;
        if (dragging == false)
            CancelDrag();
    }

    protected virtual void ExplanationGUI() {
        Handles.BeginGUI();
        guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 80, 1000, 20), "Left click to place ", guiStyle);
        GUI.Label(new Rect(10, 105, 1000, 20), "Press Space to toggle snap mode", guiStyle);
        //GUI.Label(new Rect(10, 60, 1000, 20), "Left Click + Shift to Duplicate", guiStyle);
        Handles.EndGUI();
    }

    protected virtual void PlaceObject() {
        PlaceObjectBaseLogic();
    }

    protected void PlaceObjectBaseLogic() {
        dragging = false;
        Undo.RegisterCreatedObjectUndo(obj.gameObject, "Created: " + obj.name);
        EditorUtility.SetDirty(obj);
        SceneView.RepaintAll();
    }
}
