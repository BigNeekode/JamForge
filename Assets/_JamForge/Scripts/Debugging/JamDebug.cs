using UnityEngine;
using UnityEngine.InputSystem;

namespace JamForge
{
    public sealed class JamDebug : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
                return;

            if (keyboard.f5Key.wasPressedThisFrame)
                SceneLoader.ReloadCurrentScene();

            if (keyboard.f6Key.wasPressedThisFrame)
                GameStateManager.Instance?.SetState(GameState.Victory);

            if (keyboard.f7Key.wasPressedThisFrame)
                GameStateManager.Instance?.SetState(GameState.GameOver);

            if (keyboard.f8Key.wasPressedThisFrame)
                ScoreManager.Instance?.Add(1);

            if (keyboard.f9Key.wasPressedThisFrame)
                Time.timeScale = Mathf.Approximately(Time.timeScale, 0f) ? 1f : 0f;
        }
#endif
    }
}
