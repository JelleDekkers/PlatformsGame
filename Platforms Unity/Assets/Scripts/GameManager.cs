﻿using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public void Start() {
#if !UNITY_EDITOR
        Achievements.AchievementManager.Setup();
#endif
        if(GameEvents.OnLevelStart != null)
            GameEvents.OnLevelStart.Invoke();
        GameEvents.OnIntroComplete += () => Debug.Log("intro done");
        GameEvents.OnGameOver += () => Debug.Log("Game over");
        GameEvents.OnGameOver += () => StartCoroutine(GameOverCounter());

        if (TileSettings.UseStartingTransitions)
            StartCoroutine(IntroCounter());
        else
            GameEvents.OnIntroComplete.Invoke();
    }

    private IEnumerator IntroCounter() {
        float time = 0;
        float duration = PlayerSettings.IntroDelay + PlayerSettings.IntroDuration + 0.5f; // general settings
        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }

        GameEvents.OnIntroComplete.Invoke();
    }

    private IEnumerator GameOverCounter() {
        float time = 0;
        float duration = 1f; // general settings
        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }

        ResetLevel();
    }

    private void ResetLevel() {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.levelAsset);

        if (TileSettings.UseStartingTransitions)
            StartCoroutine(IntroCounter());
        else
            GameEvents.OnIntroComplete.Invoke();
    }

    private void OnDestroy() {
        GameEvents.ClearEvents();
    }
}
