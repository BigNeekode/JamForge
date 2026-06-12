using UnityEngine;

namespace PolishForge
{
    public class FeedbackTrigger : MonoBehaviour
    {
        [SerializeField] private FeedbackPreset preset;
        [SerializeField] private Transform anchor;
        [SerializeField] private float intensity = 1f;

        public void Play()
        {
            FeedbackPlayer.PlayGlobal(preset, CreateContext(gameObject, transform.position));
        }

        public void PlayAtPosition(Vector3 position)
        {
            FeedbackPlayer.PlayGlobal(preset, CreateContext(gameObject, position));
        }

        public void PlayWithTarget(GameObject target)
        {
            FeedbackContext context = CreateContext(target, target != null ? target.transform.position : transform.position);
            FeedbackPlayer.PlayGlobal(preset, context);
        }

        private FeedbackContext CreateContext(GameObject target, Vector3 position)
        {
            Transform contextAnchor = anchor != null ? anchor : transform;
            return new FeedbackContext
            {
                Source = gameObject,
                Target = target,
                Anchor = contextAnchor,
                Position = position,
                Intensity = intensity
            };
        }
    }
}
