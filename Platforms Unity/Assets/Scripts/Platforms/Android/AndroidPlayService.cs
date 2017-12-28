using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class AndroidPlayService : MonoBehaviour {

    private void Start() {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        SignIn();
    }

    private void SignIn() {
        Social.localUser.Authenticate(success => { Debug.Log("Social local user authenticated!"); });
    }

    // achievements stuff: needs to become interface
    public static void UnlockAchievement(string id) {
        Social.ReportProgress(id, 100f, success => { Debug.Log("Achievement unlocked of id " + id); });
    }

    public static void IncrementAchievement(string id, int stepsToIncrement) {
        PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { Debug.Log("Achievement progress incremented by " + stepsToIncrement); });
    }

    public static void ShowAchievementsUI() {
        Social.ShowAchievementsUI();
    }
}
