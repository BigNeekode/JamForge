using System;
using UnityEngine;

namespace JamForge
{
    public sealed class JamTimer : MonoBehaviour
    {
        [SerializeField] private bool useUnscaledTime;
        [SerializeField] private bool gameOverWhenFinished;
        [SerializeField] private bool victoryWhenFinished;

        public float TimeRemaining { get; private set; }
        public bool IsRunning { get; private set; }

        public event Action<float> OnTimeChanged;
        public event Action OnTimerFinished;

        private void Update()
        {
            if (!IsRunning)
                return;

            float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            TimeRemaining = Mathf.Max(0f, TimeRemaining - delta);
            OnTimeChanged?.Invoke(TimeRemaining);
            JamEventBus.Raise(new TimerChangedEvent(TimeRemaining));

            if (TimeRemaining <= 0f)
            {
                IsRunning = false;
                OnTimerFinished?.Invoke();
                JamEventBus.Raise(new TimerFinishedEvent(this));

                if (gameOverWhenFinished)
                    GameStateManager.Instance?.SetState(GameState.GameOver);
                if (victoryWhenFinished)
                    GameStateManager.Instance?.SetState(GameState.Victory);
            }
        }

        public void StartTimer(float seconds)
        {
            TimeRemaining = Mathf.Max(0f, seconds);
            IsRunning = true;
            OnTimeChanged?.Invoke(TimeRemaining);
            JamEventBus.Raise(new TimerChangedEvent(TimeRemaining));
        }

        public void StopTimer()
        {
            IsRunning = false;
        }
    }

    public readonly struct TimerChangedEvent
    {
        public readonly float TimeRemaining;
        public TimerChangedEvent(float timeRemaining) => TimeRemaining = timeRemaining;
    }

    public readonly struct TimerFinishedEvent
    {
        public readonly JamTimer Timer;
        public TimerFinishedEvent(JamTimer timer) => Timer = timer;
    }
}
