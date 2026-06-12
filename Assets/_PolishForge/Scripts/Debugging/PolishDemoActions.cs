using UnityEngine;

namespace PolishForge
{
    public class PolishDemoActions : MonoBehaviour
    {
        [SerializeField] private PolishDemoTarget target;
        [SerializeField] private FeedbackTrigger pickupTrigger;
        [SerializeField] private FeedbackPreset victoryFeedback;
        [SerializeField] private FeedbackPreset gameOverFeedback;

        public void HitTarget()
        {
            target?.Hit();
        }

        public void TriggerPickup()
        {
            pickupTrigger?.Play();
        }

        public void TriggerVictory()
        {
            FeedbackPlayer.PlayGlobal(victoryFeedback, FeedbackContext.From(gameObject));
        }

        public void TriggerGameOver()
        {
            FeedbackPlayer.PlayGlobal(gameOverFeedback, FeedbackContext.From(gameObject));
        }

        public void SetCameraShakeIntensity(float value)
        {
            if (PolishSettings.Instance == null)
                return;

            PolishSettings.Instance.CameraShakeIntensity = Mathf.Clamp01(value);
            PolishSettings.Instance.Save();
        }

        public void SetScreenFlashIntensity(float value)
        {
            if (PolishSettings.Instance == null)
                return;

            PolishSettings.Instance.ScreenFlashIntensity = Mathf.Clamp01(value);
            PolishSettings.Instance.Save();
        }

        public void SetRumbleIntensity(float value)
        {
            if (PolishSettings.Instance == null)
                return;

            PolishSettings.Instance.RumbleIntensity = Mathf.Clamp01(value);
            PolishSettings.Instance.Save();
        }
    }
}
