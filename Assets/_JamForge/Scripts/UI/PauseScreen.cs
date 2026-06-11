namespace JamForge
{
    public sealed class PauseScreen : UIScreen
    {
        public void OnResumeClicked() => GameStateManager.Instance?.SetState(GameState.Playing);
        public void OnRestartClicked() => SceneLoader.ReloadCurrentScene();

        public void OnMainMenuClicked()
        {
            GameStateManager.Instance?.SetState(GameState.MainMenu);
            SceneLoader.Load(JamScene.MainMenu);
        }
    }
}
