using UnityEngine;

namespace JamForge
{
    public sealed class Projectile : Poolable
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private int damage = 1;

        private float lifeTimer;

        public override void OnSpawned()
        {
            lifeTimer = lifetime;
        }

        private void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            lifeTimer -= Time.deltaTime;

            if (lifeTimer <= 0f)
                DespawnSelf();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Health health))
                health.Damage(damage);

            DespawnSelf();
        }
    }
}
