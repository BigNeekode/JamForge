using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/Scale Punch")]
    public class ScalePunchEffect : FeedbackEffect
    {
        [SerializeField] private PolishTargetMode targetMode = PolishTargetMode.ContextTarget;
        [SerializeField] private Transform specificTransform;
        [SerializeField] private Vector3 punchScale = new(1.15f, 1.15f, 1.15f);
        [SerializeField] private float duration = 0.15f;
        [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 0.5f, 1f);

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            Transform target = FeedbackTargetResolver.Resolve(targetMode, context, specificTransform);
            if (target == null)
            {
                Debug.LogWarning("[PolishForge] ScalePunchEffect: missing target.");
                yield break;
            }

            ScalePunch punch = target.GetComponent<ScalePunch>() ?? target.gameObject.AddComponent<ScalePunch>();
            punch.Punch(punchScale, duration, curve, UseUnscaledTime);
        }
    }
}
