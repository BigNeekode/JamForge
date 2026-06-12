using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIFadeSlide : MonoBehaviour
    {
        [SerializeField] private Vector2 hiddenOffset = new(0f, -24f);
        [SerializeField] private float duration = 0.2f;

        private CanvasGroup group;
        private RectTransform rect;
        private Vector2 shownPosition;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
            rect = transform as RectTransform;
            shownPosition = rect != null ? rect.anchoredPosition : Vector2.zero;
        }

        public void Show() => StartCoroutine(Animate(true));
        public void Hide() => StartCoroutine(Animate(false));

        private IEnumerator Animate(bool show)
        {
            float elapsed = 0f;
            Vector2 fromPosition = rect != null ? rect.anchoredPosition : Vector2.zero;
            Vector2 toPosition = show ? shownPosition : shownPosition + hiddenOffset;
            float fromAlpha = group.alpha;
            float toAlpha = show ? 1f : 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                group.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                if (rect != null)
                    rect.anchoredPosition = Vector2.Lerp(fromPosition, toPosition, t);
                yield return null;
            }
        }
    }
}
