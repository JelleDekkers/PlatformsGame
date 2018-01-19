using System;

public static class GameEvents {

    /// <summary>
    /// Called when all objects are loaded into the scene
    /// </summary>
    public static Action OnLevelLoaded;

    /// <summary>
    /// Called when the level starts
    /// </summary>
    public static Action OnLevelStart;

    /// <summary>
    /// Called when the intro for all objects are done
    /// </summary>
    public static Action OnIntroComplete;

    /// <summary>
    /// Called when a lose condition has been met
    /// </summary>
    public static Action OnGameOver;

    /// <summary>
    /// Called when a win condition has been met
    /// </summary>
    public static Action OnLevelFinished;

    public static void ClearEvents() {
        OnLevelLoaded = null;
        OnLevelStart = null;
        OnIntroComplete = null;
        OnGameOver = null;
        OnLevelFinished = null;
    }
}
