using UnityEngine;

namespace Achievements {

    public class CustomAchievementsSystem : IAchievementSystem {

        public CustomAchievementsSystem() {
            Debug.Log("Constructor for CustumAchivementSystem");
        }

        public void IncrementAchievement(string id, int stepsToIncrement) {
            Debug.Log("Trying to increment achievement");
            throw new System.NotImplementedException();
        }

        public void UnlockAchievement(string id) {
            Debug.Log("trying to unlock achievement");
            throw new System.NotImplementedException();
        }

        public void ShowAchievementsUI() {
            Debug.Log("Trying to show Achievements UI");
            throw new System.NotImplementedException();
        }
    }
}