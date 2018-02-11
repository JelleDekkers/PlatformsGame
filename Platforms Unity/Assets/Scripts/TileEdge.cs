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

    public override bool Equals(object obj) {
        return (TileOne == ((TileEdge)obj).TileOne) && (TileTwo == ((TileEdge)obj).TileTwo);
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 19;
            hash = TileOne.GetHashCode() * 17 + TileTwo.GetHashCode();
            return hash;
        }
    }
}
