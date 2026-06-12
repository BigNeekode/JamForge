using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public enum FloatingTextMode
    {
        ContextLabel,
        ContextAmount,
        FixedText
    }

    [CreateAssetMenu(menuName = "PolishForge/Effects/Floating Text")]
    public class FloatingTextEffect : FeedbackEffect
    {
        [SerializeField] private FloatingTextMode textMode = FloatingTextMode.ContextAmount;
        [SerializeField] private string fixedText = "+1";
        [SerializeField] private Color color = Color.white;
        [SerializeField] private float duration = 0.75f;
        [SerializeField] private Vector3 worldOffset = new(0f, 1f, 0f);
        [SerializeField] private float riseDistance = 1f;
        [SerializeField] private Vector3 randomSpread = new(0.2f, 0.1f, 0.2f);

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            string text = textMode switch
            {
                FloatingTextMode.ContextLabel => string.IsNullOrEmpty(context.Label) ? fixedText : context.Label,
                FloatingTextMode.ContextAmount => context.Amount.ToString(),
                FloatingTextMode.FixedText => fixedText,
                _ => fixedText
            };

            FloatingTextSpawner.Instance?.Spawn(text, context.EffectivePosition + worldOffset, color, duration, Vector3.up * riseDistance, randomSpread);
        }
    }
}
