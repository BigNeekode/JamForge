using UnityEngine;

namespace JamForge
{
    public sealed class DamageDealer : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private bool destroyAfterHit;

        private void OnTriggerEnter(Collider other)
        {
            TryDamage(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryDamage(other.gameObject);
        }

        private void TryDamage(GameObject target)
        {
            if (target.TryGetComponent(out Health health))
                health.Damage(damage);

            if (destroyAfterHit)
                Destroy(gameObject);
        }
    }
}
