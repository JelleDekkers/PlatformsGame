using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CoordinateTileDictionary : SerializableDictionary<IntVector2, Tile> {

    public Tile GetTile(IntVector2 worldCoordinates) {
        Tile tile = null;
        TryGetValue(worldCoordinates, out tile);
        return tile;
    }

    public bool ContainsCoordinates(IntVector2 coordinates) {
        return ContainsKey(coordinates);
    }

    public void AddTile(Tile tile, IntVector2 coordinates) {
        if (ContainsCoordinates(coordinates)) {
            Debug.LogWarning(coordinates + " already present");
            return;
        }
        Add(coordinates, tile);
    }

    public void RemoveTile(IntVector2 coordinates) {
        if (!ContainsCoordinates(coordinates)) {
            Debug.LogWarning(coordinates + " is not present");
            return;
        }
        Remove(coordinates);
        CheckForMissingValues();
    }

    public void RemoveTile(Tile t) {
        RemoveTile(t.coordinates);
    }

    private void CheckForMissingValues() {
        List<IntVector2> list = new List<IntVector2>(Keys);
        foreach (IntVector2 key in list) {
            Tile t = null;
            TryGetValue(key, out t);
            if(t == null)
                RemoveTile(key);
        }
    }
}