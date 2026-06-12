using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/Position Shake")]
    public class PositionShakeEffect : FeedbackEffect
    {
        [SerializeField] private PolishTargetMode targetMode = PolishTargetMode.ContextTarget;
        [SerializeField] private Transform specificTransform;
        [SerializeField] private float duration = 0.15f;
        [SerializeField] private float strength = 0.1f;
        [SerializeField] private float frequency = 25f;

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            Transform target = FeedbackTargetResolver.Resolve(targetMode, context, specificTransform);
            if (target == null)
            {
                Debug.LogWarning("[PolishForge] PositionShakeEffect: missing target.");
                yield break;
            }

            PositionShake shake = target.GetComponent<PositionShake>() ?? target.gameObject.AddComponent<PositionShake>();
            shake.Shake(duration, strength, frequency, UseUnscaledTime);
        }
    }
}
