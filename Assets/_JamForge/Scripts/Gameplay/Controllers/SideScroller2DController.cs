using UnityEngine;

namespace JamForge
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class SideScroller2DController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.15f;

        private Rigidbody2D body;
        private bool jumpQueued;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            if (InputReader.Instance != null)
                InputReader.Instance.OnJump += QueueJump;
        }

        private void OnDisable()
        {
            if (InputReader.Instance != null)
                InputReader.Instance.OnJump -= QueueJump;
        }

        private void FixedUpdate()
        {
            Vector2 move = InputReader.Instance != null ? InputReader.Instance.Move : Vector2.zero;
            body.linearVelocity = new Vector2(move.x * moveSpeed, body.linearVelocity.y);

            if (jumpQueued && IsGrounded())
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);

            jumpQueued = false;
        }

        private void QueueJump()
        {
            jumpQueued = true;
        }

        private bool IsGrounded()
        {
            Vector2 checkPosition = groundCheck != null ? groundCheck.position : transform.position;
            return Physics2D.OverlapCircle(checkPosition, groundRadius, groundMask) != null;
        }
    }
}
