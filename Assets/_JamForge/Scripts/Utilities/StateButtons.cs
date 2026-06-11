namespace JamForge
{
    public sealed class StateButtons : UnityEngine.MonoBehaviour
    {
        public void SetPlaying() => GameStateManager.Instance?.SetState(GameState.Playing);
        public void SetPaused() => GameStateManager.Instance?.SetState(GameState.Paused);
        public void SetGameOver() => GameStateManager.Instance?.SetState(GameState.GameOver);
        public void SetVictory() => GameStateManager.Instance?.SetState(GameState.Victory);
        public void ReloadCurrentScene() => SceneLoader.ReloadCurrentScene();
        public void LoadMainMenu() => SceneLoader.Load(JamScene.MainMenu);
        public void LoadGame() => SceneLoader.Load(JamScene.Game);
        public void QuitGame() => SceneLoader.QuitGame();
    }
}
