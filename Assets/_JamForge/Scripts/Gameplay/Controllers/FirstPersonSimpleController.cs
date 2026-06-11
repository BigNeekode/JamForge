using UnityEngine;

namespace JamForge
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonSimpleController : MonoBehaviour
    {
        [SerializeField] private Transform view;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float lookSensitivity = 0.08f;
        [SerializeField] private float jumpSpeed = 5f;
        [SerializeField] private float gravity = -20f;

        private CharacterController controller;
        private float pitch;
        private float verticalVelocity;
        private bool jumpQueued;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (view == null && Camera.main != null)
                view = Camera.main.transform;
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

        private void Update()
        {
            Vector2 moveInput = InputReader.Instance != null ? InputReader.Instance.Move : Vector2.zero;
            Vector2 lookInput = InputReader.Instance != null ? InputReader.Instance.Look : Vector2.zero;

            transform.Rotate(Vector3.up, lookInput.x * lookSensitivity);
            pitch = Mathf.Clamp(pitch - lookInput.y * lookSensitivity, -85f, 85f);
            if (view != null)
                view.localEulerAngles = new Vector3(pitch, 0f, 0f);

            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            if (move.sqrMagnitude > 1f)
                move.Normalize();

            if (controller.isGrounded)
            {
                if (verticalVelocity < 0f)
                    verticalVelocity = -1f;
                if (jumpQueued)
                    verticalVelocity = jumpSpeed;
            }

            verticalVelocity += gravity * Time.deltaTime;
            Vector3 velocity = move * moveSpeed;
            velocity.y = verticalVelocity;
            controller.Move(velocity * Time.deltaTime);
            jumpQueued = false;
        }

        private void QueueJump()
        {
            jumpQueued = true;
        }
    }
}
