using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorHelper  {

    public const float MAX_RAY_DISTANCE = 100000f;

    public static Vector3 GetMousePositionInScene(float y = 0) {
        Plane plane = new Plane(Vector3.up, y);
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        float rayDistance = MAX_RAY_DISTANCE;
        if (plane.Raycast(ray, out rayDistance)) {
            Vector3 hitPoint = ray.GetPoint(rayDistance);
            return new Vector3(LevelManager.Instance.transform.InverseTransformPoint(hitPoint).x, 
                               LevelManager.Instance.transform.position.y, 
                               LevelManager.Instance.transform.InverseTransformPoint(hitPoint).z);
        }
        return Vector3.zero;
    }
}
