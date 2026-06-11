using UnityEngine;

namespace JamForge
{
    public sealed class JamBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameObject jamCorePrefab;
        [SerializeField] private bool loadMainMenuOnStart = true;

        private void Start()
        {
            JamRuntime.EnsureCoreExists(jamCorePrefab);

            GameStateManager.Instance?.SetState(GameState.MainMenu);

            if (loadMainMenuOnStart)
                SceneLoader.Load(JamScene.MainMenu);
        }
    }
}
