namespace JamForge
{
    public sealed class VictoryScreen : UIScreen
    {
        public void OnRetryClicked()
        {
            GameStateManager.Instance?.SetState(GameState.Playing);
            SceneLoader.ReloadCurrentScene();
        }

        public void OnMainMenuClicked()
        {
            GameStateManager.Instance?.SetState(GameState.MainMenu);
            SceneLoader.Load(JamScene.MainMenu);
        }
    }
}
