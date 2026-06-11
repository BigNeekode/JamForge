using UnityEngine;

namespace JamForge
{
    public sealed class SampleGameController : MonoBehaviour
    {
        [SerializeField] private float gameDuration = 60f;
        [SerializeField] private JamTimer timer;

        private void Start()
        {
            ScoreManager.Instance?.ResetScore();
            GameStateManager.Instance?.SetState(GameState.Playing);

            if (timer != null)
                timer.StartTimer(gameDuration);
        }
    }
}
