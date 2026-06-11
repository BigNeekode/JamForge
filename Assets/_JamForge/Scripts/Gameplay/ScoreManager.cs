using System;
using UnityEngine;

namespace JamForge
{
    public sealed class ScoreManager : PersistentSingleton<ScoreManager>
    {
        public int Score { get; private set; }
        public event Action<int> OnScoreChanged;

        public void Add(int amount) => Set(Score + amount);

        public void Set(int value)
        {
            Score = Mathf.Max(0, value);
            OnScoreChanged?.Invoke(Score);
            JamEventBus.Raise(new ScoreChangedEvent(Score));
        }

        public void ResetScore() => Set(0);
    }

    public readonly struct ScoreChangedEvent
    {
        public readonly int Score;
        public ScoreChangedEvent(int score) => Score = score;
    }
}
