using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int chapterNr, levelNr;
    public TextAsset[] levels;

    public void Start() {
#if !UNITY_EDITOR
        Achievements.AchievementManager.Setup();
#endif
        GameEvents.OnGameOver += () => StartCoroutine(GameOverCounter());
        GameEvents.OnLevelFinished += () => StartCoroutine(NextLevelCounter());

        if (LevelManager.CurrentLevel == null)
            LevelManager.Instance.LoadLevelFromFile(levels[levelNr - 1]);
        StartNewLevel();
    }

    private void StartNewLevel() {
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
        LevelManager.Instance.ReloadCurrentLevel();
    }

    private IEnumerator NextLevelCounter() {
        float time = 0;
        float duration = GeneralConfig.GameOverCounterDuration;
        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }
        levelNr++;
        Debug.Log("load next level " + levels[levelNr - 1].name);
        LevelManager.Instance.LoadLevelFromFile(levels[levelNr - 1]);
    }

    private void OnDestroy() {
        GameEvents.ClearEvents();
    }
}
