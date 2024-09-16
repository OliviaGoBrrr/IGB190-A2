using DeveloperToolbox;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/*
 * ScreenFade.cs
 * 
 * Author: Joel Harman
 * Version: 1.0
 * Date: 26 August 2024
 * 
 * Description:
 * 
 * The `ScreenFade` class provides a simple and reusable screen fade effect in Unity, allowing you to smoothly transition 
 * between scenes or UI elements by fading the screen in or out. This effect is achieved using a `CanvasGroup` to control 
 * the screen's alpha transparency over a specified duration.
 * 
 * Key Features:
 * - Fade the screen in or out over a customizable duration.
 * - Chain fade-in and fade-out effects with a delay between them.
 * - Execute optional actions upon the completion of the fade effects.
 * - Automatically ensures a single instance of the fade effect is active and persistent across scenes.
 * 
 * How to Use:
 * 
 * 1. Ensure you have a `ScreenFadePrefab` with a `CanvasGroup` component available in your Resources folder.
 * 
 * 2. To fade the screen in (from opaque to transparent):
 *    `ScreenFade.FadeIn(float duration = 1.0f, UnityAction finishAction = null);`
 *    Example:
 *      ScreenFade.FadeIn(2.0f, OnFadeInComplete); // Fades in over 2 seconds and calls `OnFadeInComplete` when finished.
 * 
 * 3. To fade the screen out (from transparent to opaque):
 *    `ScreenFade.FadeOut(float duration = 1.0f, UnityAction finishAction = null);`
 *    Example:
 *      ScreenFade.FadeOut(1.5f); // Fades out over 1.5 seconds.
 * 
 * 4. To fade out, hold the screen fully faded, and then fade in:
 *    `ScreenFade.Fade(float fadeOutDuration = 1.0f, float fullyFadedDuration = 1.0f, float fadeInDuration = 1.0f, UnityAction fullyFadedAction = null);`
 *    Example:
 *      ScreenFade.Fade(1.0f, 2.0f, 1.0f, OnFullyFaded); // Fades out over 1 second, holds for 2 seconds, and fades in over 1 second, invoking `OnFullyFaded` after fade-out.
 * 
 * 5. To check if a fade is currently active:
 *    `ScreenFade.IsFading();`
 *    Example:
 *      if (ScreenFade.IsFading()) { }
 *
 * 6.To stop an active fade:
 *    `ScreenFade.Stop();`
 *    Example:
 *      ScreenFade.Stop(); // Immediately stops any active fade and removes the fade effect.
 */
namespace DeveloperToolbox
{
    public class ScreenFade : MonoBehaviour
    {
        public static ScreenFade Instance { get; private set; }
        public CanvasGroup canvasGroup;

        public static void FadeIn(float duration = 1.0f, UnityAction finishAction = null)
        {
            EnsureInstance();
            Instance.StartCoroutine(Instance.FadeInCoroutine(duration, finishAction));
        }

        public static void FadeOut(float duration = 1.0f, UnityAction finishAction = null)
        {
            EnsureInstance();
            Instance.StartCoroutine(Instance.FadeOutCoroutine(duration, finishAction));
        }

        public static void Fade(float fadeOutDuration = 1.0f, float fullyFadedDuration = 1.0f, float fadeInDuration = 1.0f, UnityAction fullyFadedAction = null)
        {
            EnsureInstance();
            Instance.StartCoroutine(Instance.FadeOutAndInCoroutine(fadeOutDuration, fullyFadedDuration, fadeInDuration, fullyFadedAction));
        }

        private static void EnsureInstance()
        {
            if (Instance != null) Destroy(Instance.gameObject);
            Instance = Instantiate(Resources.Load<ScreenFade>("ScreenFadePrefab"));
            DontDestroyOnLoad(Instance);
        }

        public static bool IsFading() => Instance != null;

        public static void Stop()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
                Instance = null;
            }
        }

        private IEnumerator FadeInCoroutine(float duration, UnityAction finishAction)
        {
            yield return FadeCoroutine(1.0f, 0.0f, duration);
            finishAction?.Invoke();
        }

        private IEnumerator FadeOutCoroutine(float duration, UnityAction finishAction)
        {
            yield return FadeCoroutine(0.0f, 1.0f, duration);
            finishAction?.Invoke();
        }

        private IEnumerator FadeOutAndInCoroutine(float fadeOutDuration, float fullyFadedDuration, float fadeInDuration, UnityAction fullyFadedAction)
        {
            yield return FadeOutCoroutine(fadeOutDuration, null);
            fullyFadedAction?.Invoke();
            yield return new WaitForSeconds(fullyFadedDuration);
            yield return FadeInCoroutine(fadeInDuration, null);
        }

        private IEnumerator FadeCoroutine(float fromAlpha, float toAlpha, float duration)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            canvasGroup.alpha = fromAlpha;

            while (Time.time < endTime)
            {
                float ratio = (Time.time - startTime) / duration;
                canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, ratio);
                yield return null;
            }

            canvasGroup.alpha = toAlpha;
        }
    }
}