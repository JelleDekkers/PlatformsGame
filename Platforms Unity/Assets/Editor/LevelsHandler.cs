using UnityEngine;
using System;
using Malee;

[CreateAssetMenu(fileName = "LevelsHandler", menuName = "Tools/Levels Handler", order = 1)]
public class LevelsHandler : ScriptableObjectSingleton<LevelsHandler> {

    #region chapters
    [Reorderable]
    public Chapters chapters;

    [Serializable]
    public class Chapter {

        public Color backgroundColor;

        [Reorderable(singleLine = true)]
        public LevelDrawer levels;
    }

    [Serializable]
    public class LevelCustomDrawer {

        public TextAsset level;
    }

    [Serializable]
    public class LevelDrawer : ReorderableArray<LevelCustomDrawer> { }

    [Serializable]
    public class Chapters : ReorderableArray<Chapter> { }
    #endregion
}