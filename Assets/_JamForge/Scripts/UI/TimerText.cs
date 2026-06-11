using TMPro;
using UnityEngine;

namespace JamForge
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class TimerText : MonoBehaviour
    {
        [SerializeField] private string format = "Time: {0:0}";

        private TMP_Text label;

        private void Awake()
        {
            label = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            JamEventBus.Subscribe<TimerChangedEvent>(OnTimerChanged);
        }

        private void OnDisable()
        {
            JamEventBus.Unsubscribe<TimerChangedEvent>(OnTimerChanged);
        }

        private void OnTimerChanged(TimerChangedEvent evt)
        {
            label.text = string.Format(format, evt.TimeRemaining);
        }
    }
}
