using System.Collections.Generic;
using UnityEngine;

namespace PolishForge
{
    public class VfxSpawner : PolishService<VfxSpawner>
    {
        private readonly Dictionary<GameObject, Queue<PooledVfx>> pools = new();

        public void Spawn(VfxCue cue, FeedbackContext context)
        {
            if (cue == null || cue.Prefab == null)
            {
                Debug.LogWarning("[PolishForge] VfxSpawner: missing VFX cue or prefab.");
                return;
            }

            Transform attach = cue.AttachToAnchor ? context.Anchor : null;
            if (attach == null && cue.AttachToAnchor && context.Target != null)
                attach = context.Target.transform;

            Vector3 position = (attach != null ? attach.position : context.EffectivePosition) + cue.Offset;
            GameObject instance;
            PooledVfx pooled = null;

            if (cue.UsePooling)
            {
                pooled = GetPooled(cue.Prefab);
                instance = pooled.gameObject;
            }
            else
            {
                instance = Instantiate(cue.Prefab);
                pooled = instance.GetComponent<PooledVfx>() ?? instance.AddComponent<PooledVfx>();
                pooled.Initialize(this, true);
            }

            instance.transform.SetParent(attach, true);
            instance.transform.SetPositionAndRotation(position, Quaternion.identity);
            instance.SetActive(true);
            pooled.Play(cue.Lifetime);
        }

        public void Despawn(PooledVfx vfx)
        {
            if (vfx == null)
                return;

            if (vfx.ShouldDestroyOnDespawn)
            {
                Destroy(vfx.gameObject);
                return;
            }

            vfx.transform.SetParent(transform, false);
            vfx.gameObject.SetActive(false);

            if (vfx.PrefabKey != null && pools.TryGetValue(vfx.PrefabKey, out Queue<PooledVfx> pool) && !pool.Contains(vfx))
                pool.Enqueue(vfx);
        }

        private PooledVfx GetPooled(GameObject prefab)
        {
            if (!pools.TryGetValue(prefab, out Queue<PooledVfx> pool))
            {
                pool = new Queue<PooledVfx>();
                pools[prefab] = pool;
            }

            while (pool.Count > 0)
            {
                PooledVfx item = pool.Dequeue();
                if (item != null)
                    return item;
            }

            GameObject created = Instantiate(prefab, transform);
            PooledVfx pooled = created.GetComponent<PooledVfx>() ?? created.AddComponent<PooledVfx>();
            pooled.Initialize(this);
            pooled.SetPrefabKey(prefab);
            return pooled;
        }
    }
}
