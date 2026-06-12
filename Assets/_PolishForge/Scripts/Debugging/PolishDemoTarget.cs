using UnityEngine;

namespace PolishForge
{
    public class PolishDemoTarget : MonoBehaviour
    {
        [SerializeField] private FeedbackPreset hitFeedback;
        [SerializeField] private FeedbackPreset deathFeedback;
        [SerializeField] private int health = 3;

        private int currentHealth;

        private void Awake()
        {
            currentHealth = Mathf.Max(1, health);
        }

        private void OnMouseDown()
        {
            Hit();
        }

        public void Hit()
        {
            currentHealth--;
            FeedbackPlayer.PlayGlobal(hitFeedback, new FeedbackContext
            {
                Source = gameObject,
                Target = gameObject,
                Anchor = transform,
                Position = transform.position,
                Intensity = 1f,
                Amount = 1,
                Label = "Hit"
            });

            if (currentHealth <= 0)
            {
                FeedbackPlayer.PlayGlobal(deathFeedback, new FeedbackContext
                {
                    Source = gameObject,
                    Target = gameObject,
                    Anchor = transform,
                    Position = transform.position,
                    Intensity = 1.5f,
                    Label = "KO"
                });

                currentHealth = Mathf.Max(1, health);
            }
        }
    }
}
