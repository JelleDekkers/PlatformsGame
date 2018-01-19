using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

#if UNITY_ANDROID || UNITY_IOS
namespace Achievements {

    public class AndroidAchievementSystem : IAchievementSystem {

        public AndroidAchievementSystem() {
            Debug.Log("Constructor for AndroidAchievementSystem");
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
            Social.localUser.Authenticate(ProcessAuthentication);
        }

        void ProcessAuthentication(bool success) {
            if (success)
                Debug.Log("Succesfully Authenticated");
            else
                Debug.Log("Failed to authenticate");
        }

        public void IncrementAchievement(string id, int stepsToIncrement) {
            Debug.Log("Trying to increment achievement");
            PlayGamesPlatform.Instance.IncrementAchievement(id, stepsToIncrement, success => { Debug.Log("Succesfully incrementend achievement progress by" + stepsToIncrement); });
        }

        public void UnlockAchievement(string id) {
            Debug.Log("trying to unlock achievement");
            //PlayGamesPlatform.Instance.UnlockAchievement(id, success => { Debug.Log("Achievement unlocked, id: " + id); });
            Social.ReportProgress(id, 100f, success => { Debug.Log("Achievement unlocked, id: " + id); });
        }

        public void ShowAchievementsUI() {
            Debug.Log("Trying to show Achievements UI");
            //PlayGamesPlatform.Instance.ShowAchievementsUI();
            Social.ShowAchievementsUI();
        }
    }
}
#endif