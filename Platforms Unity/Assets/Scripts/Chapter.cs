using UnityEngine;
using System;
using Malee;

[Serializable]
public class Chapter {

    public Color color;
    [Reorderable]
    public TextAsset[] levels;

    public int LevelCount { get { return levels.Length; } }
}


