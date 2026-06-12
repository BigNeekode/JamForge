using UnityEngine;
using UnityEngine.InputSystem;

namespace PolishForge
{
    public class PolishDebugPanel : MonoBehaviour
    {
        [SerializeField] private FeedbackPreset previewPreset;
        [SerializeField] private bool visibleByDefault;

        private bool visible;

        private void Awake()
        {
            visible = visibleByDefault;
        }

        private void Update()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (Keyboard.current != null && Keyboard.current.f2Key.wasPressedThisFrame)
                visible = !visible;
#endif
        }

        private void OnGUI()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!visible)
                return;

            GUILayout.BeginArea(new Rect(12f, 12f, 260f, 260f), "PolishForge", GUI.skin.window);
            GUILayout.Label($"Active effects: {FeedbackPlayer.ActiveEffectCount}");

            if (PolishSettings.Instance != null)
            {
                PolishSettings.Instance.MasterPolishIntensity = GUILayout.HorizontalSlider(PolishSettings.Instance.MasterPolishIntensity, 0f, 1f);
                GUILayout.Label($"Intensity: {PolishSettings.Instance.MasterPolishIntensity:0.00}");
            }

            if (GUILayout.Button("Preview Preset") && previewPreset != null)
                FeedbackPlayer.PlayGlobal(previewPreset, FeedbackContext.From(gameObject));
            if (GUILayout.Button("Test Camera Shake"))
                PolishCameraShake.Instance?.Shake(0.2f, 0.2f);
            if (GUILayout.Button("Test Screen Flash"))
                ScreenFlashOverlay.Instance?.Flash(Color.white, 0.15f, 0.5f, null);
            if (GUILayout.Button("Test Hit Stop"))
                HitStopController.Instance?.HitStop(0.05f, 0f, true);
            if (GUILayout.Button("Test Floating Text"))
                FloatingTextSpawner.Instance?.Spawn("+10", transform.position + Vector3.up * 2f, Color.yellow, 0.75f, Vector3.up, Vector3.zero);

            GUILayout.EndArea();
#endif
        }
    }
}
