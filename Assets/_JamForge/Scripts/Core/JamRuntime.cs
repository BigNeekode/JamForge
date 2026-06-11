using UnityEngine;
using UnityEngine.SceneManagement;

namespace JamForge
{
    public static class JamRuntime
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad()
        {
            JamEventBus.Clear();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            EnsureCoreExists(null);
        }

        public static void EnsureCoreExists(GameObject jamCorePrefab)
        {
            if (GameStateManager.Instance != null &&
                SettingsManager.Instance != null &&
                InputReader.Instance != null &&
                AudioManager.Instance != null &&
                UIManager.Instance != null)
            {
                return;
            }

            if (jamCorePrefab != null && GameStateManager.Instance == null)
            {
                Object.Instantiate(jamCorePrefab);
                return;
            }

            GameObject core = GameObject.Find("JamCore");
            if (core == null)
                core = new GameObject("JamCore");

            if (core.GetComponent<GameStateManager>() == null)
                core.AddComponent<GameStateManager>();
            if (core.GetComponent<SettingsManager>() == null)
                core.AddComponent<SettingsManager>();
            if (core.GetComponent<InputReader>() == null)
                core.AddComponent<InputReader>();
            if (core.GetComponent<AudioManager>() == null)
                core.AddComponent<AudioManager>();
            if (core.GetComponent<UIManager>() == null)
                core.AddComponent<UIManager>();
            if (core.GetComponent<ScoreManager>() == null)
                core.AddComponent<ScoreManager>();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (GameStateManager.Instance == null)
                return;

            if (scene.name == SceneLoader.GetSceneName(JamScene.MainMenu))
                GameStateManager.Instance.SetState(GameState.MainMenu);
            else if (scene.name == SceneLoader.GetSceneName(JamScene.Game))
                GameStateManager.Instance.SetState(GameState.Playing);
            else if (scene.name == SceneLoader.GetSceneName(JamScene.GameOver))
                GameStateManager.Instance.SetState(GameState.GameOver);
        }
    }
}
