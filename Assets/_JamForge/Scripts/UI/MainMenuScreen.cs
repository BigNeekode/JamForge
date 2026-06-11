namespace JamForge
{
    public sealed class MainMenuScreen : UIScreen
    {
        public void OnPlayClicked()
        {
            GameStateManager.Instance?.SetState(GameState.Playing);
            SceneLoader.Load(JamScene.Game);
        }

        public void OnSettingsClicked() => UIManager.Instance?.Show<SettingsScreen>();
        public void OnQuitClicked() => SceneLoader.QuitGame();
    }
}
