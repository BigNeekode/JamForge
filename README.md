# JamForge

**JamForge** is a small, reusable **Unity 6 game jam framework** built to help teams start prototypes faster without dragging a giant production architecture into a 48-hour chaos kitchen.

It lives under `Assets/_JamForge` so the framework stays easy to inspect, copy, upgrade, or delete when a jam prototype grows into something custom.

> JamForge is a fast-start kit, not a full engine. It gives you the scaffolding so your actual game can climb faster.

---

## What Is Included

JamForge includes common systems that game jam projects usually need again and again:

- Scene loading through `SceneLoader`
- Persistent core services with `PersistentSingleton`
- Runtime bootstrap safety for direct scene testing
- Game state events through `GameStateManager` and `JamEventBus`
- Input System wrapper through `InputReader`
- UGUI/TMP screen helpers through `UIManager` and `UIScreen`
- PlayerPrefs-backed settings through `SettingsManager`
- Simple audio library and `AudioManager`
- Object pooling
- Health and damage helpers
- Interaction helpers
- Score and timer systems
- Projectile helpers
- Camera helpers
- Sample controllers
- Debug tools
- Editor menu helpers under `Tools/JamForge`

---

## Current Project Progress

JamForge is currently in an **early reusable framework** stage. The core scaffolding is already in place and focused on getting a Unity 6 game jam project playable quickly.

### Implemented / Available

- Root project documentation through this `README.md`
- Internal framework documentation at `Assets/_JamForge/README.md`
- Framework folder layout under `Assets/_JamForge`
- Bootstrap-oriented scene flow design
- Default jam scene order:

```text
00_Boot
01_MainMenu
02_Game
03_GameOver
```

- Runtime bootstrap safety through `JamRuntime`
- Persistent service pattern through `PersistentSingleton`
- Scene loading through `SceneLoader`
- Game state flow through `GameStateManager`
- Global event messaging through `JamEventBus`
- Unity Input System wrapper through `InputReader`
- Default runtime input map fallback when no `InputActionAsset` is assigned
- UI screen helper pattern through `UIManager` and `UIScreen`
- PlayerPrefs-backed settings through `SettingsManager`
- Simple music and SFX playback through `AudioManager`
- Health and damage helpers through `Health` and `IDamageable`
- Object pooling helpers
- Interaction, score, timer, projectile, camera, sample controller, and debug helper systems
- Debug shortcut keys for fast jam testing
- Editor menu helpers under `Tools/JamForge`
- Repository-level `.gitignore` rule for private `spec/` planning files
- Apache License 2.0 added through `LICENSE`

### Usable Now

JamForge can already be used as a starter foundation for jam prototypes that need:

- A boot scene
- A main menu scene
- A gameplay scene
- A game-over scene
- Basic game state transitions
- Basic UI screen switching
- Basic settings persistence
- Basic audio playback
- Basic input handling
- Basic health/damage logic
- Quick debug shortcuts

### Still Needs Polish / Validation

These areas should be tested and refined through real jam prototypes:

- Full prefab generation flow from `Tools/JamForge/Create Core Prefabs`
- Default scene generation flow from `Tools/JamForge/Create Jam Scenes`
- Example gameplay scene wiring
- WebGL build testing
- Sample 2D prototype
- Sample 3D prototype
- More complete usage examples for each system
- Optional build/submission checklist tooling
- Third-party asset/package license review before public release

### Private Planning Docs

Private specs and planning documents are intentionally kept out of the public-facing README and ignored through `.gitignore`.

If you keep a local `spec/` folder, it is for planning only and should not be treated as part of the public framework package.

---

## Target Setup

Recommended baseline:

```text
Unity 6
Universal Render Pipeline
Unity Input System
TextMeshPro
UGUI
```

Optional:

```text
Cinemachine
```

Cinemachine can be useful for jam cameras, but JamForge core systems are designed not to depend on it.

---

## Quick Start

1. Open the project in Unity 6.
2. Use `Tools/JamForge/Create Jam Scenes` to create the default scene files.
3. Use `Tools/JamForge/Create Core Prefabs` to create starter prefabs.
4. Put `JamBootstrapper` in `00_Boot`.
5. Add scenes to Build Settings in this order:

```text
00_Boot
01_MainMenu
02_Game
03_GameOver
```

6. Press Play from `00_Boot`.

Directly opening a gameplay scene also works because `JamRuntime` creates missing core services before scene load.

---

## Folder Structure

The framework is designed to stay contained inside `Assets/_JamForge`:

```text
Assets/
  _JamForge/
    Art/
    Audio/
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
    Resources/
    Settings/
  ThirdParty/
  Plugins/
```

Keep game-specific scripts outside `_JamForge` unless they are reusable framework helpers. That keeps the framework clean for the next jam, instead of fossilizing one prototype's goblin logic into every future project.

---

## Input

`InputReader` exposes:

- `Move`
- `Look`
- `Jump`
- `Attack`
- `Interact`
- `Pause`
- `Restart`
- `Debug1`
- `Debug2`

If no `InputActionAsset` is assigned, JamForge creates a default runtime action map with keyboard, mouse, WASD, and gamepad bindings.

Input callbacks are bound while `InputReader` is enabled and removed when disabled or destroyed, preventing duplicate callbacks across repeated play sessions.

---

## Audio

`AudioManager` provides simple immediate playback:

```csharp
AudioManager.Instance.PlayMusic("main_theme");
AudioManager.Instance.StopMusic();
AudioManager.Instance.PlaySfx("button_click");
```

Music fade parameters are intentionally not part of the public API. Per-entry music volume is preserved when master or music settings volume changes.

---

## Health And Damage

`Health` implements `IDamageable` and provides:

```csharp
health.Damage(1);
health.Heal(1);
health.Kill();
health.ResetHealth();
```

Use it for quick enemies, player health, destructible objects, and other jam-friendly damageable entities.

---

## Debug Keys

JamForge includes debug shortcuts for rapid testing:

| Key | Action |
| --- | --- |
| `F1` | Toggle debug overlay |
| `F5` | Reload current scene |
| `F6` | Set victory |
| `F7` | Set game over |
| `F8` | Add score |
| `F9` | Toggle time scale |

---

## Design Philosophy

JamForge should be:

- Simple
- Modular
- Easy to delete
- Easy to understand during a tired 3 AM jam session
- Useful for both 2D and 3D games
- Built for Unity 6
- Free of paid dependencies
- Free of unnecessary architecture fog

The framework should feel like a reliable folding knife, not a cathedral with wheels.

---

## Recommended Workflow

For each new jam:

1. Duplicate or import JamForge.
2. Generate the default scenes and core prefabs.
3. Build your game-specific systems outside `_JamForge`.
4. Use JamForge services only where they save time.
5. Delete or replace anything that does not fit the game.

JamForge is meant to bend, not demand tribute.

---

## Documentation

More implementation notes are available inside the project:

```text
Assets/_JamForge/README.md
```

Private planning specs are not referenced here because they are intended to stay local/private.

---

## License

JamForge is licensed under the **Apache License 2.0**.

See [`LICENSE`](LICENSE) for the full license text.

Copyright 2026 Nico Dicky Hermawan.

Third-party packages, assets, fonts, audio, or Unity Asset Store content are not automatically covered by this license. Review and keep their original licenses before making the project public.

---

## Status

Early reusable framework for Unity 6 game jam development.

Expect sharp edges, tiny dragons, and useful shortcuts.
