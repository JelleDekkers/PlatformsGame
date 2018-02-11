using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileEdgeWallDictionary : SerializableDictionary<TileEdge, Wall> {

    public Wall GetWall(TileEdge edge) {
        return GetWall(edge.TileOne, edge.TileTwo);
    }

    public Wall GetWall(IntVector2 one, IntVector2 two) {
        Wall wall = null;
        TryGetValue(new TileEdge(one, two), out wall);
        if(wall == null)
            TryGetValue(new TileEdge(two, one), out wall);
        return wall;
    }

    public bool ContainsWall(TileEdge edge) {
        //Debug.Log(edge + " hash 1" + edge.TileOne.GetHashCode() + " hash 2" + edge.TileTwo.GetHashCode());
        //Debug.Log(ContainsKey(edge) + " " + ContainsKey(new TileEdge(edge.TileTwo, edge.TileOne)));
        //Wall wallFound = GetWall(edge.TileOne, edge.TileTwo);
        //if (wallFound != null) {
        //    Debug.Log(wallFound.Edge + " hash 1" + wallFound.Edge.TileOne.GetHashCode() + " hash 2" + wallFound.Edge.TileTwo.GetHashCode());
        //    Debug.Log(wallFound.Edge.Equals(edge) + " " + edge.GetHashCode() + " " + wallFound.Edge.GetHashCode());
        //}
        return ContainsKey(edge) || ContainsKey(new TileEdge(edge.TileTwo, edge.TileOne));
    }

    public bool ContainsWall(IntVector2 one, IntVector2 two) {
        return ContainsWall(new TileEdge(one, two));
    }

    public void AddWall(TileEdge edge, Wall wall) {
        if (ContainsWall(edge)) {
            Debug.LogWarning("Wall between " + edge.TileOne + " and " + edge.TileTwo + " already present");
            return;
        }
        Add(edge, wall);
    }

    public void AddWall(IntVector2 one, IntVector2 two, Wall wall) {
        AddWall(new TileEdge(one, two), wall);
    }

    public void RemoveWall(TileEdge edge) {
        if (!ContainsWall(edge)) {
            Debug.LogWarning("No wall between " + edge.TileOne + " and " + edge.TileTwo);
            return;
        }

        if (!Remove(edge))
           Remove(new TileEdge(edge.TileTwo, edge.TileOne));

        //CheckForMissingValues();
    }

    public void RemoveWall(IntVector2 one, IntVector2 two) {
        RemoveWall(new TileEdge(one, two));
    }

    private void CheckForMissingValues() {
        List<TileEdge> list = new List<TileEdge>(Keys);
        foreach (TileEdge key in list) {
            Wall w = null;
            TryGetValue(key, out w);
            if (w == null)
                RemoveWall(key);
        }
    }
}