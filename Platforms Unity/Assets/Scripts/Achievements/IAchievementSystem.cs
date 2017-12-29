namespace Achievements {

    public interface IAchievementSystem {

        void UnlockAchievement(string id);
        void IncrementAchievement(string id, int stepsToIncrement);
        void ShowAchievementsUI();
    }
}