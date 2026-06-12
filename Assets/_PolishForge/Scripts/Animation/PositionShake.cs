using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public class PositionShake : MonoBehaviour
    {
        private Coroutine activeRoutine;
        private Vector3 originalLocalPosition;

        public void Shake(float duration, float strength, float frequency, bool useUnscaledTime)
        {
            if (activeRoutine != null)
                StopCoroutine(activeRoutine);

            activeRoutine = StartCoroutine(ShakeRoutine(duration, strength, frequency, useUnscaledTime));
        }

        private IEnumerator ShakeRoutine(float duration, float strength, float frequency, bool useUnscaledTime)
        {
            originalLocalPosition = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float falloff = 1f - Mathf.Clamp01(elapsed / duration);
                Vector2 random = Random.insideUnitCircle * strength * falloff;
                transform.localPosition = originalLocalPosition + new Vector3(random.x, random.y, 0f);
                yield return null;
            }

            transform.localPosition = originalLocalPosition;
            activeRoutine = null;
        }
    }
}
