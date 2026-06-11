using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JamForge
{
    public sealed class InputReader : PersistentSingleton<InputReader>
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private bool handlePauseAndRestart = true;

        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction attackAction;
        private InputAction interactAction;
        private InputAction pauseAction;
        private InputAction restartAction;
        private InputAction debug1Action;
        private InputAction debug2Action;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public event Action OnJump;
        public event Action OnAttack;
        public event Action OnInteract;
        public event Action OnPause;
        public event Action OnRestart;
        public event Action OnDebug1;
        public event Action OnDebug2;

        protected override void OnAwakeSingleton()
        {
            BindActions();
        }

        private void OnEnable()
        {
            inputActions?.Enable();
        }

        private void OnDisable()
        {
            inputActions?.Disable();
        }

        private void Update()
        {
            Move = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
            Look = lookAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        private void BindActions()
        {
            if (inputActions == null)
                inputActions = CreateDefaultActions();

            moveAction = inputActions.FindAction("Move", true);
            lookAction = inputActions.FindAction("Look", false);
            jumpAction = inputActions.FindAction("Jump", false);
            attackAction = inputActions.FindAction("Attack", false);
            interactAction = inputActions.FindAction("Interact", false);
            pauseAction = inputActions.FindAction("Pause", false);
            restartAction = inputActions.FindAction("Restart", false);
            debug1Action = inputActions.FindAction("Debug1", false);
            debug2Action = inputActions.FindAction("Debug2", false);

            if (jumpAction != null) jumpAction.performed += _ => OnJump?.Invoke();
            if (attackAction != null) attackAction.performed += _ => OnAttack?.Invoke();
            if (interactAction != null) interactAction.performed += _ => OnInteract?.Invoke();
            if (pauseAction != null) pauseAction.performed += _ => HandlePause();
            if (restartAction != null) restartAction.performed += _ => HandleRestart();
            if (debug1Action != null) debug1Action.performed += _ => OnDebug1?.Invoke();
            if (debug2Action != null) debug2Action.performed += _ => OnDebug2?.Invoke();

            inputActions.Enable();
        }

        private void HandlePause()
        {
            OnPause?.Invoke();

            if (!handlePauseAndRestart || GameStateManager.Instance == null)
                return;

            if (GameStateManager.Instance.Is(GameState.Playing))
                GameStateManager.Instance.SetState(GameState.Paused);
            else if (GameStateManager.Instance.Is(GameState.Paused))
                GameStateManager.Instance.SetState(GameState.Playing);
        }

        private void HandleRestart()
        {
            OnRestart?.Invoke();

            if (handlePauseAndRestart)
                SceneLoader.ReloadCurrentScene();
        }

        private static InputActionAsset CreateDefaultActions()
        {
            const string json = @"{
                ""maps"": [
                    {
                        ""name"": ""Gameplay"",
                        ""actions"": [
                            { ""name"": ""Move"", ""type"": ""Value"", ""expectedControlType"": ""Vector2"" },
                            { ""name"": ""Look"", ""type"": ""Value"", ""expectedControlType"": ""Vector2"" },
                            { ""name"": ""Jump"", ""type"": ""Button"" },
                            { ""name"": ""Attack"", ""type"": ""Button"" },
                            { ""name"": ""Interact"", ""type"": ""Button"" },
                            { ""name"": ""Pause"", ""type"": ""Button"" },
                            { ""name"": ""Submit"", ""type"": ""Button"" },
                            { ""name"": ""Cancel"", ""type"": ""Button"" },
                            { ""name"": ""Restart"", ""type"": ""Button"" },
                            { ""name"": ""Debug1"", ""type"": ""Button"" },
                            { ""name"": ""Debug2"", ""type"": ""Button"" }
                        ],
                        ""bindings"": [
                            { ""name"": ""WASD"", ""path"": ""2DVector"", ""action"": ""Move"", ""isComposite"": true },
                            { ""name"": ""up"", ""path"": ""<Keyboard>/w"", ""action"": ""Move"", ""isPartOfComposite"": true },
                            { ""name"": ""down"", ""path"": ""<Keyboard>/s"", ""action"": ""Move"", ""isPartOfComposite"": true },
                            { ""name"": ""left"", ""path"": ""<Keyboard>/a"", ""action"": ""Move"", ""isPartOfComposite"": true },
                            { ""name"": ""right"", ""path"": ""<Keyboard>/d"", ""action"": ""Move"", ""isPartOfComposite"": true },
                            { ""path"": ""<Gamepad>/leftStick"", ""action"": ""Move"" },
                            { ""path"": ""<Mouse>/delta"", ""action"": ""Look"" },
                            { ""path"": ""<Gamepad>/rightStick"", ""action"": ""Look"" },
                            { ""path"": ""<Keyboard>/space"", ""action"": ""Jump"" },
                            { ""path"": ""<Gamepad>/buttonSouth"", ""action"": ""Jump"" },
                            { ""path"": ""<Mouse>/leftButton"", ""action"": ""Attack"" },
                            { ""path"": ""<Gamepad>/rightTrigger"", ""action"": ""Attack"" },
                            { ""path"": ""<Keyboard>/e"", ""action"": ""Interact"" },
                            { ""path"": ""<Gamepad>/buttonWest"", ""action"": ""Interact"" },
                            { ""path"": ""<Keyboard>/escape"", ""action"": ""Pause"" },
                            { ""path"": ""<Gamepad>/start"", ""action"": ""Pause"" },
                            { ""path"": ""<Keyboard>/r"", ""action"": ""Restart"" },
                            { ""path"": ""<Keyboard>/f1"", ""action"": ""Debug1"" },
                            { ""path"": ""<Keyboard>/f2"", ""action"": ""Debug2"" }
                        ]
                    }
                ]
            }";

            return InputActionAsset.FromJson(json);
        }
    }
}
