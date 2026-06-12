using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/Camera Shake")]
    public class CameraShakeEffect : FeedbackEffect
    {
        [SerializeField] private float duration = 0.15f;
        [SerializeField] private float strength = 0.15f;
        [SerializeField] private float frequency = 25f;
        [SerializeField] private bool scaleByContextIntensity = true;

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            float finalStrength = strength * (scaleByContextIntensity ? context.EffectiveIntensity : 1f);
            PolishCameraShake.Instance?.Shake(duration, finalStrength, frequency);
        }
    }
}
