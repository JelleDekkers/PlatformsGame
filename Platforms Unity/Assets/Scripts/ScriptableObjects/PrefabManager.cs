using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs Manager", menuName = "Tools/Prefabs Manager", order = 1)]
public class PrefabManager : ScriptableObjectSingleton<PrefabManager> {

    [SerializeField] private Tile[] tiles; 
    [SerializeField] private Block[] blocks;

    private DataLink<Tile> tilesDataLink;
    public static DataLink<Tile> TilesDataLink {
        get {
            if (Instance.tilesDataLink == null)
                Instance.tilesDataLink = new DataLink<Tile>(Instance.tiles);
            return Instance.tilesDataLink;
        }
    }

    private DataLink<Block> blocksDataLink;
    public static DataLink<Block> BlocksDataLink {
        get {
            if (Instance.blocksDataLink == null)
                Instance.blocksDataLink = new DataLink<Block>(Instance.blocks);
            return Instance.blocksDataLink;
        }
    }

    [SerializeField]
    private Portal portal;
    public static Portal Portal { get { return Instance.portal; } }
}

public class DataLink<T> where T : MonoBehaviour {

    private Dictionary<Type, T> prefabLinks;

    public DataLink(T[] prefabs) {
        prefabLinks = new Dictionary<Type, T>();
        foreach (T t in prefabs)
            prefabLinks.Add(t.GetType(), t);
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
