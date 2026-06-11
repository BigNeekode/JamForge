using System;
using UnityEngine;

namespace JamForge
{
    public sealed class Health : MonoBehaviour, Damageable
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
            CurrentHealth = Mathf.Max(1, maxHealth);
        }

        public void Damage(int amount)
        {
            if (IsDead || amount <= 0)
                return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
            JamEventBus.Raise(new HealthChangedEvent(this, CurrentHealth, maxHealth));

            if (CurrentHealth == 0)
            {
                OnDied?.Invoke();
                JamEventBus.Raise(new DeathEvent(this));
                if (gameOverOnDeath)
                    GameStateManager.Instance?.SetState(GameState.GameOver);
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead)
                return;

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
            JamEventBus.Raise(new HealthChangedEvent(this, CurrentHealth, maxHealth));
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
