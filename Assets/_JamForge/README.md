# JamForge

JamForge is a small Unity 6 game jam framework. It lives under `Assets/_JamForge` so it can be inspected, copied, or deleted without hunting through the project.

## What Is Included

- Scene loading through `SceneLoader`
- Persistent core services with `PersistentSingleton`
- Runtime bootstrap safety for direct scene testing
- Game state events through `GameStateManager` and `JamEventBus`
- Input System wrapper through `InputReader`
- UGUI/TMP screen helpers through `UIManager` and `UIScreen`
- PlayerPrefs-backed settings through `SettingsManager`
- Simple audio library and `AudioManager`
- Object pooling, health, damage, interaction, score, timer, projectiles, camera helpers, sample controllers, and debug tools
- Editor menu helpers under `Tools/JamForge`

## Quick Start

1. Open Unity.
2. Use `Tools/JamForge/Create Jam Scenes` to create the default scene files.
3. Use `Tools/JamForge/Create Core Prefabs` to create starter prefabs.
4. Put `JamBootstrapper` in `00_Boot`.
5. Add scenes to Build Settings in this order: `00_Boot`, `01_MainMenu`, `02_Game`, `03_GameOver`.
6. Press Play from `00_Boot`.

Directly opening a game scene also works because `JamRuntime` creates missing core services before scene load.

## Input

`InputReader` exposes `Move`, `Look`, and events for `Jump`, `Attack`, `Interact`, `Pause`, `Restart`, `Debug1`, and `Debug2`. If no `InputActionAsset` is assigned, it creates a default runtime action map with WASD, mouse, keyboard, and gamepad bindings.

## Debug Keys

- `F1`: Toggle debug overlay
- `F5`: Reload current scene
- `F6`: Set victory
- `F7`: Set game over
- `F8`: Add score
- `F9`: Toggle time scale

## Notes

Keep game-specific scripts outside `_JamForge` unless they are reusable framework helpers. JamForge is intended to be simple jam scaffolding, not a production architecture.
