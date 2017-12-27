using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public void Start() {
        if(GameEvents.OnLevelStart != null)
            GameEvents.OnLevelStart.Invoke();
        GameEvents.OnIntroDone += () => Debug.Log("intro done");
        GameEvents.OnGameOver += () => Debug.Log("Game over");

        if (TileSettings.UseStartingTransitions)
            StartCoroutine(IntroCounter());
        else
            GameEvents.OnIntroDone.Invoke();
    }

    private IEnumerator IntroCounter() {
        float time = 0;
        float duration = PlayerSettings.IntroDelay + PlayerSettings.IntroDuration + 0.5f; // general settings
        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }

        GameEvents.OnIntroDone.Invoke();
    }

    private void ResetLevel() {
        //onGameOver
    }

    private void OnDestroy() {
        GameEvents.ClearEvents();
    }
}
