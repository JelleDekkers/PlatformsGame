using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public void Start() {
#if !UNITY_EDITOR
        Achievements.AchievementManager.Setup();
#endif
        if(GameEvents.OnLevelStart != null)
            GameEvents.OnLevelStart.Invoke();
        GameEvents.OnGameOver += () => StartCoroutine(GameOverCounter());

        if (TileSettings.UseStartingTransitions)
            StartCoroutine(IntroCounter());
        else
            GameEvents.OnIntroComplete.Invoke();
    }

    private IEnumerator IntroCounter() {
        float time = 0;
        float duration = PlayerSettings.IntroDelay + PlayerSettings.IntroDuration + 0.5f; // TODO: general settings
        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }

        GameEvents.OnIntroComplete.Invoke();
    }

    private IEnumerator GameOverCounter() {
        Debug.Log("Game Over");
        float time = 0;
        float duration = 1f; // TODO: general settings
        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }

        RestartLevel();
    }

    private void RestartLevel() {
        LevelManager.Instance.LoadLevelFromFile(LevelManager.Instance.levelAsset);

        if (TileSettings.UseStartingTransitions)
            StartCoroutine(IntroCounter());
        else
            GameEvents.OnIntroComplete.Invoke();
    }

    private void OnDestroy() {
        GameEvents.ClearEvents();
    }
}
