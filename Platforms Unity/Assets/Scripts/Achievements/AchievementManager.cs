using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour {

    public static AchievementManager Instance { get; private set; }
    public static int Counter { get; private set; }

    private void Start() {
        Instance = this;
    }

    public void AddPoint() {
        Counter++;
    }

    public void IncrementCounter() {
        AndroidPlayService.IncrementAchievement(GPGSIds.achievement_baby_steps, 1);
        Counter++;
    }

    public void FinishFirstLevel() {
        AndroidPlayService.UnlockAchievement(GPGSIds.achievement_complete_first_level);
    }

    public void ResetCounter() {
        Debug.Log("final score: " + Counter);
        Counter = 0;
    }

    public void ShowAchievements() {
        AndroidPlayService.ShowAchievementsUI();
    }

    private void OnGUI() {
        GUI.Label(new Rect(10, 100, 1000, 20), "Counter: " + Counter);

        if (GUI.Button(new Rect(10, 130, 200, 20), "Counter++"))
            IncrementCounter();

        if (GUI.Button(new Rect(10, 150, 200, 20), "Finish level"))
            FinishFirstLevel();

        if (GUI.Button(new Rect(10, 170, 200, 20), "reset"))
            ResetCounter();

        if (GUI.Button(new Rect(10, 190, 200, 20), "showAchievments"))
            ShowAchievements();
    }
}
