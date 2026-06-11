using System;
using UnityEngine;

namespace JamForge
{
    public sealed class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private bool gameOverOnDeath;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsDead => CurrentHealth <= 0;

        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        private void Awake()
        {
            maxHealth = Mathf.Max(1, maxHealth);
            CurrentHealth = maxHealth;
        }

        private void OnValidate()
        {
            if (maxHealth < 1)
            {
                Debug.LogWarning("Health maxHealth must be at least 1. Clamping to 1.", this);
                maxHealth = 1;
            }
        }

        public void Damage(int amount)
        {
            if (IsDead || amount <= 0)
                return;

            SetHealth(CurrentHealth - amount);

            if (CurrentHealth == 0)
                Die();
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead)
                return;

            SetHealth(CurrentHealth + amount);
        }

        public void Kill()
        {
            if (IsDead)
                return;

            SetHealth(0);
            Die();
        }

        public void ResetHealth()
        {
            SetHealth(maxHealth);
        }

        private void SetHealth(int value)
        {
            CurrentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
            JamEventBus.Raise(new HealthChangedEvent(this, CurrentHealth, maxHealth));
        }

        private void Die()
        {
            OnDied?.Invoke();
            JamEventBus.Raise(new DeathEvent(this));

            if (gameOverOnDeath)
                GameStateManager.Instance?.SetState(GameState.GameOver);
        }
    }

    public readonly struct HealthChangedEvent
    {
        public readonly Health Health;
        public readonly int Current;
        public readonly int Max;

        public HealthChangedEvent(Health health, int current, int max)
        {
            Health = health;
            Current = current;
            Max = max;
        }
    }

    public readonly struct DeathEvent
    {
        public readonly Health Health;

        public DeathEvent(Health health)
        {
            Health = health;
        }
    }
}
