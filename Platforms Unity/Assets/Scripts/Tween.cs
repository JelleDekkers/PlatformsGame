using System.Collections;
using System;
using UnityEngine;

public static class Tween {

    /// <summary>
    /// Scales transform
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="delay"></param>
    /// <param name="duration"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="onStart"></param>
    /// <param name="onFinished"></param>
    /// <returns></returns>
    public static IEnumerator Scale(Transform transform, float delay, float duration, Vector3 from, Vector3 to, Action onStart, Action onFinished) {
        float elapsedTime = 0;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (elapsedTime < delay) {
            elapsedTime += Time.deltaTime;
            yield return wait;
        }

        if (onStart != null)
            onStart.Invoke();

        elapsedTime = 0;
        while (elapsedTime < duration) {
            transform.localScale = Vector3.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return wait;
        }

        transform.localScale = to;
        if (onFinished != null)
            onFinished.Invoke();
    }

    /// <summary>
    /// Moves transform between 2 points, keeping in account the distance transform is already to the target
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="delay"></param>
    /// <param name="duration"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="onStart"></param>
    /// <param name="onFinished"></param>
    /// <returns></returns>
    public static IEnumerator MoveBetweenRemaining(Transform transform, float delay, float duration, Vector3 from, Vector3 to, Action onStart, Action onFinished) {
        float elapsedTime = 0;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (elapsedTime < delay) {
            elapsedTime += Time.deltaTime;
            yield return wait;
        }

        elapsedTime = 0;
        if (onStart != null)
            onStart.Invoke();

        float distanceLeft = Vector3.Distance(to, transform.position);
        float totalDistance = Vector3.Distance(from, to);
        float percentage = (distanceLeft * -1 + totalDistance) / totalDistance;
        elapsedTime = duration * percentage;

        while (elapsedTime < duration) {
            transform.position = Vector3.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return wait;
        }

        transform.position = to;
        if(onFinished != null)
            onFinished.Invoke();
    }

    /// <summary>
    /// Sets transform at start and moves to target
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="delay"></param>
    /// <param name="duration"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="onStart"></param>
    /// <param name="onFinished"></param>
    /// <returns></returns>
    public static IEnumerator MoveBetween(Transform transform, float delay, float duration, Vector3 from, Vector3 to, Action onStart = null, Action onFinished = null) {
        float elapsedTime = 0;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (elapsedTime < delay) {
            elapsedTime += Time.deltaTime;
            yield return wait;
        }

        elapsedTime = 0;

        transform.position = from;
        if (onStart != null)
            onStart.Invoke();

        while (elapsedTime < duration) {
            transform.position = Vector3.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return wait;
        }

        transform.position = to;
        if (onFinished != null)
            onFinished.Invoke();
    }
}
