using System.Collections.Generic;
using UnityEngine;

namespace JamForge
{
    public sealed class ObjectPool : MonoBehaviour
    {
        [SerializeField] private Poolable prefab;
        [SerializeField] private int initialSize = 16;
        [SerializeField] private bool expandWhenEmpty = true;

        private readonly Queue<Poolable> available = new();
        private readonly HashSet<Poolable> active = new();

        public int ActiveCount => active.Count;
        public int AvailableCount => available.Count;

        private void Awake()
        {
            Prewarm(initialSize);
        }

        private void OnValidate()
        {
            initialSize = Mathf.Max(0, initialSize);

            if (prefab == null)
                Debug.LogWarning("ObjectPool needs a Poolable prefab before it can spawn objects.", this);
        }

        public Poolable Spawn(Vector3 position, Quaternion rotation)
        {
            Poolable item = GetItem();
            if (item == null)
                return null;

            Transform itemTransform = item.transform;
            itemTransform.SetPositionAndRotation(position, rotation);
            item.gameObject.SetActive(true);
            active.Add(item);
            item.OnSpawned();
            return item;
        }

        public T Spawn<T>(Vector3 position, Quaternion rotation) where T : Poolable
        {
            return Spawn(position, rotation) as T;
        }

        public void Despawn(Poolable item)
        {
            if (item == null || !active.Remove(item))
                return;

            item.OnDespawned();
            item.gameObject.SetActive(false);
            available.Enqueue(item);
        }

        private void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Poolable item = CreateItem();
                if (item != null)
                    available.Enqueue(item);
            }
        }

        private Poolable GetItem()
        {
            if (available.Count > 0)
                return available.Dequeue();

            return expandWhenEmpty ? CreateItem() : null;
        }

        private Poolable CreateItem()
        {
            if (prefab == null)
            {
                Debug.LogError("ObjectPool is missing a prefab.", this);
                return null;
            }

            Poolable item = Instantiate(prefab, transform);
            item.SetOwningPool(this);
            item.gameObject.SetActive(false);
            return item;
        }
    }
}
