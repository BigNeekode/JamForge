using System;
using UnityEngine;

namespace JamForge
{
    public enum GameState
    {
        Booting,
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public sealed class GameStateManager : PersistentSingleton<GameStateManager>
    {
        [SerializeField] private bool controlTimeScale = true;

        public GameState CurrentState { get; private set; } = GameState.Booting;
        public event Action<GameState, GameState> OnStateChanged;

        public void SetState(GameState newState)
        {
            if (CurrentState == newState)
                return;

            GameState previous = CurrentState;
            CurrentState = newState;

            ApplyTimeScale(newState);
            OnStateChanged?.Invoke(previous, newState);
            JamEventBus.Raise(new GameStateChangedEvent(previous, newState));
        }

        public bool Is(GameState state) => CurrentState == state;

        private void ApplyTimeScale(GameState state)
        {
            if (!controlTimeScale)
                return;

            Time.timeScale = state switch
            {
                GameState.Paused => 0f,
                GameState.GameOver => 0f,
                GameState.Victory => 0f,
                _ => 1f
            };
        }
    }

    public readonly struct GameStateChangedEvent
    {
        public readonly GameState Previous;
        public readonly GameState Current;

        public GameStateChangedEvent(GameState previous, GameState current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
