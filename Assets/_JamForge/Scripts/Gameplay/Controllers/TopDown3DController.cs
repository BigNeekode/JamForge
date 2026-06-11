using UnityEngine;

namespace JamForge
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class TopDown3DController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -20f;

        private CharacterController controller;
        private float verticalVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Vector2 input = InputReader.Instance != null ? InputReader.Instance.Move : Vector2.zero;
            Vector3 move = new(input.x, 0f, input.y);
            if (move.sqrMagnitude > 1f)
                move.Normalize();

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -1f;

            verticalVelocity += gravity * Time.deltaTime;
            Vector3 velocity = move * moveSpeed;
            velocity.y = verticalVelocity;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
