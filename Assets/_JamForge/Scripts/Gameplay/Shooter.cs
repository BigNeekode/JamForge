using UnityEngine;

namespace JamForge
{
    public sealed class Shooter : MonoBehaviour
    {
        [SerializeField] private ObjectPool projectilePool;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float cooldown = 0.15f;

        private float nextFireTime;

        private void OnValidate()
        {
            cooldown = Mathf.Max(0f, cooldown);

            if (projectilePool == null)
                Debug.LogWarning("Shooter needs a projectile ObjectPool reference.", this);
        }

        private void OnEnable()
        {
            if (InputReader.Instance != null)
                InputReader.Instance.OnAttack += TryShoot;
        }

        private void OnDisable()
        {
            if (InputReader.Instance != null)
                InputReader.Instance.OnAttack -= TryShoot;
        }

        public void TryShoot()
        {
            if (Time.time < nextFireTime)
                return;

            if (projectilePool == null)
            {
                Debug.LogWarning("Shooter is missing projectile pool.", this);
                return;
            }

            Transform origin = firePoint != null ? firePoint : transform;
            projectilePool.Spawn(origin.position, origin.rotation);
            nextFireTime = Time.time + cooldown;
        }
    }
}
