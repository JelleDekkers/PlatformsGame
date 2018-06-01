using UnityEngine;
using System;
using System.Collections.Generic;
using Serialization;
using System.Xml.Serialization;
using System.IO;
using Malee;

[CreateAssetMenu(fileName = "LevelsHandler", menuName = "Tools/Levels Handler", order = 1)]
public class LevelsHandler : ScriptableObjectSingleton<LevelsHandler> {

    #region chapters
    [Reorderable]
    public Chapters chapters;

    [System.Serializable]
    public class Chapter {

        public Color backgroundColor;

        [Reorderable(singleLine = true)]
        public LevelDrawer levels;
    }

    [System.Serializable]
    public class LevelCustomDrawer {

        public TextAsset level;
    }

    [System.Serializable]
    public class LevelDrawer : ReorderableArray<LevelCustomDrawer> { }

    [System.Serializable]
    public class Chapters : ReorderableArray<Chapter> { }
    #endregion
}