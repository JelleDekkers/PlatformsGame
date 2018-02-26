using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs Manager", menuName = "Tools/Prefabs Manager", order = 1)]
public class PrefabManager : ScriptableObjectSingleton<PrefabManager> {

    [SerializeField] TileNameDictionary tiles;
    public static TileNameDictionary Tiles { get { return Instance.tiles; } }

    private DataLink<Tile> tilesDataLink;
    public static DataLink<Tile> TilesDataLink {
        get {
            if (Instance.tilesDataLink == null)
                Instance.tilesDataLink = new DataLink<Tile>(Instance.tiles);
            return Instance.tilesDataLink;
        }
    }

    [SerializeField] BlockNameDictionary blocks;
    public static BlockNameDictionary Blocks { get { return Instance.blocks; } }

    private DataLink<Block> blocksDataLink;
    public static DataLink<Block> BlocksDataLink {
        get {
            if (Instance.blocksDataLink == null)
                Instance.blocksDataLink = new DataLink<Block>(Instance.blocks);
            return Instance.blocksDataLink;
        }
    }

    [SerializeField] WallNameDictionary walls;
    public static WallNameDictionary Walls { get { return Instance.walls; } }

    private DataLink<Wall> wallsDataLink;
    public static DataLink<Wall> WallsDataLink {
        get {
            if (Instance.wallsDataLink == null)
                Instance.wallsDataLink = new DataLink<Wall>(Instance.walls);
            return Instance.wallsDataLink;
        }
    }
}

public class DataLink<T> where T : MonoBehaviour {

    private Dictionary<Type, T> prefabLinks;

    public DataLink(T[] prefabs) {
        prefabLinks = new Dictionary<Type, T>();
        foreach (T t in prefabs)
            prefabLinks.Add(t.GetType(), t);
    }

    public DataLink(Dictionary<T, string> prefabs) {
        prefabLinks = new Dictionary<Type, T>();
        foreach (KeyValuePair<T, string> pair in prefabs)
            prefabLinks.Add(pair.Key.GetType(), pair.Key);
    }

    public T GetPrefabByType(Type t) {
        try {
            return prefabLinks[t];
        } catch (Exception ex) {
            Debug.Log("Could not find corresponding prefab of type " + typeof(T).ToString() + " with type " + t + " + ex");
            throw ex;
        }
    }
}
