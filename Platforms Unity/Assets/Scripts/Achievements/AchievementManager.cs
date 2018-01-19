using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Achievements {

    public static class AchievementManager {

        private static IAchievementSystem achievementSystemInstance;
        public static IAchievementSystem AchievementSystemInstance {
            get {
                if (achievementSystemInstance == null)
                    achievementSystemInstance = GetPlatformDependentAchievementSystem();
                return achievementSystemInstance;
            }
        }

        public static void Setup() {
            Debug.Log("Achievement manager setup()");
            achievementSystemInstance = GetPlatformDependentAchievementSystem();
        }

        public static IAchievementSystem GetPlatformDependentAchievementSystem() {
            Debug.Log("GetPlatformDependentAchievementSystem()");
#if UNITY_ANDROID || UNITY_IOS
            return new AndroidAchievementSystem();
#else
            return new CustomAchievementsSystem();
#endif
        }

    }
}