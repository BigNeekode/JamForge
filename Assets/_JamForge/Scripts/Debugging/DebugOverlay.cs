using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace JamForge
{
    public sealed class DebugOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private bool visibleByDefault;

        private float smoothedDeltaTime;
        private bool isVisible;

        private void Awake()
        {
            if (label == null)
                label = GetComponentInChildren<TMP_Text>();

            SetVisible(visibleByDefault);
        }

        private void Update()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.f1Key.wasPressedThisFrame)
                SetVisible(!isVisible);

            smoothedDeltaTime += (Time.unscaledDeltaTime - smoothedDeltaTime) * 0.1f;
            if (!isVisible || label == null)
                return;

            float fps = smoothedDeltaTime > 0f ? 1f / smoothedDeltaTime : 0f;
            string scene = SceneManager.GetActiveScene().name;
            string state = GameStateManager.Instance != null ? GameStateManager.Instance.CurrentState.ToString() : "None";
            int score = ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0;
            label.text = $"Scene: {scene}\nState: {state}\nFPS: {fps:0}\nScore: {score}\nTime Scale: {Time.timeScale:0.00}";
#endif
        }

        private void SetVisible(bool visible)
        {
            isVisible = visible;
            if (label != null)
                label.enabled = visible;
        }
    }
}
