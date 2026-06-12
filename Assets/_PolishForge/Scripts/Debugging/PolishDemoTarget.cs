using UnityEngine;
using UnityEngine.InputSystem;

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

    public class PolishDemoClicker : MonoBehaviour
    {
        [SerializeField] private Camera raycastCamera;

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
                return;

            Camera cameraToUse = raycastCamera != null ? raycastCamera : Camera.main;
            if (cameraToUse == null)
                return;

            Ray ray = cameraToUse.ScreenPointToRay(mouse.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.TryGetComponent(out PolishDemoTarget target))
                target.Hit();
        }
    }
}
