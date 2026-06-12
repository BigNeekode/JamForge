using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/Screen Fade")]
    public class ScreenFadeEffect : FeedbackEffect
    {
        [SerializeField] private Color fadeColor = Color.black;
        [SerializeField] private float fadeInDuration = 0.2f;
        [SerializeField] private float holdDuration = 0.1f;
        [SerializeField] private float fadeOutDuration = 0.2f;
        [SerializeField] private float targetAlpha = 0.8f;

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            ScreenFadeOverlay.Instance?.Fade(fadeColor, fadeInDuration, holdDuration, fadeOutDuration, targetAlpha);
        }
    }
}
