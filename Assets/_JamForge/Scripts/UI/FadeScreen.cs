using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JamForge
{
    public sealed class FadeScreen : UIScreen
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image fadeImage;
        [SerializeField] private float defaultDuration = 0.25f;

        private Coroutine routine;

        protected override void Awake()
        {
            base.Awake();

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            if (fadeImage == null)
                fadeImage = GetComponentInChildren<Image>();
        }

        public void FadeIn() => FadeTo(0f, defaultDuration);
        public void FadeOut() => FadeTo(1f, defaultDuration);

        public void FadeTo(float alpha, float duration)
        {
            if (canvasGroup == null)
                return;

            if (routine != null)
                StopCoroutine(routine);

            routine = StartCoroutine(FadeRoutine(Mathf.Clamp01(alpha), Mathf.Max(0.01f, duration)));
        }

        private IEnumerator FadeRoutine(float target, float duration)
        {
            Show();
            float start = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = target;
            if (Mathf.Approximately(target, 0f))
                Hide();
        }
    }
}
