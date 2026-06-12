using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/Time Scale Curve")]
    public class TimeScaleCurveEffect : FeedbackEffect
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private AnimationCurve timeScaleCurve = AnimationCurve.EaseInOut(0f, 0.25f, 1f, 1f);
        [SerializeField] private bool restorePreviousTimeScale = true;

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();

            if (PolishSettings.Instance != null && !PolishSettings.Instance.EnableSlowMotion)
                yield break;

            float previous = Time.timeScale;
            float elapsed = 0f;
            duration = Mathf.Max(0.01f, duration);

            while (elapsed < duration)
            {
                elapsed += UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Time.timeScale = Mathf.Max(0f, timeScaleCurve.Evaluate(t));
                yield return null;
            }

            if (restorePreviousTimeScale)
                Time.timeScale = previous;
        }
    }
}
