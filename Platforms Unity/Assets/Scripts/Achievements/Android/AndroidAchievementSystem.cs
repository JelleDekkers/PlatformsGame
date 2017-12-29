using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Achievements {

    public class AndroidAchievementSystem : IAchievementSystem {

        public AndroidAchievementSystem() {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
            Social.localUser.Authenticate(success => { Debug.Log("Social local user authenticated!"); });
        }

        public void IncrementAchievement(string id, int stepsToIncrement) {
            PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { Debug.Log("Achievement progress incremented by " + stepsToIncrement); });
        }

        public void UnlockAchievement(string id) {
            Social.ReportProgress(id, 100f, success => { Debug.Log("Achievement unlocked, id: " + id); });
        }

        public void ShowAchievementsUI() {
            Social.ShowAchievementsUI();
        }
    }
}