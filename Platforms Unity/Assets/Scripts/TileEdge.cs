using System;
using UnityEngine;

[Serializable]
public class TileEdge {

    public IntVector2 TileOne;// { get; private set; }
    public IntVector2 TileTwo;// { get; private set; }

    public TileEdge(IntVector2 one, IntVector2 two) {
        TileOne = one;
        TileTwo = two;
    }

    public override string ToString() {
        return TileOne.ToString() + " " + TileTwo.ToString();
    }

    //public static TileEdge operator +(TileEdge first, TileEdge second) {
    //    first.TileOne += second.TileTwo;
    //    first.TileTwo += second.TileTwo;
    //    return first;
    //}

    //public static TileEdge operator -(TileEdge first, TileEdge second) {
    //    first.TileOne -= second.TileTwo;
    //    first.TileTwo -= second.TileTwo;
    //    return first;
    //}

    //public static bool operator ==(TileEdge first, TileEdge second) {
    //    return (first.TileOne == second.TileOne) && (first.TileTwo == second.TileTwo);
    //}

    //public static bool operator !=(TileEdge first, TileEdge second) {
    //    if (first == null || second == null)
    //        return false;
    //    return (first.TileOne != second.TileOne) || (first.TileTwo != second.TileTwo);
    //}

    public override bool Equals(object obj) {
        return (TileOne != ((TileEdge)obj).TileOne) || (TileTwo != ((TileEdge)obj).TileTwo);
    }

    public override int GetHashCode() {
        unchecked {
            int hash = TileOne.GetHashCode() + TileTwo.GetHashCode();
            return hash;
        }
    }
}
