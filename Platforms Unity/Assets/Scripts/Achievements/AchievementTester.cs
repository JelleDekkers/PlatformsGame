using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Achievements;

public class AchievementTester : MonoBehaviour {
    
    public static int Counter { get; private set; }

    public void IncrementCounter() {
        AchievementManager.AchievementSystemInstance.IncrementAchievement(GPGSIds.achievement_baby_steps, 1);
        Counter++;

        Counter++;
    }

    public void FinishFirstLevel() {
        AchievementManager.AchievementSystemInstance.UnlockAchievement(GPGSIds.achievement_complete_first_level);
    }

    public void ShowAchievements() {
        AchievementManager.AchievementSystemInstance.ShowAchievementsUI();
    }

    public void ResetCounter() {
        Debug.Log("RESET final score: " + Counter);
        Counter = 0;
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
