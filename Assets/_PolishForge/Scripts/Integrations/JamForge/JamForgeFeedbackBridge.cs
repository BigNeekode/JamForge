using UnityEngine;

namespace PolishForge.JamForgeIntegration
{
    public class JamForgeFeedbackBridge : MonoBehaviour
    {
        [SerializeField] private FeedbackPreset healthChangedFeedback;
        [SerializeField] private FeedbackPreset deathFeedback;
        [SerializeField] private FeedbackPreset scoreChangedFeedback;
        [SerializeField] private FeedbackPreset victoryFeedback;
        [SerializeField] private FeedbackPreset gameOverFeedback;

        private void OnEnable()
        {
            JamForge.JamEventBus.Subscribe<JamForge.HealthChangedEvent>(OnHealthChanged);
            JamForge.JamEventBus.Subscribe<JamForge.DeathEvent>(OnDeath);
            JamForge.JamEventBus.Subscribe<JamForge.ScoreChangedEvent>(OnScoreChanged);
            JamForge.JamEventBus.Subscribe<JamForge.GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            JamForge.JamEventBus.Unsubscribe<JamForge.HealthChangedEvent>(OnHealthChanged);
            JamForge.JamEventBus.Unsubscribe<JamForge.DeathEvent>(OnDeath);
            JamForge.JamEventBus.Unsubscribe<JamForge.ScoreChangedEvent>(OnScoreChanged);
            JamForge.JamEventBus.Unsubscribe<JamForge.GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnHealthChanged(JamForge.HealthChangedEvent evt)
        {
            if (healthChangedFeedback == null || evt.Health == null)
                return;

            FeedbackPlayer.PlayGlobal(healthChangedFeedback, new FeedbackContext
            {
                Target = evt.Health.gameObject,
                Position = evt.Health.transform.position,
                Anchor = evt.Health.transform,
                Intensity = 1f,
                Amount = evt.Current
            });
        }

        private void OnDeath(JamForge.DeathEvent evt)
        {
            if (deathFeedback == null || evt.Health == null)
                return;

            FeedbackPlayer.PlayGlobal(deathFeedback, new FeedbackContext
            {
                Target = evt.Health.gameObject,
                Position = evt.Health.transform.position,
                Anchor = evt.Health.transform,
                Intensity = 1f
            });
        }

        private void OnScoreChanged(JamForge.ScoreChangedEvent evt)
        {
            if (scoreChangedFeedback == null)
                return;

            FeedbackPlayer.PlayGlobal(scoreChangedFeedback, new FeedbackContext
            {
                Source = gameObject,
                Position = transform.position,
                Intensity = 1f,
                Amount = evt.Score
            });
        }

        private void OnGameStateChanged(JamForge.GameStateChangedEvent evt)
        {
            if (evt.Current == JamForge.GameState.Victory && victoryFeedback != null)
                FeedbackPlayer.PlayGlobal(victoryFeedback, FeedbackContext.From(gameObject));

            if (evt.Current == JamForge.GameState.GameOver && gameOverFeedback != null)
                FeedbackPlayer.PlayGlobal(gameOverFeedback, FeedbackContext.From(gameObject));
        }
    }
}
