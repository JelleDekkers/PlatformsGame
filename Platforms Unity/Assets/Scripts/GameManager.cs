using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public void Start() {
#if !UNITY_EDITOR
        Achievements.AchievementManager.Setup();
#endif
        GameEvents.OnGameOver += () => StartCoroutine(GameOverCounter());
        StartNewLevel();
    }

    private void StartNewLevel() {
        if (LevelManager.CurrentLevel == null)
            LevelManager.Instance.LoadLevelFromFile();

        if (GameEvents.OnLevelStart != null)
            GameEvents.OnLevelStart.Invoke();

        if (GeneralConfig.UseTransitionAnimations)
            StartCoroutine(IntroCounter());
        else
            GameEvents.OnIntroComplete.Invoke();
    }

    private IEnumerator IntroCounter() {
        float time = 0;
        float duration = PlayerConfig.IntroAnimationStartingDelay + PlayerConfig.IntroAnimationDuration + GeneralConfig.IntroCounterExtraDurationOffset; 
        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }

        GameEvents.OnIntroComplete.Invoke();
    }

    private IEnumerator GameOverCounter() {
        float time = 0;
        float duration = GeneralConfig.GameOverCounterDuration;
        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }

        NextLevel();

        RestartLevel();
    }

    private void RestartLevel() {
        LevelManager.Builder.ClearLevel();
        LevelManager.CurrentLevel = null;
        StartNewLevel();
    }

    private void NextLevel() {

    }

    private void OnDestroy() {
        GameEvents.ClearEvents();
    }
}
