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
        private Action<InputAction.CallbackContext> jumpCallback;
        private Action<InputAction.CallbackContext> attackCallback;
        private Action<InputAction.CallbackContext> interactCallback;
        private Action<InputAction.CallbackContext> pauseCallback;
        private Action<InputAction.CallbackContext> restartCallback;
        private Action<InputAction.CallbackContext> debug1Callback;
        private Action<InputAction.CallbackContext> debug2Callback;
        private bool callbacksBound;

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
            ResolveActions();
        }

        private void OnEnable()
        {
            BindCallbacks();
            inputActions?.Enable();
        }

        private void OnDisable()
        {
            inputActions?.Disable();
            UnbindCallbacks();
            Move = Vector2.zero;
            Look = Vector2.zero;
        }

        protected override void OnDestroy()
        {
            UnbindCallbacks();
            base.OnDestroy();
        }

        private void Update()
        {
            Move = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
            Look = lookAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }

        private void ResolveActions()
        {
            if (inputActions == null)
            {
                Debug.LogWarning("InputReader has no InputActionAsset assigned. Using runtime default actions.", this);
                inputActions = CreateDefaultActions();
            }

            moveAction = FindAction("Move", true);
            lookAction = inputActions.FindAction("Look", false);
            jumpAction = inputActions.FindAction("Jump", false);
            attackAction = inputActions.FindAction("Attack", false);
            interactAction = inputActions.FindAction("Interact", false);
            pauseAction = inputActions.FindAction("Pause", false);
            restartAction = inputActions.FindAction("Restart", false);
            debug1Action = inputActions.FindAction("Debug1", false);
            debug2Action = inputActions.FindAction("Debug2", false);
        }

        private InputAction FindAction(string actionName, bool required)
        {
            InputAction action = inputActions.FindAction(actionName, false);
            if (required && action == null)
                Debug.LogError($"InputReader could not find required input action '{actionName}'.", this);

            return action;
        }

        private void BindCallbacks()
        {
            if (callbacksBound)
                return;

            jumpCallback = _ => OnJump?.Invoke();
            attackCallback = _ => OnAttack?.Invoke();
            interactCallback = _ => OnInteract?.Invoke();
            pauseCallback = _ => HandlePause();
            restartCallback = _ => HandleRestart();
            debug1Callback = _ => OnDebug1?.Invoke();
            debug2Callback = _ => OnDebug2?.Invoke();

            if (jumpAction != null) jumpAction.performed += jumpCallback;
            if (attackAction != null) attackAction.performed += attackCallback;
            if (interactAction != null) interactAction.performed += interactCallback;
            if (pauseAction != null) pauseAction.performed += pauseCallback;
            if (restartAction != null) restartAction.performed += restartCallback;
            if (debug1Action != null) debug1Action.performed += debug1Callback;
            if (debug2Action != null) debug2Action.performed += debug2Callback;

            callbacksBound = true;
        }

        private void UnbindCallbacks()
        {
            if (!callbacksBound)
                return;

            if (jumpAction != null) jumpAction.performed -= jumpCallback;
            if (attackAction != null) attackAction.performed -= attackCallback;
            if (interactAction != null) interactAction.performed -= interactCallback;
            if (pauseAction != null) pauseAction.performed -= pauseCallback;
            if (restartAction != null) restartAction.performed -= restartCallback;
            if (debug1Action != null) debug1Action.performed -= debug1Callback;
            if (debug2Action != null) debug2Action.performed -= debug2Callback;

            jumpCallback = null;
            attackCallback = null;
            interactCallback = null;
            pauseCallback = null;
            restartCallback = null;
            debug1Callback = null;
            debug2Callback = null;
            callbacksBound = false;
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
