using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public class PolishCameraShake : PolishService<PolishCameraShake>
    {
        [SerializeField] private Camera targetCamera;

        private Coroutine shakeRoutine;
        private Vector3 originalLocalPosition;

        public void Shake(float duration, float strength, float frequency = 25f)
        {
            if (PolishSettings.Instance != null)
            {
                if (!PolishSettings.Instance.EnableCameraShake)
                    return;

                strength *= PolishSettings.Instance.MasterPolishIntensity * PolishSettings.Instance.CameraShakeIntensity;
            }

            if (strength <= 0f || duration <= 0f)
                return;

            if (targetCamera == null)
                targetCamera = Camera.main;

            if (targetCamera == null)
            {
                Debug.LogWarning("[PolishForge] PolishCameraShake: no camera available for shake.");
                return;
            }

            if (shakeRoutine != null)
                StopCoroutine(shakeRoutine);

            shakeRoutine = StartCoroutine(ShakeRoutine(duration, strength, frequency));
        }

        private IEnumerator ShakeRoutine(float duration, float strength, float frequency)
        {
            Transform target = targetCamera.transform;
            originalLocalPosition = target.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float falloff = 1f - Mathf.Clamp01(elapsed / duration);
                float x = (Mathf.PerlinNoise(Time.unscaledTime * frequency, 0f) - 0.5f) * 2f;
                float y = (Mathf.PerlinNoise(0f, Time.unscaledTime * frequency) - 0.5f) * 2f;
                target.localPosition = originalLocalPosition + new Vector3(x, y, 0f) * strength * falloff;
                yield return null;
            }

            target.localPosition = originalLocalPosition;
            shakeRoutine = null;
        }
    }
}
