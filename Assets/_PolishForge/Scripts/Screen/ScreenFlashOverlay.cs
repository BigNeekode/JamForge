using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PolishForge
{
    public class ScreenFlashOverlay : PolishService<ScreenFlashOverlay>
    {
        [SerializeField] private Image image;

        protected override void Awake()
        {
            base.Awake();
            EnsureImage();
            SetAlpha(0f);
        }

        public void Flash(Color color, float duration, float maxAlpha, AnimationCurve curve)
        {
            if (PolishSettings.Instance != null && !PolishSettings.Instance.EnableScreenFlash)
                return;

            StartCoroutine(FlashRoutine(color, duration, maxAlpha, curve));
        }

        private IEnumerator FlashRoutine(Color color, float duration, float maxAlpha, AnimationCurve curve)
        {
            EnsureImage();
            float settingScale = PolishSettings.Instance != null
                ? PolishSettings.Instance.MasterPolishIntensity * PolishSettings.Instance.ScreenFlashIntensity
                : 1f;
            float elapsed = 0f;
            duration = Mathf.Max(0.01f, duration);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float alpha = maxAlpha * settingScale * (curve != null ? curve.Evaluate(t) : 1f - t);
                image.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }

            SetAlpha(0f);
        }

        private void EnsureImage()
        {
            if (image != null)
                return;

            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObject = new("ScreenFlashCanvas");
                canvasObject.transform.SetParent(transform, false);
                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 5000;
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            GameObject imageObject = new("Flash");
            imageObject.transform.SetParent(canvas.transform, false);
            image = imageObject.AddComponent<Image>();
            image.raycastTarget = false;
            RectTransform rect = image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void SetAlpha(float alpha)
        {
            if (image != null)
                image.color = new Color(1f, 1f, 1f, alpha);
        }
    }
}
