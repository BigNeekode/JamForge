using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PolishForge
{
    public class ScreenFadeOverlay : PolishService<ScreenFadeOverlay>
    {
        [SerializeField] private Image image;

        protected override void Awake()
        {
            base.Awake();
            EnsureImage();
            SetColor(Color.clear);
        }

        public void Fade(Color color, float fadeIn, float hold, float fadeOut, float targetAlpha)
        {
            StartCoroutine(FadeRoutine(color, fadeIn, hold, fadeOut, targetAlpha));
        }

        private IEnumerator FadeRoutine(Color color, float fadeIn, float hold, float fadeOut, float targetAlpha)
        {
            EnsureImage();
            yield return LerpAlpha(color, 0f, targetAlpha, fadeIn);
            if (hold > 0f)
                yield return new WaitForSecondsRealtime(hold);
            yield return LerpAlpha(color, targetAlpha, 0f, fadeOut);
        }

        private IEnumerator LerpAlpha(Color color, float from, float to, float duration)
        {
            duration = Mathf.Max(0.01f, duration);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
                SetColor(new Color(color.r, color.g, color.b, alpha));
                yield return null;
            }
        }

        private void EnsureImage()
        {
            if (image != null)
                return;

            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObject = new("ScreenFadeCanvas");
                canvasObject.transform.SetParent(transform, false);
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 6000;
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            GameObject imageObject = new("Fade");
            imageObject.transform.SetParent(canvas.transform, false);
            image = imageObject.AddComponent<Image>();
            image.raycastTarget = false;
            RectTransform rect = image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void SetColor(Color color)
        {
            if (image != null)
                image.color = color;
        }
    }
}
