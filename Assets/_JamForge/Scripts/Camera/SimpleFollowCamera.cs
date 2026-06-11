using UnityEngine;

namespace JamForge
{
    public sealed class SimpleFollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0f, 6f, -8f);
        [SerializeField] private float smoothTime = 0.12f;
        [SerializeField] private bool lookAtTarget = true;

        private Vector3 velocity;

        private void LateUpdate()
        {
            if (target == null)
                return;

            Vector3 desired = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);

            if (lookAtTarget)
                transform.LookAt(target);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
