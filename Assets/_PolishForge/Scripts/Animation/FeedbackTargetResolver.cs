using UnityEngine;

namespace PolishForge
{
    public static class FeedbackTargetResolver
    {
        public static Transform Resolve(PolishTargetMode mode, FeedbackContext context, Transform specific)
        {
            return mode switch
            {
                PolishTargetMode.ContextTarget => context.Target != null ? context.Target.transform : null,
                PolishTargetMode.ContextSource => context.Source != null ? context.Source.transform : null,
                PolishTargetMode.Anchor => context.Anchor,
                PolishTargetMode.SpecificTransform => specific,
                _ => null
            };
        }
    }
}
