using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PolishForge
{
    public class FloatingTextSpawner : PolishService<FloatingTextSpawner>
    {
        [SerializeField] private FloatingText prefab;

        private readonly Queue<FloatingText> pool = new();

        public void Spawn(string text, Vector3 position, Color color, float duration, Vector3 rise, Vector3 randomSpread)
        {
            if (PolishSettings.Instance != null && !PolishSettings.Instance.EnableFloatingText)
                return;

            FloatingText item = Get();
            item.transform.position = position + new Vector3(
                Random.Range(-randomSpread.x, randomSpread.x),
                Random.Range(-randomSpread.y, randomSpread.y),
                Random.Range(-randomSpread.z, randomSpread.z));
            item.gameObject.SetActive(true);
            item.Show(text, color, duration, rise);
        }

        public void Despawn(FloatingText item)
        {
            if (item == null)
                return;

            item.gameObject.SetActive(false);
            item.transform.SetParent(transform, false);
            pool.Enqueue(item);
        }

        private FloatingText Get()
        {
            while (pool.Count > 0)
            {
                FloatingText item = pool.Dequeue();
                if (item != null)
                    return item;
            }

            FloatingText created;
            if (prefab != null)
            {
                created = Instantiate(prefab, transform);
            }
            else
            {
                GameObject root = new("FloatingText");
                root.transform.SetParent(transform, false);
                created = root.AddComponent<FloatingText>();
                GameObject labelObject = new("Label");
                labelObject.transform.SetParent(root.transform, false);
                TextMeshPro label = labelObject.AddComponent<TextMeshPro>();
                label.alignment = TextAlignmentOptions.Center;
                label.fontSize = 4f;
            }

            created.Initialize(this);
            return created;
        }
    }
}
