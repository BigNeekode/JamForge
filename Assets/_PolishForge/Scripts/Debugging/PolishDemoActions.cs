using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace PolishForge
{
    public class PolishDemoActions : MonoBehaviour
    {
        [SerializeField] private PolishDemoTarget target;
        [SerializeField] private FeedbackTrigger pickupTrigger;
        [SerializeField] private FeedbackPreset victoryFeedback;
        [SerializeField] private FeedbackPreset gameOverFeedback;

        private void Awake()
        {
            RepairInputModule();
        }

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

        private static void RepairInputModule()
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem == null)
                return;

            foreach (StandaloneInputModule oldModule in eventSystem.GetComponents<StandaloneInputModule>())
            {
                oldModule.enabled = false;
                Destroy(oldModule);
            }

            if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
        }
    }
}
