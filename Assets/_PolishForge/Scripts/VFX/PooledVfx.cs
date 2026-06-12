using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public class PooledVfx : MonoBehaviour
    {
        private VfxSpawner owner;
        private Coroutine despawnRoutine;
        private bool destroyOnDespawn;

        public GameObject PrefabKey { get; private set; }

        public void Initialize(VfxSpawner spawner, bool destroyWhenDone = false)
        {
            owner = spawner;
            destroyOnDespawn = destroyWhenDone;
        }

        public void SetPrefabKey(GameObject prefabKey)
        {
            PrefabKey = prefabKey;
        }

        public void Play(float lifetime)
        {
            if (despawnRoutine != null)
                StopCoroutine(despawnRoutine);

            despawnRoutine = StartCoroutine(DespawnAfter(lifetime));
        }

        private IEnumerator DespawnAfter(float lifetime)
        {
            yield return new WaitForSeconds(Mathf.Max(0.01f, lifetime));
            if (owner != null)
                owner.Despawn(this);
            else
                Destroy(gameObject);
        }

        public bool ShouldDestroyOnDespawn => destroyOnDespawn;
    }
}
