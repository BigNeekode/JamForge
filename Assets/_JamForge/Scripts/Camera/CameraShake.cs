using System.Collections;
using UnityEngine;

namespace JamForge
{
    public sealed class CameraShake : PersistentSingleton<CameraShake>
    {
        [SerializeField] private Transform targetCamera;

        private Coroutine routine;
        private Vector3 originalLocalPosition;

        protected override void OnAwakeSingleton()
        {
            if (targetCamera == null && Camera.main != null)
                targetCamera = Camera.main.transform;

            if (targetCamera != null)
                originalLocalPosition = targetCamera.localPosition;
        }

        public void Shake(float duration = 0.2f, float strength = 0.2f)
        {
            if (targetCamera == null)
                return;

            if (routine != null)
                StopCoroutine(routine);

            routine = StartCoroutine(ShakeRoutine(duration, strength));
        }

        private IEnumerator ShakeRoutine(float duration, float strength)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                targetCamera.localPosition = originalLocalPosition + Random.insideUnitSphere * strength;
                yield return null;
            }

            targetCamera.localPosition = originalLocalPosition;
            routine = null;
        }
    }
}
