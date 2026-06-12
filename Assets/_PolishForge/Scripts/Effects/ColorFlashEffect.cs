using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/Color Flash")]
    public class ColorFlashEffect : FeedbackEffect
    {
        [SerializeField] private PolishTargetMode targetMode = PolishTargetMode.ContextTarget;
        [SerializeField] private Transform specificTransform;
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float duration = 0.08f;
        [SerializeField] private bool includeChildren = true;

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            Transform target = FeedbackTargetResolver.Resolve(targetMode, context, specificTransform);
            if (target == null)
            {
                Debug.LogWarning("[PolishForge] ColorFlashEffect: missing target.");
                yield break;
            }

            ColorFlasher flasher = target.GetComponent<ColorFlasher>() ?? target.gameObject.AddComponent<ColorFlasher>();
            flasher.Flash(flashColor, duration, includeChildren);
        }
    }
}
