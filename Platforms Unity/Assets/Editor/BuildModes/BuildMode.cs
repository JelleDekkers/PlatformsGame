using UnityEditor;
using UnityEngine;

public abstract class BuildMode {

    public abstract Object[] Prefabs { get; }
    public abstract string[] PrefabNames { get; protected set; }

    protected Level Level { get { return LevelManager.CurrentLevel; } }

    private int objectIndex;

    public abstract void Update();
    public abstract void Build(int index, Vector3 coordinates);
    public abstract void Remove(Vector3 coordinates);
    public abstract bool IsValidPosition(Vector3 position);

    public virtual void DrawHelperGizmos(bool isValidPosition, Vector3 position, Vector3 size) {
        Handles.color = (isValidPosition) ? Color.green : Color.red;
        Handles.DrawWireCube(position, size);
        SceneView.RepaintAll();
    }

    protected string[] GetObjectNames(Object[] arr) {
        string[] names = new string[arr.Length];
        for (int i = 0; i < arr.Length; i++)
            names[i] = arr[i].name;
        return names;
    }

}
