# JamForge: Unity 6 Game Jam Framework Specification

Version: 1.0  
Target Engine: Unity 6  
Primary Goal: A reusable, delete-friendly, inspector-friendly Unity framework for short-duration game jams.

---

## 0. Design Philosophy

JamForge is not a production engine. It is a fast-start game jam kit.

The framework should help a small Unity team start a new jam quickly with working scene flow, input, UI, audio, game state, pooling, debug tools, and simple gameplay primitives.

The framework must be:

- Simple
- Modular
- Easy to delete
- Easy to understand during a tired 3 AM jam session
- Useful for both 2D and 3D games
- Built for Unity 6
- Friendly to Codex-assisted implementation
- Free of paid dependencies
- Free of unnecessary architecture fog

The framework should feel like a reliable folding knife, not a cathedral with wheels.

---

## 1. Recommended Unity Setup

Recommended baseline:

- Unity 6
- Universal Render Pipeline project template
- Unity Input System package
- TextMeshPro
- UGUI
- Optional Cinemachine support, but do not make core systems depend on Cinemachine

Notes:

- Use Unity's Input System through an `InputActionAsset` and a wrapper class.
- Use `PlayerPrefs` only for simple preferences such as volume, fullscreen, quality, and resolution.
- Use `ScriptableObject` for reusable configuration and shared content data.

Reference docs:

- Unity Input System PlayerInput docs: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.6/manual/PlayerInput.html
- Unity Input Action Assets docs: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.6/manual/ActionAssets.html
- Unity PlayerPrefs docs: https://docs.unity3d.com/6000.4/Documentation/ScriptReference/PlayerPrefs.html
- Unity ScriptableObject docs: https://docs.unity3d.com/6000.1/Documentation/ScriptReference/ScriptableObject.html

---

## 2. Folder Structure

Create this folder structure:

```text
Assets/
  _JamForge/
    Art/
      Sprites/
      Materials/
      Models/
    Audio/
      Music/
      SFX/
      Mixers/
    Prefabs/
      Core/
      UI/
      Gameplay/
      VFX/
    Scenes/
      00_Boot.unity
      01_MainMenu.unity
      02_Game.unity
      03_GameOver.unity
    Scripts/
      Core/
      Input/
      UI/
      Audio/
      SceneFlow/
      Gameplay/
      Camera/
      Save/
      Debugging/
      Utilities/
      Editor/
    ScriptableObjects/
      Audio/
      Items/
      Gameplay/
      Settings/
    Resources/
    Settings/
  ThirdParty/
  Plugins/
```

Use `_JamForge` so framework files stay visible at the top of the Unity Project window.

---

## 3. Assembly Definitions

Create assembly definitions:

```text
Assets/_JamForge/Scripts/JamForge.Runtime.asmdef
Assets/_JamForge/Scripts/Editor/JamForge.Editor.asmdef
```

Rules:

- Runtime scripts must not reference editor-only code.
- Editor scripts must stay under `Assets/_JamForge/Scripts/Editor/`.
- Use namespace `JamForge` for all runtime framework code.
- Use namespace `JamForge.Editor` for all editor-only tools.

---

## 4. Code Pattern Recommendations

This section defines the preferred pattern for each framework part.

### 4.1 Core Services Pattern

Use a simple persistent singleton only for true framework services.

Allowed singleton services:

- `GameStateManager`
- `InputReader`
- `AudioManager`
- `SettingsManager`
- `UIManager`
- `ScoreManager`, optional
- `CameraShake`, optional

Do not use singletons for:

- Enemies
- Player weapons
- Pickups
- Projectiles
- Level-specific objects
- Doors
- Quest objects
- Temporary gameplay logic

Recommended base pattern:

```csharp
using UnityEngine;

namespace JamForge
{
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            DontDestroyOnLoad(gameObject);
            OnAwakeSingleton();
        }

        protected virtual void OnAwakeSingleton() { }
    }
}
```

Pattern reason:

- Easy to use
- Easy to understand
- Fast for jams
- Avoids complex dependency injection

Avoid:

- Reflection injection
- Service locators everywhere
- Global access for gameplay objects

---

### 4.2 Scene Flow Pattern

Use a static `SceneLoader` for convenience.

Pattern:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JamForge
{
    public enum JamScene
    {
        Boot,
        MainMenu,
        Game,
        GameOver
    }

    public static class SceneLoader
    {
        public static void Load(JamScene scene)
        {
            SceneManager.LoadScene(GetSceneName(scene));
        }

        public static void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public static void ReloadCurrentScene()
        {
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.name);
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            Debug.Log("Quit requested. Ignored inside the Unity Editor.");
#else
            Application.Quit();
#endif
        }

        private static string GetSceneName(JamScene scene)
        {
            return scene switch
            {
                JamScene.Boot => "00_Boot",
                JamScene.MainMenu => "01_MainMenu",
                JamScene.Game => "02_Game",
                JamScene.GameOver => "03_GameOver",
                _ => "01_MainMenu"
            };
        }
    }
}
```

Recommended usage:

```csharp
SceneLoader.Load(JamScene.Game);
SceneLoader.ReloadCurrentScene();
SceneLoader.Load(JamScene.MainMenu);
```

Avoid:

- Hardcoding scene names all over UI buttons
- Making every system responsible for scene loading

---

### 4.3 Bootstrap Pattern

Use a `JamBootstrapper` in `00_Boot`.

Responsibilities:

- Validate or spawn `JamCore`
- Call settings load
- Initialize audio
- Load main menu
- Prevent duplicate core managers

Pattern:

```csharp
using UnityEngine;

namespace JamForge
{
    public sealed class JamBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameObject jamCorePrefab;
        [SerializeField] private bool loadMainMenuOnStart = true;

        private void Start()
        {
            EnsureCoreExists();

            SettingsManager.Instance.Load();
            GameStateManager.Instance.SetState(GameState.MainMenu);

            if (loadMainMenuOnStart)
            {
                SceneLoader.Load(JamScene.MainMenu);
            }
        }

        private void EnsureCoreExists()
        {
            if (GameStateManager.Instance != null)
                return;

            if (jamCorePrefab == null)
            {
                Debug.LogError("JamBootstrapper is missing JamCore prefab.", this);
                return;
            }

            Instantiate(jamCorePrefab);
        }
    }
}
```

Important:

- Directly opening `02_Game` during development should still work.
- Add a `RuntimeInitializeOnLoadMethod` safety net if needed.

---

### 4.4 Game State Pattern

Use an event-driven state manager.

Pattern:

```csharp
using System;
using UnityEngine;

namespace JamForge
{
    public enum GameState
    {
        Booting,
        MainMenu,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public sealed class GameStateManager : PersistentSingleton<GameStateManager>
    {
        [SerializeField] private bool controlTimeScale = true;

        public GameState CurrentState { get; private set; } = GameState.Booting;
        public event Action<GameState, GameState> OnStateChanged;

        public void SetState(GameState newState)
        {
            if (CurrentState == newState)
                return;

            GameState previous = CurrentState;
            CurrentState = newState;

            ApplyTimeScale(newState);
            OnStateChanged?.Invoke(previous, newState);
            JamEventBus.Raise(new GameStateChangedEvent(previous, newState));
        }

        public bool Is(GameState state) => CurrentState == state;

        private void ApplyTimeScale(GameState state)
        {
            if (!controlTimeScale)
                return;

            Time.timeScale = state switch
            {
                GameState.Paused => 0f,
                GameState.GameOver => 0f,
                GameState.Victory => 0f,
                _ => 1f
            };
        }
    }

    public readonly struct GameStateChangedEvent
    {
        public readonly GameState Previous;
        public readonly GameState Current;

        public GameStateChangedEvent(GameState previous, GameState current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
```

Recommended usage:

```csharp
GameStateManager.Instance.SetState(GameState.Paused);
GameStateManager.Instance.SetState(GameState.Playing);
```

Avoid:

- Letting UI, player, enemy, and timer each invent their own pause logic
- Using strings for state names

---

### 4.5 Event Bus Pattern

Use a tiny typed event bus.

Pattern:

```csharp
using System;
using System.Collections.Generic;

namespace JamForge
{
    public static class JamEventBus
    {
        private static readonly Dictionary<Type, Delegate> Events = new();

        public static void Subscribe<T>(Action<T> listener)
        {
            Type type = typeof(T);
            Events[type] = Events.TryGetValue(type, out Delegate existing)
                ? Delegate.Combine(existing, listener)
                : listener;
        }

        public static void Unsubscribe<T>(Action<T> listener)
        {
            Type type = typeof(T);

            if (!Events.TryGetValue(type, out Delegate existing))
                return;

            Delegate current = Delegate.Remove(existing, listener);

            if (current == null)
                Events.Remove(type);
            else
                Events[type] = current;
        }

        public static void Raise<T>(T evt)
        {
            if (Events.TryGetValue(typeof(T), out Delegate existing))
            {
                (existing as Action<T>)?.Invoke(evt);
            }
        }

        public static void Clear()
        {
            Events.Clear();
        }
    }
}
```

Recommended event style:

```csharp
public readonly struct ScoreChangedEvent
{
    public readonly int Score;
    public ScoreChangedEvent(int score) => Score = score;
}
```

Recommended usage:

```csharp
private void OnEnable()
{
    JamEventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
}

private void OnDisable()
{
    JamEventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
}
```

Avoid:

- Event bus for every small local interaction
- String-based event names
- Static events scattered in many different classes

Use direct C# events for local component-to-component communication.

Use `JamEventBus` only for global gameplay messages.

---

### 4.6 Input Pattern

Use the Unity Input System, but hide it behind an `InputReader`.

Recommended pattern:

- Create one `JamInputActions.inputactions` asset.
- Generate the C# class from the asset if desired.
- `InputReader` reads and exposes clean game-facing values/events.
- Gameplay scripts never directly depend on raw input action callbacks.

Pattern:

```csharp
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JamForge
{
    public sealed class InputReader : PersistentSingleton<InputReader>
    {
        [SerializeField] private InputActionAsset inputActions;

        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction attackAction;
        private InputAction interactAction;
        private InputAction pauseAction;
        private InputAction restartAction;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public event Action OnJump;
        public event Action OnAttack;
        public event Action OnInteract;
        public event Action OnPause;
        public event Action OnRestart;

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
            {
                Debug.LogError("InputReader is missing InputActionAsset.", this);
                return;
            }

            moveAction = inputActions.FindAction("Move", true);
            lookAction = inputActions.FindAction("Look", false);
            jumpAction = inputActions.FindAction("Jump", false);
            attackAction = inputActions.FindAction("Attack", false);
            interactAction = inputActions.FindAction("Interact", false);
            pauseAction = inputActions.FindAction("Pause", false);
            restartAction = inputActions.FindAction("Restart", false);

            if (jumpAction != null) jumpAction.performed += _ => OnJump?.Invoke();
            if (attackAction != null) attackAction.performed += _ => OnAttack?.Invoke();
            if (interactAction != null) interactAction.performed += _ => OnInteract?.Invoke();
            if (pauseAction != null) pauseAction.performed += _ => OnPause?.Invoke();
            if (restartAction != null) restartAction.performed += _ => OnRestart?.Invoke();
        }
    }
}
```

Recommended action names:

```text
Move
Look
Jump
Attack
Interact
Pause
Submit
Cancel
Restart
Debug1
Debug2
```

Avoid:

- Calling `Keyboard.current` directly in gameplay scripts
- Letting every controller own its own input action map
- Mixing old Input Manager and new Input System without reason

---

### 4.7 UI Pattern

Use screen classes with a central `UIManager`.

Pattern:

```csharp
using UnityEngine;

namespace JamForge
{
    public abstract class UIScreen : MonoBehaviour
    {
        public bool IsVisible => gameObject.activeSelf;

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
```

UIManager pattern:

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace JamForge
{
    public sealed class UIManager : PersistentSingleton<UIManager>
    {
        [SerializeField] private List<UIScreen> screens = new();
        private readonly Dictionary<System.Type, UIScreen> screenMap = new();

        protected override void OnAwakeSingleton()
        {
            foreach (UIScreen screen in screens)
            {
                if (screen == null)
                    continue;

                screenMap[screen.GetType()] = screen;
            }
        }

        public T Get<T>() where T : UIScreen
        {
            if (screenMap.TryGetValue(typeof(T), out UIScreen screen))
                return screen as T;

            Debug.LogWarning($"UI screen not found: {typeof(T).Name}");
            return null;
        }

        public void Show<T>() where T : UIScreen
        {
            Get<T>()?.Show();
        }

        public void Hide<T>() where T : UIScreen
        {
            Get<T>()?.Hide();
        }
    }
}
```

Recommended UI screens:

- `MainMenuScreen`
- `PauseScreen`
- `GameOverScreen`
- `VictoryScreen`
- `HUDScreen`
- `SettingsScreen`
- `FadeScreen`

Recommended button pattern:

```csharp
public void OnPlayClicked()
{
    GameStateManager.Instance.SetState(GameState.Playing);
    SceneLoader.Load(JamScene.Game);
}
```

Avoid:

- UI buttons directly controlling too many gameplay objects
- Searching UI with `FindObjectOfType` repeatedly
- Animating UI before functionality works

---

### 4.8 Audio Pattern

Use a ScriptableObject audio library plus an `AudioManager`.

Audio entry:

```csharp
using System;
using UnityEngine;

namespace JamForge
{
    [Serializable]
    public sealed class AudioEntry
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop;
    }
}
```

Audio library:

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace JamForge
{
    [CreateAssetMenu(menuName = "JamForge/Audio Library")]
    public sealed class AudioLibrary : ScriptableObject
    {
        public List<AudioEntry> music = new();
        public List<AudioEntry> sfx = new();
    }
}
```

Recommended AudioManager approach:

- One looping music source
- One or more SFX sources
- Optional spatial SFX using `AudioSource.PlayClipAtPoint`
- Optional AudioMixer integration
- Log warning if id is missing

Avoid:

- Dragging AudioSource references manually into every button and gameplay object
- Calling `Resources.Load` for every SFX
- Crashing when an audio id is missing

---

### 4.9 Settings Pattern

Use PlayerPrefs wrapper.

Pattern:

```csharp
using UnityEngine;

namespace JamForge
{
    public sealed class SettingsManager : PersistentSingleton<SettingsManager>
    {
        private const string MasterVolumeKey = "JamForge.MasterVolume";
        private const string MusicVolumeKey = "JamForge.MusicVolume";
        private const string SfxVolumeKey = "JamForge.SfxVolume";
        private const string FullscreenKey = "JamForge.Fullscreen";

        public float MasterVolume { get; set; } = 1f;
        public float MusicVolume { get; set; } = 1f;
        public float SfxVolume { get; set; } = 1f;
        public bool Fullscreen { get; set; } = true;

        public void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            Fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
            PlayerPrefs.SetInt(FullscreenKey, Fullscreen ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ResetToDefaults()
        {
            MasterVolume = 1f;
            MusicVolume = 1f;
            SfxVolume = 1f;
            Fullscreen = true;
            Save();
        }
    }
}
```

Use PlayerPrefs for:

- Volume
- Fullscreen
- Resolution index
- Quality index
- VSync
- Mouse sensitivity

Do not use PlayerPrefs for:

- Complex save data
- Inventory
- Quest progress
- Large serialized data

---

### 4.10 Object Pool Pattern

Use a component-based pool.

Pattern:

```csharp
using UnityEngine;

namespace JamForge
{
    public abstract class Poolable : MonoBehaviour
    {
        public ObjectPool OwnerPool { get; internal set; }

        public virtual void OnSpawned() { }
        public virtual void OnDespawned() { }

        public void DespawnSelf()
        {
            if (OwnerPool != null)
                OwnerPool.Despawn(gameObject);
            else
                Destroy(gameObject);
        }
    }
}
```

Recommended pool behavior:

- Prewarm in `Awake`
- Store inactive objects in a queue
- Expand only when allowed
- Call `OnSpawned` and `OnDespawned`
- Parent pooled objects under the pool object

Avoid:

- A huge global pool manager for every possible prefab
- Pooling everything automatically
- Pooling objects that are spawned only once

Pool only:

- Bullets
- Particles
- Floating text
- Enemies if repeated often
- Pickups if repeated often

---

### 4.11 Health and Damage Pattern

Use tiny, reusable components.

Pattern:

```csharp
using System;
using UnityEngine;

namespace JamForge
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 3;

        public int MaxHealth => maxHealth;
        public int CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }

        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        private void Awake()
        {
            ResetHealth();
        }

        public void Damage(int amount)
        {
            if (IsDead || amount <= 0)
                return;

            CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (CurrentHealth <= 0)
            {
                IsDead = true;
                OnDied?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0)
                return;

            CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void Kill()
        {
            Damage(CurrentHealth);
        }

        public void ResetHealth()
        {
            IsDead = false;
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}
```

Recommended damage flow:

- `Projectile` detects collision
- Looks for `Health`
- Calls `Damage(amount)`
- Projectile despawns

Avoid:

- Putting all combat logic inside Health
- Making Health know about score, UI, particles, audio, and scene flow

Use events to connect death to effects or scoring.

---

### 4.12 Interaction Pattern

Use interface plus interactor.

Pattern:

```csharp
using UnityEngine;

namespace JamForge
{
    public interface IInteractable
    {
        string GetInteractionText();
        void Interact(GameObject interactor);
    }
}
```

Example interactable:

```csharp
using UnityEngine;

namespace JamForge
{
    public sealed class ScorePickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private int scoreAmount = 1;

        public string GetInteractionText() => "Pick up";

        public void Interact(GameObject interactor)
        {
            ScoreManager.Instance.Add(scoreAmount);
            Destroy(gameObject);
        }
    }
}
```

Interactor recommendations:

- Use trigger collider for simple jam interaction
- Track nearby interactables in a list
- Choose closest interactable
- Show prompt text
- Call interact on input

Avoid:

- Hardcoding every interactable type in the player script
- Making all objects inherit from one giant base class

---

### 4.13 Score Pattern

Use a simple manager and event.

Pattern:

```csharp
using System;
using UnityEngine;

namespace JamForge
{
    public sealed class ScoreManager : PersistentSingleton<ScoreManager>
    {
        public int Score { get; private set; }
        public event Action<int> OnScoreChanged;

        public void Add(int amount)
        {
            Set(Score + amount);
        }

        public void Set(int value)
        {
            Score = Mathf.Max(0, value);
            OnScoreChanged?.Invoke(Score);
            JamEventBus.Raise(new ScoreChangedEvent(Score));
        }

        public void ResetScore()
        {
            Set(0);
        }
    }

    public readonly struct ScoreChangedEvent
    {
        public readonly int Score;
        public ScoreChangedEvent(int score) => Score = score;
    }
}
```

Avoid:

- Letting UI calculate score
- Letting enemies directly modify UI text

---

### 4.14 Timer Pattern

Use a reusable timer component, not a singleton by default.

Pattern:

```csharp
using System;
using UnityEngine;

namespace JamForge
{
    public sealed class JamTimer : MonoBehaviour
    {
        [SerializeField] private bool useUnscaledTime;

        public float TimeRemaining { get; private set; }
        public bool IsRunning { get; private set; }

        public event Action<float> OnTimeChanged;
        public event Action OnTimerFinished;

        private void Update()
        {
            if (!IsRunning)
                return;

            float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            TimeRemaining = Mathf.Max(0f, TimeRemaining - delta);
            OnTimeChanged?.Invoke(TimeRemaining);

            if (TimeRemaining <= 0f)
            {
                IsRunning = false;
                OnTimerFinished?.Invoke();
            }
        }

        public void StartTimer(float seconds)
        {
            TimeRemaining = Mathf.Max(0f, seconds);
            IsRunning = true;
            OnTimeChanged?.Invoke(TimeRemaining);
        }

        public void StopTimer()
        {
            IsRunning = false;
        }
    }
}
```

Avoid:

- Making all timers global
- Hiding timer behavior in UI scripts

---

### 4.15 Controller Pattern

Controllers should be optional examples.

Rules:

- Keep controllers separate from core.
- Use `InputReader`.
- Do not make advanced movement.
- Provide common controller scripts that can be deleted.

Required sample controllers:

- `TopDown2DController`
- `TopDown3DController`
- `SideScroller2DController`
- `FirstPersonSimpleController`

Recommended top-down 2D pattern:

```csharp
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
```

Note:

- In Unity 6, `Rigidbody2D.linearVelocity` is preferred over older `velocity` usage.
- If compatibility is needed, Codex may adapt based on installed Unity API.

---

### 4.16 Projectile Pattern

Projectile should be pool-aware but not pool-dependent.

Pattern:

```csharp
using UnityEngine;

namespace JamForge
{
    public sealed class Projectile : Poolable
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private int damage = 1;

        private float lifeTimer;

        public override void OnSpawned()
        {
            lifeTimer = lifetime;
        }

        private void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            lifeTimer -= Time.deltaTime;

            if (lifeTimer <= 0f)
                DespawnSelf();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Health health))
            {
                health.Damage(damage);
            }

            DespawnSelf();
        }
    }
}
```

For 2D, create separate `Projectile2D` using `Collider2D`.

Avoid:

- One projectile class trying to support every 2D and 3D case with too many flags
- Hardcoded player/enemy tags unless needed

---

### 4.17 ScriptableObject Data Pattern

Use ScriptableObjects for jam data.

Recommended data assets:

- `GameConfig`
- `AudioLibrary`
- `ItemData`
- `WeaponData`
- `EnemyData`
- `LevelData`

Pattern:

```csharp
using UnityEngine;

namespace JamForge
{
    [CreateAssetMenu(menuName = "JamForge/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        public int startingLives = 3;
        public int startingScore = 0;
        public float gameDuration = 120f;
        public string defaultMusicId = "main_theme";
    }
}
```

Use ScriptableObjects for:

- Static data
- Tuning values
- Audio lists
- Item definitions
- Enemy definitions
- Level metadata

Avoid using ScriptableObjects as:

- Runtime mutable save files
- Global state containers unless intentional
- Replacement for every MonoBehaviour

---

### 4.18 Debug Pattern

Use debug helpers that compile out of release builds.

Pattern:

```csharp
using UnityEngine;

namespace JamForge
{
    public sealed class JamDebug : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current == null)
                return;

            var keyboard = UnityEngine.InputSystem.Keyboard.current;

            if (keyboard.f5Key.wasPressedThisFrame)
                SceneLoader.ReloadCurrentScene();

            if (keyboard.f6Key.wasPressedThisFrame)
                GameStateManager.Instance.SetState(GameState.Victory);

            if (keyboard.f7Key.wasPressedThisFrame)
                GameStateManager.Instance.SetState(GameState.GameOver);
        }
#endif
    }
}
```

Recommended debug keys:

```text
F1 toggle debug overlay
F5 reload scene
F6 trigger victory
F7 trigger game over
F8 add score
F9 toggle timescale
```

Avoid:

- Shipping debug cheats in release build
- Debug scripts that are required for gameplay to work

---

### 4.19 Editor Tool Pattern

Use Unity editor menu items only for setup helpers.

Recommended menu:

```text
Tools/JamForge/Create Jam Scenes
Tools/JamForge/Create Core Prefabs
Tools/JamForge/Open Framework Readme
Tools/JamForge/Reset PlayerPrefs
Tools/JamForge/Build Windows
Tools/JamForge/Build WebGL
```

Editor tools should:

- Be optional
- Save time during setup
- Not be required during runtime

Avoid:

- Editor magic that hides how the framework works
- Editor tools that create overly complex scene structures

---

## 5. Required Framework Systems

### 5.1 SceneLoader

File:

```text
Assets/_JamForge/Scripts/SceneFlow/SceneLoader.cs
```

Responsibilities:

- Load scene by enum
- Load scene by name
- Restart current scene
- Quit game
- Optional loading screen support
- Optional fade transition hook

Acceptance criteria:

- Main menu can load game scene
- Game scene can reload itself
- Game scene can return to main menu
- Quit does nothing in editor except log a message

---

### 5.2 JamBootstrapper

File:

```text
Assets/_JamForge/Scripts/Core/JamBootstrapper.cs
```

Responsibilities:

- Lives in `00_Boot`
- Creates or validates persistent core services
- Loads main menu after initialization
- Uses `DontDestroyOnLoad`

Core services:

```text
GameStateManager
AudioManager
InputReader
SettingsManager
UIManager
```

Acceptance criteria:

- Starting from Boot scene initializes framework
- Starting directly from Game scene still works during development
- Duplicate managers are automatically destroyed

---

### 5.3 GameStateManager

File:

```text
Assets/_JamForge/Scripts/Core/GameStateManager.cs
```

States:

```csharp
public enum GameState
{
    Booting,
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Victory
}
```

Acceptance criteria:

- Pressing pause changes state to Paused
- Resuming changes state to Playing
- Game over changes state to GameOver
- Systems can subscribe to state changes

---

### 5.4 JamEventBus

File:

```text
Assets/_JamForge/Scripts/Core/JamEventBus.cs
```

API:

```csharp
public static class JamEventBus
{
    public static void Subscribe<T>(Action<T> listener);
    public static void Unsubscribe<T>(Action<T> listener);
    public static void Raise<T>(T evt);
    public static void Clear();
}
```

Acceptance criteria:

- A listener can subscribe and receive an event
- A listener can unsubscribe and stop receiving events
- Raising event with no listeners does not error

---

### 5.5 InputReader

Files:

```text
Assets/_JamForge/Scripts/Input/InputReader.cs
Assets/_JamForge/Settings/JamInputActions.inputactions
```

Required actions:

```text
Move
Look
Jump
Attack
Interact
Pause
Submit
Cancel
Restart
Debug1
Debug2
```

Acceptance criteria:

- WASD or left stick updates Move
- Escape or Start triggers Pause
- E or gamepad button triggers Interact
- R triggers Restart

---

### 5.6 UI System

Files:

```text
Assets/_JamForge/Scripts/UI/UIManager.cs
Assets/_JamForge/Scripts/UI/UIScreen.cs
Assets/_JamForge/Scripts/UI/FadeScreen.cs
```

Default UI prefabs:

```text
MainMenuScreen
PauseScreen
GameOverScreen
VictoryScreen
HUDScreen
SettingsScreen
FadeScreen
ConfirmDialog
```

Acceptance criteria:

- Main menu buttons work
- Pause UI appears when game state is Paused
- Game over UI appears when game state is GameOver
- HUD appears during Playing
- Settings panel can adjust volume

---

### 5.7 Audio System

Files:

```text
Assets/_JamForge/Scripts/Audio/AudioManager.cs
Assets/_JamForge/Scripts/Audio/AudioLibrary.cs
Assets/_JamForge/Scripts/Audio/AudioEntry.cs
```

API:

```csharp
public void PlayMusic(string id, bool fade = true);
public void StopMusic(bool fade = true);
public void PlaySfx(string id);
public void PlaySfxAtPosition(string id, Vector3 position);
public void SetMasterVolume(float value);
public void SetMusicVolume(float value);
public void SetSfxVolume(float value);
```

Acceptance criteria:

- Main menu can play music
- Button click SFX plays
- Music volume slider works
- SFX volume slider works

---

### 5.8 SettingsManager

File:

```text
Assets/_JamForge/Scripts/Save/SettingsManager.cs
```

Settings:

```text
MasterVolume
MusicVolume
SfxVolume
Fullscreen
ResolutionIndex
QualityIndex
VSync
```

Acceptance criteria:

- Volume settings persist after stopping and restarting play mode
- Reset button restores defaults

---

### 5.9 Object Pool

Files:

```text
Assets/_JamForge/Scripts/Gameplay/ObjectPool.cs
Assets/_JamForge/Scripts/Gameplay/Poolable.cs
```

Acceptance criteria:

- Can spawn bullet prefab repeatedly
- Can despawn bullet
- No Instantiate after initial pool when enough objects exist

---

### 5.10 Health and Damage

Files:

```text
Assets/_JamForge/Scripts/Gameplay/Health.cs
Assets/_JamForge/Scripts/Gameplay/DamageDealer.cs
Assets/_JamForge/Scripts/Gameplay/Damageable.cs
```

Acceptance criteria:

- Damage lowers health
- Heal restores health
- Death event fires at 0
- Player death can trigger GameOver state

---

### 5.11 Interaction System

Files:

```text
Assets/_JamForge/Scripts/Gameplay/IInteractable.cs
Assets/_JamForge/Scripts/Gameplay/Interactor.cs
Assets/_JamForge/Scripts/Gameplay/InteractablePrompt.cs
```

Acceptance criteria:

- Player can approach object and see prompt
- Pressing interact calls object interaction
- Prompt disappears when leaving range

---

### 5.12 Score and Timer

Files:

```text
Assets/_JamForge/Scripts/Gameplay/ScoreManager.cs
Assets/_JamForge/Scripts/Gameplay/JamTimer.cs
```

Acceptance criteria:

- Score UI updates when score changes
- Timer counts down during Playing
- Timer can trigger GameOver or Victory depending on option

---

### 5.13 Camera Helpers

Files:

```text
Assets/_JamForge/Scripts/Camera/SimpleFollowCamera.cs
Assets/_JamForge/Scripts/Camera/CameraShake.cs
```

Acceptance criteria:

- Camera follows target smoothly
- Calling shake creates temporary camera shake
- Shake returns camera to original offset

---

### 5.14 Sample Controllers

Folder:

```text
Assets/_JamForge/Scripts/Gameplay/Controllers/
```

Required controllers:

```text
TopDown2DController
TopDown3DController
SideScroller2DController
FirstPersonSimpleController
```

Acceptance criteria:

- Developer can drag controller onto player prefab and move immediately
- Controllers can be deleted without breaking core framework

---

### 5.15 Projectile System

Files:

```text
Assets/_JamForge/Scripts/Gameplay/Projectile.cs
Assets/_JamForge/Scripts/Gameplay/Projectile2D.cs
Assets/_JamForge/Scripts/Gameplay/Shooter.cs
```

Acceptance criteria:

- Player can shoot pooled projectiles
- Projectile damages Health component
- Projectile despawns after lifetime

---

### 5.16 Debug Tools

Files:

```text
Assets/_JamForge/Scripts/Debugging/JamDebug.cs
Assets/_JamForge/Scripts/Debugging/DebugOverlay.cs
```

Debug overlay displays:

```text
Current scene
Game state
FPS
Score
Timer
Active pooled objects
```

Acceptance criteria:

- F1 toggles overlay in editor
- F5 reloads current scene
- F6 sets Victory
- F7 sets GameOver

---

### 5.17 Editor Tools

Files:

```text
Assets/_JamForge/Scripts/Editor/JamForgeMenu.cs
Assets/_JamForge/Scripts/Editor/JamBuildTools.cs
```

Menu:

```text
Tools/JamForge/Create Jam Scenes
Tools/JamForge/Create Core Prefabs
Tools/JamForge/Open Framework Readme
Tools/JamForge/Reset PlayerPrefs
Tools/JamForge/Build Windows
Tools/JamForge/Build WebGL
```

Acceptance criteria:

- Menu item can create missing scenes
- Menu item can reset PlayerPrefs
- Build menu can create Windows build
- Build menu can create WebGL build

---

## 6. Required Prefabs

Create these prefabs:

```text
Assets/_JamForge/Prefabs/Core/JamCore.prefab
Assets/_JamForge/Prefabs/Core/JamBootstrapper.prefab
Assets/_JamForge/Prefabs/UI/UIRoot.prefab
Assets/_JamForge/Prefabs/UI/MainMenuScreen.prefab
Assets/_JamForge/Prefabs/UI/PauseScreen.prefab
Assets/_JamForge/Prefabs/UI/GameOverScreen.prefab
Assets/_JamForge/Prefabs/UI/VictoryScreen.prefab
Assets/_JamForge/Prefabs/UI/HUDScreen.prefab
Assets/_JamForge/Prefabs/UI/SettingsScreen.prefab
Assets/_JamForge/Prefabs/Gameplay/SamplePlayer2D.prefab
Assets/_JamForge/Prefabs/Gameplay/SamplePlayer3D.prefab
Assets/_JamForge/Prefabs/Gameplay/SampleProjectile.prefab
```

Acceptance criteria:

- Opening MainMenu scene shows usable menu
- Opening Game scene shows usable player, HUD, pause, and game over flow

---

## 7. Sample Game Requirements

Create a tiny sample game in `02_Game`.

Sample requirements:

- Player can move
- Player can interact with a pickup
- Pickup adds score
- Player can shoot a projectile
- Enemy has health
- Projectile damages enemy
- Enemy death adds score
- Timer counts down
- Pause works
- Game over works
- Victory works
- Main menu can start game
- Game over can retry

The sample should be boring but functional. It exists to prove the framework works.

---

## 8. Non-Goals

Do not implement:

- Full inventory system
- Full quest system
- Full dialogue system
- Behavior tree AI
- Networking
- Multiplayer
- Save slots
- ECS/DOTS
- Addressables by default
- Complex ability system
- Complex animation framework
- Complex localization
- Online leaderboard
- Monetization
- Ads
- Analytics

These can be added later as optional modules.

---

## 9. Implementation Order

Implement in this order:

```text
Phase 1: Folder structure and assembly definitions
Phase 2: SceneLoader
Phase 3: PersistentSingleton and Bootstrapper
Phase 4: GameStateManager
Phase 5: JamEventBus
Phase 6: InputReader and InputActionAsset
Phase 7: UIManager and basic screens
Phase 8: SettingsManager
Phase 9: AudioManager
Phase 10: ObjectPool
Phase 11: Health/Damage
Phase 12: Interaction
Phase 13: Score and Timer
Phase 14: Camera helpers
Phase 15: Sample controllers
Phase 16: Projectile/Shooter
Phase 17: Debug tools
Phase 18: Editor menu
Phase 19: Sample game
Phase 20: README
```

---

## 10. Codex Prompt

Use this prompt for Codex:

```text
Implement this Unity 6 game jam framework inside the existing Unity project.

Important constraints:
- Keep it simple.
- Do not introduce external paid dependencies.
- Use namespace JamForge.
- Use Unity Input System.
- Use UGUI and TextMeshPro for UI.
- Use PlayerPrefs only for settings, not game saves.
- Use ScriptableObject for reusable static data/config.
- Avoid reflection and complex dependency injection.
- Use persistent singletons only for true core services.
- Make every system usable from inspector.
- Include a minimal sample scene proving the framework works.
- Do not overbuild beyond the spec.
- Keep controllers optional and deletable.
- Add helpful warnings/errors for missing references.
```

---

## 11. Version 1 Cut

If Codex struggles with the full framework, implement only this first:

```text
SceneLoader
PersistentSingleton
Bootstrapper
GameStateManager
JamEventBus
InputReader
UIManager
AudioManager
SettingsManager
ObjectPool
Health/Damage
Interaction
ScoreManager
JamTimer
DebugOverlay
```

Then add:

```text
Sample controllers
Projectile system
Editor tools
Sample game polish
```

---

## 12. Definition of Done

The framework is complete when:

- No compiler errors
- No missing script references
- Main menu works
- Scene loading works
- Pause/resume works
- Audio works
- Settings persist
- Player movement works
- Interaction works
- Object pooling works
- Health/damage works
- Score updates
- Timer updates
- Game over and victory screens work
- Debug overlay works in editor
- README explains how to use the framework
- All systems are simple enough to understand during a game jam

Final play flow:

```text
Open Unity
Open 00_Boot
Press Play
Main menu appears
Click Play
Game scene loads
Move player
Collect score
Shoot enemy
Pause game
Resume game
Trigger game over
Retry
Return to main menu
```
