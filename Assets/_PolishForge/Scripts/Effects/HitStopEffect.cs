using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/Hit Stop")]
    public class HitStopEffect : FeedbackEffect
    {
        [SerializeField] private float duration = 0.035f;
        [SerializeField] private float timeScaleDuringStop;
        [SerializeField] private bool useUnscaledWait = true;
        [SerializeField] private bool scaleByContextIntensity = true;

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            float finalDuration = duration * (scaleByContextIntensity ? context.EffectiveIntensity : 1f);
            HitStopController.Instance?.HitStop(finalDuration, timeScaleDuringStop, useUnscaledWait);
        }
    }
}
