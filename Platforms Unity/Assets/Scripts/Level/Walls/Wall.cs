using System;
using UnityEngine;

[Serializable]
public class Wall : MonoBehaviour {

    public static Vector3 GetCorrespondingRotation(TileEdge edge) {
        IntVector2 dif = edge.TileTwo.ToAbsolute() - edge.TileOne.ToAbsolute();

        if (edge.TileOne.x <= 0 && edge.TileTwo.x <= 0)
            dif.x *= -1;
        if (edge.TileOne.z <= 0 && edge.TileTwo.z <= 0)
            dif.z *= -1;

        if (dif.x == 0 && dif.z == 1)
            return new Vector3(0, -90, 0);
        else if (dif.x == 0 && dif.z == -1)
            return new Vector3(0, 90, 0);
        else if (dif.x == 1 && dif.z == 0)
            return new Vector3(0, 0, 0);
        else
            return new Vector3(0, 180, 0);
    }

    public static Vector3 GetCorrespondingRotation(IntVector2 coordinatesOne, IntVector2 coordinatesTWo) {
        return GetCorrespondingRotation(new TileEdge(coordinatesOne, coordinatesTWo));
    }
}
