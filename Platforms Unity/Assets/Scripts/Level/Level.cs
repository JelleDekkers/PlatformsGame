using System;
using UnityEngine;

[Serializable]
public class Level {

    [SerializeField]
    private CoordinateTileDictionary tiles;
    public CoordinateTileDictionary Tiles { get { return tiles; } }

    [SerializeField]
    private TileEdgeWallDictionary walls;
    public TileEdgeWallDictionary Walls { get { return walls; } }

    public Level() {
        tiles = new CoordinateTileDictionary();
        walls = new TileEdgeWallDictionary();
    }
}
