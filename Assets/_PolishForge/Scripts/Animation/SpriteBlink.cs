using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public class SpriteBlink : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Blink(float duration, float interval = 0.08f)
        {
            if (spriteRenderer != null)
                StartCoroutine(BlinkRoutine(duration, interval));
        }

        private IEnumerator BlinkRoutine(float duration, float interval)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += interval;
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(interval);
            }

            spriteRenderer.enabled = true;
        }
    }
}
