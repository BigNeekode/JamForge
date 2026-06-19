# Forge Toolkit

**Forge Toolkit** is a small Unity 6 framework collection for game jam and prototype development.

It currently contains two focused modules:

```text
JamForge     = make a playable prototype fast
PolishForge  = make that prototype feel better, clearer, and more shippable
```

Both modules live inside `Assets/` as separate package-style folders so they are easy to inspect, copy, upgrade, or delete.

> The goal is not to build a giant engine. The goal is to keep a reusable forge rack of practical tools that helps prototypes survive jam chaos.

---

## Modules

### JamForge

**JamForge** is a reusable Unity 6 game jam framework built to help teams start prototypes faster without dragging a giant production architecture into a 48-hour chaos kitchen.

It lives under:

```text
Assets/_JamForge
```

JamForge focuses on:

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

### PolishForge

**PolishForge** is a Unity 6 game-feel framework for prototypes and game jam projects.

It lives under:

```text
Assets/_PolishForge
```

PolishForge focuses on reusable feedback and presentation polish:

- Feedback presets through `FeedbackPreset`
- Global feedback playback through `FeedbackPlayer`
- Context-aware effect playback through `FeedbackContext`
- Camera shake
- Screen flash
- Screen fade
- Hit stop
- Time-scale curves
- Audio cues
- VFX cues and spawning
- Floating text
- Position shake
- Scale punch
- Color flash
- UI punch and shake
- Juicy buttons
- Toast notifications
- Accessibility intensity settings through `PolishSettings`
- Debug panel through `PolishDebugPanel`
- Polish checklist under `Tools/PolishForge`
- Optional JamForge integration under `Scripts/Integrations/JamForge`

Core PolishForge runtime is designed to stay independent from JamForge. JamForge should not depend on PolishForge, while PolishForge may optionally integrate with JamForge.

---

## Current Project Progress

### JamForge Status

JamForge is currently in an **early reusable framework** stage. The core scaffolding is already in place and focused on getting a Unity 6 game jam project playable quickly.

Implemented or available:

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
- MIT License added through `LICENSE`

JamForge still needs more real-project validation, especially around generated prefabs, generated scenes, WebGL builds, sample 2D/3D prototypes, and public-release cleanup.

### PolishForge Status

PolishForge has been added as the second framework module.

Implemented or available:

- Internal framework documentation at `Assets/_PolishForge/README.md`
- Runtime assembly definition: `Assets/_PolishForge/Scripts/PolishForge.Runtime.asmdef`
- Editor assembly definition: `Assets/_PolishForge/Scripts/Editor/PolishForge.Editor.asmdef`
- Optional JamForge integration assembly under `Assets/_PolishForge/Scripts/Integrations/JamForge`
- Demo scene: `Assets/_PolishForge/Scenes/PolishForge_Demo.unity`
- Core prefab: `Assets/_PolishForge/Prefabs/Core/PolishCore.prefab`
- Demo target prefab: `Assets/_PolishForge/Prefabs/Samples/PolishDemoTarget.prefab`
- Toast manager prefab
- Screen flash overlay prefab
- Sample VFX prefabs such as pickup sparkle and enemy death poof
- Feedback preset assets for button click, enemy hit, pickup, error, and game over
- VFX cue assets
- Core feedback system through `FeedbackPreset`, `FeedbackEffect`, `FeedbackContext`, and `FeedbackPlayer`
- Effect assets for camera shake, screen flash, screen fade, hit stop, audio cues, VFX spawning, scale punch, color flash, floating text, position shake, time-scale curves, and rumble
- UI helpers including juicy button, number counter, UI punch, UI shake, UI fade/slide, toast notification, and toast manager
- Motion helpers including bob motion, position shake, squash/stretch, sprite blink, target resolving, and target mode helpers
- Accessibility settings through `PolishSettings`
- Debug tooling through `PolishDebugPanel` and demo action scripts
- Editor polish checklist under `Tools > PolishForge > Polish Checklist`

PolishForge still needs Unity-side validation in clean projects, demo-scene testing, preset tuning, WebGL testing, rumble behavior checks, accessibility menu UI, and more integration examples.

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

Cinemachine can be useful for jam cameras, but core systems are designed not to depend on it.

---

## Quick Start

### JamForge

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

### PolishForge

1. Keep the `Assets/_PolishForge` folder in the project.
2. Add `PolishCore` to a boot/demo scene, or allow services to auto-create when first used.
3. Create a preset from:

```text
Create > PolishForge > Feedback Preset
```

4. Add effect assets such as `CameraShakeEffect`, `ScreenFlashEffect`, `HitStopEffect`, `AudioCueEffect`, `VfxSpawnEffect`, `ScalePunchEffect`, `ColorFlashEffect`, or `FloatingTextEffect`.
5. Trigger the preset from code or from a `FeedbackTrigger` UnityEvent.

Example:

```csharp
FeedbackPlayer.PlayGlobal(enemyHitPreset, new FeedbackContext
{
    Source = player,
    Target = enemy,
    Position = enemy.transform.position,
    Intensity = 1f,
    Amount = damage
});
```

---

## Folder Structure

The toolkit is designed to keep each module isolated:

```text
Assets/
  _JamForge/
    Art/
    Audio/
    Prefabs/
    Scenes/
    Scripts/
    ScriptableObjects/
    Resources/
    Settings/

  _PolishForge/
    Prefabs/
      Core/
      Samples/
      ScreenEffects/
      UI/
      VFX/
    Scenes/
      PolishForge_Demo.unity
    Scripts/
      Accessibility/
      Animation/
      Core/
      Debugging/
      Editor/
      Effects/
      Integrations/
      Screen/
      UI/
      Utilities/
    ScriptableObjects/
      FeedbackPresets/
      VfxCues/
```

Keep game-specific scripts outside `_JamForge` and `_PolishForge` unless they are reusable framework helpers.

---

## JamForge Debug Keys

| Key | Action |
| --- | --- |
| `F1` | Toggle debug overlay |
| `F5` | Reload current scene |
| `F6` | Set victory |
| `F7` | Set game over |
| `F8` | Add score |
| `F9` | Toggle time scale |

---

## PolishForge Debug Keys

| Key | Action |
| --- | --- |
| `F2` | Toggle `PolishDebugPanel` in editor/development builds |

---

## Design Philosophy

The toolkit should be:

- Simple
- Modular
- Easy to delete
- Easy to understand during a tired 3 AM jam session
- Useful for both 2D and 3D games
- Built for Unity 6
- Free of paid dependencies
- Free of unnecessary architecture fog

Dependency rule:

```text
JamForge should not depend on PolishForge.
PolishForge may optionally integrate with JamForge.
```

The framework should feel like a reliable folding knife, not a cathedral with wheels.

---

## Recommended Workflow

For each new jam:

1. Start with JamForge to get the project playable.
2. Build game-specific systems outside the framework folders.
3. Once the loop works, add PolishForge feedback presets for repeated moments.
4. Tune the polish intensity through `PolishSettings`.
5. Use debug panels and checklists before building.
6. Delete or replace anything that does not fit the game.

JamForge is meant to bend. PolishForge is meant to sparkle without taking over the castle.

---

## Documentation

More implementation notes are available inside the project:

```text
Assets/_JamForge/README.md
Assets/_PolishForge/README.md
```

A Notion documentation page also exists for PolishForge.

Private planning specs are not referenced here because they are intended to stay local/private.

---

## License

Forge Toolkit is licensed under the **MIT License**.

See [`LICENSE`](LICENSE) for the full license text.

Copyright 2026 Nico Dicky Hermawan.

Third-party packages, assets, fonts, audio, or Unity Asset Store content are not automatically covered by this license. Review and keep their original licenses before making the project public.

---

## Status

Early reusable Unity 6 framework collection for game jam development.

Expect sharp edges, tiny dragons, useful shortcuts, and occasional sparks from the anvil.
