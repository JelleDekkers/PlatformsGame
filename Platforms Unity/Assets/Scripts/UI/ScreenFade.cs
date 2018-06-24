using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {

    public class ScreenFade : MonoBehaviour {

        [SerializeField] public CanvasGroup canvasGroup;

        private void Awake() {
            if(GeneralConfig.UseScreenFade) { 
                //GameEvents.OnGameOver += FadeOutFunction;
                canvasGroup.alpha = 1;
                StartCoroutine(FadeIn());
            }
        }

        private void FadeInFunction() {
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn() {
            float timer = 0;

            while(timer < GeneralConfig.ScreenFadeInTime) {
                canvasGroup.alpha = 1 - timer / GeneralConfig.ScreenFadeInTime;
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

            while (timer < GeneralConfig.ScreenFadeOutTime) {
                canvasGroup.alpha = timer / GeneralConfig.ScreenFadeOutTime;
                timer += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1;
        }
    }
}