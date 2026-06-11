using UnityEngine;

namespace JamForge
{
    [RequireComponent(typeof(Health))]
    public sealed class AwardScoreOnDeath : MonoBehaviour
    {
        [SerializeField] private int scoreAmount = 5;
        [SerializeField] private bool destroyOnDeath = true;

        private Health health;

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            health.OnDied += HandleDeath;
        }

        private void OnDisable()
        {
            health.OnDied -= HandleDeath;
        }

        private void HandleDeath()
        {
            ScoreManager.Instance?.Add(scoreAmount);

            if (destroyOnDeath)
                Destroy(gameObject);
        }
    }
}
