using System.Collections;
using TMPro;
using UnityEngine;

namespace PolishForge
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;

        private FloatingTextSpawner owner;
        private Coroutine activeRoutine;

        public void Initialize(FloatingTextSpawner spawner)
        {
            owner = spawner;
            if (label == null)
                label = GetComponentInChildren<TMP_Text>();
        }

        public void Show(string text, Color color, float duration, Vector3 rise)
        {
            if (label == null)
                label = GetComponentInChildren<TMP_Text>();

            if (label != null)
            {
                label.text = text;
                label.color = color;
            }

            if (activeRoutine != null)
                StopCoroutine(activeRoutine);

            activeRoutine = StartCoroutine(ShowRoutine(color, duration, rise));
        }

        private IEnumerator ShowRoutine(Color color, float duration, Vector3 rise)
        {
            Vector3 start = transform.position;
            float elapsed = 0f;
            duration = Mathf.Max(0.01f, duration);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transform.position = Vector3.Lerp(start, start + rise, t);
                if (label != null)
                    label.color = new Color(color.r, color.g, color.b, 1f - t);
                yield return null;
            }

            owner?.Despawn(this);
        }
    }
}
