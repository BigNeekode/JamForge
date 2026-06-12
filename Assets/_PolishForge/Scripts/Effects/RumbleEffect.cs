using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/Rumble")]
    public class RumbleEffect : FeedbackEffect
    {
        [SerializeField] private float lowFrequency = 0.2f;
        [SerializeField] private float highFrequency = 0.6f;
        [SerializeField] private float duration = 0.12f;
        [SerializeField] private bool scaleByContextIntensity = true;

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            float scale = scaleByContextIntensity ? context.EffectiveIntensity : 1f;
            RumbleController.Instance?.Rumble(lowFrequency * scale, highFrequency * scale, duration);
        }
    }
}
