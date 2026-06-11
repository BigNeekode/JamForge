using TMPro;
using UnityEngine;

namespace JamForge
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class ScoreText : MonoBehaviour
    {
        [SerializeField] private string format = "Score: {0}";

        private TMP_Text label;

        private void Awake()
        {
            label = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            JamEventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
            OnScoreChanged(new ScoreChangedEvent(ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0));
        }

        private void OnDisable()
        {
            JamEventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        }

        private void OnScoreChanged(ScoreChangedEvent evt)
        {
            label.text = string.Format(format, evt.Score);
        }
    }
}
