using UnityEngine;

namespace JamForge
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class TopDown2DController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody2D body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Vector2 move = InputReader.Instance != null ? InputReader.Instance.Move : Vector2.zero;
            body.linearVelocity = move.normalized * moveSpeed;
        }
    }
}
