using System.Collections.Generic;
using UnityEngine;

namespace JamForge
{
    public sealed class UIManager : PersistentSingleton<UIManager>
    {
        [SerializeField] private List<UIScreen> screens = new();

        private readonly Dictionary<System.Type, UIScreen> screenMap = new();

        protected override void OnAwakeSingleton()
        {
            RebuildScreenMap();
            JamEventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        protected override void OnDestroy()
        {
            JamEventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
            base.OnDestroy();
        }

        public void Register(UIScreen screen)
        {
            if (screen == null)
                return;

            if (!screens.Contains(screen))
                screens.Add(screen);

            screenMap[screen.GetType()] = screen;
        }

        public T Get<T>() where T : UIScreen
        {
            if (screenMap.TryGetValue(typeof(T), out UIScreen screen))
                return screen as T;

            Debug.LogWarning($"UI screen not found: {typeof(T).Name}");
            return null;
        }

        public void Show<T>() where T : UIScreen => Get<T>()?.Show();
        public void Hide<T>() where T : UIScreen => Get<T>()?.Hide();

        private void RebuildScreenMap()
        {
            screenMap.Clear();
            foreach (UIScreen screen in screens)
            {
                if (screen != null)
                    screenMap[screen.GetType()] = screen;
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            SetScreenVisible<PauseScreen>(evt.Current == GameState.Paused);
            SetScreenVisible<GameOverScreen>(evt.Current == GameState.GameOver);
            SetScreenVisible<VictoryScreen>(evt.Current == GameState.Victory);
            SetScreenVisible<HUDScreen>(evt.Current == GameState.Playing);
        }

        private void SetScreenVisible<T>(bool visible) where T : UIScreen
        {
            if (!screenMap.TryGetValue(typeof(T), out UIScreen screen) || screen == null)
                return;

            if (visible)
                screen.Show();
            else
                screen.Hide();
        }
    }
}
