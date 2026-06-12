using UnityEngine;

namespace PolishForge
{
    public class BobMotion : MonoBehaviour
    {
        [SerializeField] private float amplitude = 0.15f;
        [SerializeField] private float speed = 2f;

        private Vector3 start;

        private void Awake() => start = transform.localPosition;

        private void Update()
        {
            transform.localPosition = start + Vector3.up * (Mathf.Sin(Time.time * speed) * amplitude);
        }
    }
}
