using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions {

    public static IntVector2 RoundToIntVector2(this UnityEngine.Vector2 vector2) {
        return new IntVector2(Mathf.RoundToInt(vector2.x), Mathf.RoundToInt(vector2.y));
    }

    /// <summary>
    /// Converts IntVector2 coordinates to a single array index.
    /// </summary>
    public static int ToSingleIndex(this IntVector2 coordinates, IntVector2 levelSize) {
        return coordinates.x * levelSize.z + coordinates.z;
    }

    public static Vector3 ToVector3(this IntVector2 coordinate, float y = 0) {
        return new Vector3(coordinate.x, y, coordinate.z);
    }

    public static IntVector2 ToIntVector2(this Vector3 position) {
        return new IntVector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
    }

    public static Vector3 Rounded(this Vector3 position) {
        position.x = (position.x > 0) ? (int)position.x : (int)position.x - 1;
        position.z = (position.z > 0) ? (int)position.z : (int)position.z - 1;
        return position;
    }

    public static bool ContainsCoordinate<T>(this T[] array, IntVector2 coordinate, IntVector2 levelSize) {
        bool containsX = coordinate.x >= 0 && coordinate.x < levelSize.x;
        bool containsZ = coordinate.z >= 0 && coordinate.z < levelSize.z;
        return (containsX == true && containsZ == true);
    }

    public static bool ContainsCoordinate<T>(this List<T> array, IntVector2 coordinate, IntVector2 levelSize) {
        bool containsX = coordinate.x >= 0 && coordinate.x < levelSize.x;
        bool containsZ = coordinate.z >= 0 && coordinate.z < levelSize.z;
        return (containsX == true && containsZ == true);
    }

    public static int RoundToInt(this float i) {
        if (i > 0)
            return (int)i;
        else
            return (int)i - 1;
    }

    public static IntVector2 ToAbsolute(this IntVector2 coordinates) {
        coordinates.x = Mathf.Abs(coordinates.x);
        coordinates.z = Mathf.Abs(coordinates.z);
        return coordinates;
    }

    public static T GetRandom<T>(this T[] array) {
        int r = Random.Range(0, array.Length);
        return array[r];
    }

    public static T LoopedIndex<T>(this T[] array, int index) {
        if (index > array.Length - 1)
            return array[index - array.Length];
        else
            return array[index];
    }

    public static T GetInterface<T>(this GameObject obj) where T : class {
        if (!typeof(T).IsInterface) {
            Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
            return null;
        }
        return obj.GetComponent<T>() as T;
    }

    public static T[] GetInterfaces<T>(this GameObject obj) where T : class {
        if (!typeof(T).IsInterface) {
            Debug.LogError(typeof(T).ToString() + ": is not an actual interface!");
            return null; ;
        }

        return obj.GetComponents<T>() as T[];
    }

    public static T GetComponentWithInterface<T>(this GameObject gObject, out Object objectWithInterface) where T : class {
        Component[] components = gObject.GetComponents(typeof(Component));
        foreach(Component c in components) {
            if(c is T) {
                objectWithInterface = c;
                return c as T;
            }
        }
        objectWithInterface = null;
        return null;
    }
}
