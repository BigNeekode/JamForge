using UnityEngine;

namespace PolishForge
{
    public struct FeedbackContext
    {
        public GameObject Source;
        public GameObject Target;
        public Vector3 Position;
        public Transform Anchor;
        public float Intensity;
        public string Label;
        public int Amount;

        public static FeedbackContext From(GameObject source)
        {
            return new FeedbackContext
            {
                Source = source,
                Target = source,
                Anchor = source != null ? source.transform : null,
                Position = source != null ? source.transform.position : Vector3.zero,
                Intensity = 1f
            };
        }

        public readonly float EffectiveIntensity => Intensity > 0f ? Intensity : 1f;
        public readonly Vector3 EffectivePosition => Anchor != null ? Anchor.position : Position;
    }
}
