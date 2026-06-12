using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public class ImpactNudge : MonoBehaviour
    {
        [SerializeField] private float distance = 0.2f;
        [SerializeField] private float duration = 0.12f;

        public void Nudge(Vector3 direction)
        {
            StartCoroutine(NudgeRoutine(direction.normalized));
        }

        private IEnumerator NudgeRoutine(Vector3 direction)
        {
            Vector3 start = transform.localPosition;
            Vector3 end = start + direction * distance;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Sin(Mathf.Clamp01(elapsed / duration) * Mathf.PI);
                transform.localPosition = Vector3.Lerp(start, end, t);
                yield return null;
            }

            transform.localPosition = start;
        }
    }
}
