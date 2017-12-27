using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {

    public class ScreenFade : MonoBehaviour {

        [SerializeField] bool useFade = true;
        [SerializeField] float fadeInTime = 1;
        [SerializeField] float fadeOuTime = 1;

        private CanvasGroup canvasGroup;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();

            if(useFade)
                GameEvents.OnGameOver += FadeOutFunction;

            if(useFade) {
                canvasGroup.alpha = 1;
                StartCoroutine(FadeIn());
            }
        }

        private void FadeInFunction() {
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn() {
            float timer = 0;

            while(timer < fadeInTime) {
                canvasGroup.alpha = 1 - timer / fadeInTime;
                timer += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 0;
        }

        private void FadeOutFunction() {
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut() {
            float timer = 0;

            while (timer < fadeOuTime) {
                canvasGroup.alpha = timer / fadeOuTime;
                timer += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1;
        }
    }
}