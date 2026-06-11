using UnityEngine;

namespace JamForge
{
    public class Poolable : MonoBehaviour
    {
        public ObjectPool OwningPool { get; private set; }

        public void SetOwningPool(ObjectPool pool)
        {
            OwningPool = pool;
        }

        public virtual void OnSpawned()
        {
        }

        public virtual void OnDespawned()
        {
        }

        protected void DespawnSelf()
        {
            if (OwningPool != null)
                OwningPool.Despawn(this);
            else
                gameObject.SetActive(false);
        }
    }
}
