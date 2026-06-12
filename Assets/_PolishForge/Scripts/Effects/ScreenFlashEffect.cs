using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/Screen Flash")]
    public class ScreenFlashEffect : FeedbackEffect
    {
        [SerializeField] private Color color = Color.white;
        [SerializeField] private float duration = 0.12f;
        [SerializeField] private float maxAlpha = 0.45f;
        [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            ScreenFlashOverlay.Instance?.Flash(color, duration, maxAlpha, curve);
        }
    }
}
