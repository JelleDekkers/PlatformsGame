using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Achievements {

    public static class AchievementManager {

        private static IAchievementSystem achievementSystemInstance;
        public static IAchievementSystem AchievementSystemInstance {
            get {
                if (achievementSystemInstance == null)
                    achievementSystemInstance = GetPlatformDependentAchivementSystem();
                return achievementSystemInstance;
            }
        }

        public static IAchievementSystem GetPlatformDependentAchivementSystem() {
#if UNITY_ANDROID || UNITY_IOS
            return new AndroidAchievementSystem();
#else
            return new CustomAchievementsSystem);
#endif
        }

    }
}