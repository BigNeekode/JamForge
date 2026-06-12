using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public class ScalePunch : MonoBehaviour
    {
        private Coroutine activeRoutine;
        private Vector3 originalScale;

        public void Punch(Vector3 punchScale, float duration, AnimationCurve curve, bool useUnscaledTime)
        {
            if (activeRoutine != null)
                StopCoroutine(activeRoutine);

            activeRoutine = StartCoroutine(PunchRoutine(punchScale, duration, curve, useUnscaledTime));
        }

        private IEnumerator PunchRoutine(Vector3 punchScale, float duration, AnimationCurve curve, bool useUnscaledTime)
        {
            originalScale = transform.localScale;
            duration = Mathf.Max(0.01f, duration);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float value = curve != null ? curve.Evaluate(t) : Mathf.Sin(t * Mathf.PI);
                transform.localScale = Vector3.LerpUnclamped(originalScale, Vector3.Scale(originalScale, punchScale), value);
                yield return null;
            }

            transform.localScale = originalScale;
            activeRoutine = null;
        }
    }
}
