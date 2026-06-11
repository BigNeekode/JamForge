# PolishForge Spec

## Unity 6 Game Polishing Framework

PolishForge is a reusable polishing and game-feel framework for Unity 6. It is designed to be used after a playable prototype already exists, especially after using a prototype framework like JamForge.

JamForge answers:

> Can the game be played?

PolishForge answers:

> Does the game feel good, readable, responsive, satisfying, and presentable?

The framework should add feedback, juice, clarity, accessibility, and presentation polish without changing the core gameplay design.

---

# 1. Project Identity

## Name

`PolishForge`

## Target Engine

Unity 6

## Target Use Case

Short-duration game jams, prototypes, and small indie games that already have working gameplay but need fast polish.

## Main Goals

PolishForge should help developers quickly add:

- Camera shake
- Screen flash
- Hit stop
- Audio variation
- UI animation
- VFX spawning
- Floating text
- Button feedback
- Object punch effects
- Color flash effects
- Low health feedback
- Victory/game over presentation
- Accessibility controls for intense effects
- A polish checklist for final jam preparation

## Design Philosophy

PolishForge must be:

- Fast to use
- Easy to remove
- Easy to understand
- Data-driven where useful
- Inspector-friendly
- Optional and modular
- Compatible with 2D and 3D games
- Usable with or without JamForge
- Safe for game jam teams

PolishForge must not become a huge visual scripting tool, node graph editor, or custom engine inside Unity.

---

# 2. Relationship With JamForge

PolishForge should work independently.

However, it may include an optional adapter for JamForge.

## JamForge Handles

- Scene flow
- Input
- Game state
- UI structure
- Audio base
- Object pooling
- Health/damage
- Interaction
- Timer/score
- Sample controllers

## PolishForge Handles

- Feedback presets
- Camera polish
- Screen effects
- Hit stop
- Audio reaction
- VFX reaction
- UI juice
- Animation helpers
- Floating text
- Accessibility for polish intensity
- Polish checklist

## Dependency Rule

The core `PolishForge` namespace must not depend on `JamForge`.

Optional integration should live in:

```text
Assets/_PolishForge/Scripts/Integrations/JamForge/
```

Example:

```text
PolishForge.JamForgeIntegration
```

---

# 3. Folder Structure

Create this folder structure:

```text
Assets/
  _PolishForge/
    Art/
      Materials/
      Sprites/
    Audio/
      SFX/
      Music/
      Mixers/
    Prefabs/
      Core/
      UI/
      VFX/
      ScreenEffects/
      Samples/
    Scenes/
      PolishForge_Demo.unity
    Scripts/
      Core/
      Effects/
      Camera/
      Screen/
      UI/
      Audio/
      VFX/
      Animation/
      Accessibility/
      Debugging/
      Utilities/
      Integrations/
        JamForge/
      Editor/
    ScriptableObjects/
      FeedbackPresets/
      AudioCues/
      VfxCues/
      Settings/
    Resources/
    Settings/
    README.md
```

---

# 4. Assembly Definitions

Create assembly definitions:

```text
Assets/_PolishForge/Scripts/PolishForge.Runtime.asmdef
Assets/_PolishForge/Scripts/Editor/PolishForge.Editor.asmdef
```

Optional integration assembly:

```text
Assets/_PolishForge/Scripts/Integrations/JamForge/PolishForge.JamForgeIntegration.asmdef
```

Rules:

- Runtime assembly must not reference editor-only code.
- Editor assembly may reference runtime assembly.
- JamForge integration assembly may reference both PolishForge and JamForge if JamForge exists.
- Core PolishForge runtime must compile without JamForge.

---

# 5. Core Concept

The central pattern is:

```text
Gameplay Event -> Feedback Preset -> Multiple Feedback Effects
```

Example:

```text
Player takes damage
-> Camera shake
-> Screen flash
-> Hit stop
-> Damage SFX
-> Player color flash
-> Health UI punch
```

Gameplay code should not directly know about camera shake, screen flash, or UI punch. It should only trigger a feedback preset.

---

# 6. Core Architecture

## Core Classes

Create:

```text
Assets/_PolishForge/Scripts/Core/FeedbackPreset.cs
Assets/_PolishForge/Scripts/Core/FeedbackEffect.cs
Assets/_PolishForge/Scripts/Core/FeedbackContext.cs
Assets/_PolishForge/Scripts/Core/FeedbackPlayer.cs
Assets/_PolishForge/Scripts/Core/FeedbackTrigger.cs
```

---

# 7. FeedbackContext

Create a lightweight struct used to pass information into feedback effects.

```csharp
namespace PolishForge
{
    public struct FeedbackContext
    {
        public GameObject Source;
        public GameObject Target;
        public Vector3 Position;
        public Transform Anchor;
        public float Intensity;
        public string Label;
        public int Amount;
    }
}
```

## Rules

- `Source` is the object that caused the feedback.
- `Target` is the object receiving feedback.
- `Position` is the world position where feedback should happen.
- `Anchor` is optional and can be used for attached effects.
- `Intensity` defaults to `1`.
- `Label` is optional for text effects.
- `Amount` is optional for damage/score effects.

Acceptance criteria:

- Feedback effects can use the same context.
- Context can be constructed manually from gameplay code.
- Missing optional fields should not crash effects.

---

# 8. FeedbackEffect

Use ScriptableObject-based effect assets.

Create:

```csharp
namespace PolishForge
{
    public abstract class FeedbackEffect : ScriptableObject
    {
        public string DisplayName = "Feedback Effect";
        public float Delay;
        public bool UseUnscaledTime;

        public abstract IEnumerator Play(FeedbackContext context);
    }
}
```

## Rules

- Effects are ScriptableObjects.
- Each effect handles exactly one kind of polish.
- Effects should be composable inside a FeedbackPreset.
- Effects may use coroutines.
- Effects must not assume JamForge exists.
- Effects should fail gracefully if required scene objects are missing.

Acceptance criteria:

- A custom effect can be created by inheriting `FeedbackEffect`.
- Effects can be placed inside a `FeedbackPreset`.
- Delayed effects work.

---

# 9. FeedbackPreset

Create:

```csharp
namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Feedback Preset")]
    public class FeedbackPreset : ScriptableObject
    {
        public List<FeedbackEffect> Effects = new();
        public bool PlayInParallel = true;
    }
}
```

## Rules

- Presets are ScriptableObjects.
- Presets can contain multiple effects.
- Effects can play in parallel or sequentially.
- Presets can be reused across multiple objects.

Acceptance criteria:

- Developer can create a feedback preset from Unity Create menu.
- Developer can add effects to the preset.
- Calling the preset plays all effects.

---

# 10. FeedbackPlayer

Create:

```text
Assets/_PolishForge/Scripts/Core/FeedbackPlayer.cs
```

API:

```csharp
namespace PolishForge
{
    public class FeedbackPlayer : MonoBehaviour
    {
        public static FeedbackPlayer Instance { get; }

        public void Play(FeedbackPreset preset, FeedbackContext context);
        public static void PlayGlobal(FeedbackPreset preset, FeedbackContext context);
    }
}
```

## Rules

- Can be used as a scene object.
- Can auto-create itself if missing.
- Must run coroutines for feedback effects.
- Must support parallel and sequential preset playback.
- Missing preset should log warning, not crash.
- Missing effect inside preset should log warning, not crash.

Acceptance criteria:

- Calling `FeedbackPlayer.PlayGlobal(preset, context)` plays preset.
- Multiple presets can play at the same time.
- Sequential and parallel modes both work.

---

# 11. FeedbackTrigger

Create:

```text
Assets/_PolishForge/Scripts/Core/FeedbackTrigger.cs
```

Purpose:

A simple MonoBehaviour that plays a selected feedback preset from Unity Events or gameplay scripts.

API:

```csharp
namespace PolishForge
{
    public class FeedbackTrigger : MonoBehaviour
    {
        [SerializeField] private FeedbackPreset preset;
        [SerializeField] private Transform anchor;
        [SerializeField] private float intensity = 1f;

        public void Play();
        public void PlayAtPosition(Vector3 position);
        public void PlayWithTarget(GameObject target);
    }
}
```

Acceptance criteria:

- Can be attached to any GameObject.
- Can be called from Unity Button onClick.
- Can play feedback at object position.
- Can override target and position.

---

# 12. Required Feedback Effects

Implement these effect ScriptableObjects:

```text
CameraShakeEffect
ScreenFlashEffect
ScreenFadeEffect
HitStopEffect
AudioCueEffect
VfxSpawnEffect
ScalePunchEffect
PositionShakeEffect
ColorFlashEffect
FloatingTextEffect
RumbleEffect
TimeScaleCurveEffect
```

Each effect should live in:

```text
Assets/_PolishForge/Scripts/Effects/
```

---

# 13. CameraShakeEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/CameraShakeEffect.cs
Assets/_PolishForge/Scripts/Camera/PolishCameraShake.cs
```

Effect settings:

```text
Duration
Strength
Frequency
UseUnscaledTime
ScaleByContextIntensity
```

API:

```csharp
public class PolishCameraShake : MonoBehaviour
{
    public static PolishCameraShake Instance { get; }
    public void Shake(float duration, float strength, float frequency = 25f);
}
```

Rules:

- Must work without Cinemachine.
- Transform-based shake is required.
- Cinemachine support is optional and should be adapter-based.
- Shake must return camera to original position/offset.
- Multiple shakes should blend or override safely.

Acceptance criteria:

- Playing effect shakes the main camera.
- Shake stops after duration.
- Camera does not drift permanently.

---

# 14. ScreenFlashEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/ScreenFlashEffect.cs
Assets/_PolishForge/Scripts/Screen/ScreenFlashOverlay.cs
```

Effect settings:

```text
Color
Duration
MaxAlpha
AnimationCurve
UseUnscaledTime
ScaleByAccessibility
```

Rules:

- Use a full-screen UI Image overlay.
- Must auto-create overlay if missing.
- Must respect PolishSettings flash intensity.
- Must be disable-able through settings.

Acceptance criteria:

- Screen flashes when effect plays.
- Flash fades out smoothly.
- Flash intensity setting affects result.
- Flash can be disabled.

---

# 15. ScreenFadeEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/ScreenFadeEffect.cs
Assets/_PolishForge/Scripts/Screen/ScreenFadeOverlay.cs
```

Effect settings:

```text
FadeColor
FadeInDuration
HoldDuration
FadeOutDuration
TargetAlpha
UseUnscaledTime
```

Use cases:

- Scene transition
- Victory presentation
- Game over presentation
- Dream/faint effect

Acceptance criteria:

- Screen can fade in and out.
- Fade can be called from feedback preset.
- Fade works while game is paused.

---

# 16. HitStopEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/HitStopEffect.cs
Assets/_PolishForge/Scripts/Core/HitStopController.cs
```

Effect settings:

```text
Duration
TimeScaleDuringStop
UseUnscaledWait
ScaleByContextIntensity
```

Rules:

- Default hit stop duration should be tiny.
- Must restore previous timescale safely.
- Must not permanently freeze game.
- Must handle overlapping hit stops.

Recommended defaults:

```text
Small hit: 0.035 seconds
Medium hit: 0.06 seconds
Heavy hit: 0.1 seconds
```

Acceptance criteria:

- Playing effect briefly freezes or slows game.
- Time scale returns to previous value.
- Repeated hit stop does not break time scale.

---

# 17. AudioCueEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/AudioCueEffect.cs
Assets/_PolishForge/Scripts/Audio/PolishAudioCue.cs
Assets/_PolishForge/Scripts/Audio/PolishAudioPlayer.cs
```

Audio cue ScriptableObject:

```csharp
[CreateAssetMenu(menuName = "PolishForge/Audio Cue")]
public class PolishAudioCue : ScriptableObject
{
    public List<AudioClip> Clips;
    public Vector2 VolumeRange = new Vector2(0.9f, 1f);
    public Vector2 PitchRange = new Vector2(0.95f, 1.05f);
    public float Cooldown = 0f;
    public bool Spatial;
    public float SpatialBlend = 1f;
}
```

Effect settings:

```text
AudioCue
PlayAtContextPosition
AttachToTarget
ScaleVolumeByIntensity
```

Rules:

- Randomly choose clip from cue.
- Randomize volume and pitch.
- Respect cooldown.
- Missing clip should warn, not crash.
- Support 2D and 3D audio.

Acceptance criteria:

- Audio cue plays random clip.
- Pitch variation works.
- Cooldown prevents spam.
- 3D positioned audio works.

---

# 18. VfxSpawnEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/VfxSpawnEffect.cs
Assets/_PolishForge/Scripts/VFX/VfxCue.cs
Assets/_PolishForge/Scripts/VFX/VfxSpawner.cs
Assets/_PolishForge/Scripts/VFX/PooledVfx.cs
```

VfxCue:

```csharp
[CreateAssetMenu(menuName = "PolishForge/VFX Cue")]
public class VfxCue : ScriptableObject
{
    public GameObject Prefab;
    public float Lifetime = 2f;
    public bool UsePooling = true;
    public bool AttachToAnchor;
    public Vector3 Offset;
}
```

Rules:

- Spawn at context position by default.
- Optional attach to anchor/target.
- Auto-despawn after lifetime.
- Use simple internal pooling if enabled.

Acceptance criteria:

- VFX prefab spawns at correct position.
- VFX despawns after lifetime.
- Pooling prevents constant Instantiate spam.

---

# 19. ScalePunchEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/ScalePunchEffect.cs
Assets/_PolishForge/Scripts/Animation/ScalePunch.cs
```

Effect settings:

```text
TargetMode: ContextTarget, ContextSource, Anchor, SpecificTransform
PunchScale
Duration
AnimationCurve
UseUnscaledTime
```

Rules:

- Store original scale.
- Return to original scale after punch.
- Support UI and world objects.

Acceptance criteria:

- Object grows/shrinks briefly.
- Object returns to original scale.
- Repeated punch does not permanently change scale.

---

# 20. PositionShakeEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/PositionShakeEffect.cs
Assets/_PolishForge/Scripts/Animation/PositionShake.cs
```

Effect settings:

```text
TargetMode
Duration
Strength
Frequency
UseUnscaledTime
```

Use cases:

- Health icon shake
- Enemy hurt shake
- Button error shake
- Chest locked shake

Acceptance criteria:

- Target shakes around original position.
- Target returns to original position.
- Works on UI RectTransform and normal Transform.

---

# 21. ColorFlashEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/ColorFlashEffect.cs
Assets/_PolishForge/Scripts/Animation/ColorFlasher.cs
```

Effect settings:

```text
FlashColor
Duration
RendererTargetMode
IncludeChildren
UseMaterialPropertyBlock
```

Rules:

- Support SpriteRenderer.
- Support MeshRenderer.
- Prefer MaterialPropertyBlock for MeshRenderer.
- Restore original color.
- Missing renderer should warn, not crash.

Acceptance criteria:

- Sprite can flash white/red.
- Mesh can flash white/red.
- Original color is restored.

---

# 22. FloatingTextEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/FloatingTextEffect.cs
Assets/_PolishForge/Scripts/UI/FloatingText.cs
Assets/_PolishForge/Scripts/UI/FloatingTextSpawner.cs
```

Effect settings:

```text
TextMode: ContextLabel, ContextAmount, FixedText
TextPrefab
Color
Duration
WorldOffset
RiseDistance
RandomSpread
```

Rules:

- Support world-space floating text.
- Support screen-space UI floating text if possible.
- Use pooling.
- Text should fade and move upward.

Acceptance criteria:

- Damage number appears above target.
- Score text can appear at pickup.
- Text fades and despawns.

---

# 23. RumbleEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/RumbleEffect.cs
Assets/_PolishForge/Scripts/Input/RumbleController.cs
```

Rules:

- Use Unity Input System gamepad rumble if available.
- Must do nothing safely if no gamepad exists.
- Must respect PolishSettings rumble intensity.
- Must stop rumble after duration.

Effect settings:

```text
LowFrequency
HighFrequency
Duration
ScaleByContextIntensity
```

Acceptance criteria:

- Gamepad rumble triggers when gamepad is connected.
- No error occurs when no gamepad is connected.
- Rumble stops after duration.

---

# 24. TimeScaleCurveEffect

Create:

```text
Assets/_PolishForge/Scripts/Effects/TimeScaleCurveEffect.cs
```

Purpose:

For dramatic slow-motion bursts, victory moments, boss death, perfect dodge, and similar effects.

Effect settings:

```text
Duration
AnimationCurve TimeScaleCurve
RestorePreviousTimeScale
UseUnscaledTime
```

Rules:

- Must restore time scale.
- Must not conflict permanently with pause systems.
- Should be used carefully and kept optional.

Acceptance criteria:

- Time scale follows curve.
- Previous time scale is restored.

---

# 25. UI Juice Components

Create:

```text
Assets/_PolishForge/Scripts/UI/JuicyButton.cs
Assets/_PolishForge/Scripts/UI/UIPunch.cs
Assets/_PolishForge/Scripts/UI/UIShake.cs
Assets/_PolishForge/Scripts/UI/UIFadeSlide.cs
Assets/_PolishForge/Scripts/UI/NumberCounter.cs
Assets/_PolishForge/Scripts/UI/ToastNotification.cs
Assets/_PolishForge/Scripts/UI/ToastManager.cs
```

## JuicyButton

Features:

- Hover scale
- Press scale
- Click sound
- Optional feedback preset
- Works with Unity UI Button

Acceptance criteria:

- Button scales on hover.
- Button punches on click.
- Click sound plays.
- Optional feedback preset plays.

## NumberCounter

Features:

- Smoothly animate number from old value to new value.
- Support integer values.
- Optional prefix/suffix.
- Optional punch on change.

Acceptance criteria:

- Score smoothly ticks up.
- Counter shows final exact value.

## ToastNotification

Features:

- Show short message.
- Fade in.
- Stay.
- Fade out.
- Queue multiple messages.

Acceptance criteria:

- Toast appears and disappears.
- Multiple toasts queue correctly.

---

# 26. Animation Helpers

Create:

```text
Assets/_PolishForge/Scripts/Animation/SquashStretch.cs
Assets/_PolishForge/Scripts/Animation/BobMotion.cs
Assets/_PolishForge/Scripts/Animation/RotateMotion.cs
Assets/_PolishForge/Scripts/Animation/SpriteBlink.cs
Assets/_PolishForge/Scripts/Animation/ImpactNudge.cs
```

## SquashStretch

Use cases:

- Landing
- Jumping
- Enemy hit
- Pickup bounce

Acceptance criteria:

- Object can squash and return to original scale.

## BobMotion

Use cases:

- Floating pickups
- Menu icons
- Interactable objects

Acceptance criteria:

- Object moves up/down smoothly.

## RotateMotion

Use cases:

- Coins
- Pickups
- Menu decoration

Acceptance criteria:

- Object rotates at configured speed.

## SpriteBlink

Use cases:

- Invulnerability
- Hurt feedback
- Warning state

Acceptance criteria:

- Sprite blinks for duration and returns visible.

## ImpactNudge

Use cases:

- Enemy hit push visual
- Player damage recoil visual
- UI icon bump

Acceptance criteria:

- Object nudges in direction and returns/stabilizes.

---

# 27. PolishSettings

Create:

```text
Assets/_PolishForge/Scripts/Accessibility/PolishSettings.cs
```

Settings:

```text
MasterPolishIntensity
CameraShakeIntensity
ScreenFlashIntensity
RumbleIntensity
EnableScreenFlash
EnableCameraShake
EnableRumble
EnableHitStop
EnableSlowMotion
EnableFloatingText
```

API:

```csharp
public class PolishSettings : MonoBehaviour
{
    public static PolishSettings Instance { get; }

    public float MasterPolishIntensity { get; set; }
    public float CameraShakeIntensity { get; set; }
    public float ScreenFlashIntensity { get; set; }
    public float RumbleIntensity { get; set; }

    public bool EnableScreenFlash { get; set; }
    public bool EnableCameraShake { get; set; }
    public bool EnableRumble { get; set; }
    public bool EnableHitStop { get; set; }
    public bool EnableSlowMotion { get; set; }
    public bool EnableFloatingText { get; set; }

    public void Load();
    public void Save();
    public void ResetToDefaults();
}
```

Rules:

- Use PlayerPrefs for settings.
- Defaults should be comfortable, not extreme.
- All intense feedback must respect settings.
- Settings should be independent from JamForge settings.

Acceptance criteria:

- Camera shake slider affects shake strength.
- Screen flash slider affects flash alpha.
- Disabling hit stop prevents hit stop effects.
- Settings persist.

---

# 28. Polish Debug Panel

Create:

```text
Assets/_PolishForge/Scripts/Debugging/PolishDebugPanel.cs
```

Features:

- Toggle with F2
- Preview selected feedback preset
- Adjust polish intensity at runtime
- Test camera shake
- Test screen flash
- Test hit stop
- Test floating text
- Display active effects count

Acceptance criteria:

- F2 toggles panel in editor/development builds.
- Test buttons trigger effects.
- Panel does not appear in release builds unless enabled.

---

# 29. Polish Checklist Editor Window

Create:

```text
Assets/_PolishForge/Scripts/Editor/PolishChecklistWindow.cs
```

Menu item:

```text
Tools/PolishForge/Polish Checklist
```

Checklist categories:

```text
Input Feedback
Player Feedback
Enemy Feedback
Camera Feel
Audio Feel
UI Clarity
Screen Effects
Accessibility
Game Flow
Build Readiness
Performance
```

Checklist items:

```text
Every button has hover/click feedback.
Player damage has visual feedback.
Player damage has audio feedback.
Enemy damage has visual feedback.
Enemy death has VFX or animation.
Pickups have sound and visual feedback.
Camera shake is not too strong.
Screen flash can be reduced or disabled.
Hit stop does not break pause.
Game over has clear feedback.
Victory has clear feedback.
Score changes are readable.
Low health state is visible/audible.
Audio is not clipping.
There is a restart option.
There is a main menu option.
Build has been tested.
WebGL build has been tested if used.
```

Rules:

- Checklist state may be stored in EditorPrefs.
- This is an editor-only tool.
- Must not affect runtime builds.

Acceptance criteria:

- Window opens from Tools menu.
- Checklist items can be toggled.
- Checklist state persists in editor.

---

# 30. Preset Assets

Create default ScriptableObject presets:

```text
Assets/_PolishForge/ScriptableObjects/FeedbackPresets/
  PlayerHitFeedback.asset
  EnemyHitFeedback.asset
  EnemyDeathFeedback.asset
  PickupFeedback.asset
  ButtonClickFeedback.asset
  VictoryFeedback.asset
  GameOverFeedback.asset
  ErrorFeedback.asset
```

Create default audio cue assets:

```text
Assets/_PolishForge/ScriptableObjects/AudioCues/
  ButtonClickCue.asset
  PickupCue.asset
  HitCue.asset
  ErrorCue.asset
```

Create default VFX cue assets:

```text
Assets/_PolishForge/ScriptableObjects/VfxCues/
  HitSparkCue.asset
  PickupSparkleCue.asset
  EnemyDeathPoofCue.asset
```

Acceptance criteria:

- Presets exist and can be assigned.
- Presets can work with placeholder audio/VFX.
- Missing placeholder assets should not break compilation.

---

# 31. Prefabs

Create prefabs:

```text
Assets/_PolishForge/Prefabs/Core/PolishCore.prefab
Assets/_PolishForge/Prefabs/ScreenEffects/ScreenFlashOverlay.prefab
Assets/_PolishForge/Prefabs/ScreenEffects/ScreenFadeOverlay.prefab
Assets/_PolishForge/Prefabs/UI/FloatingText.prefab
Assets/_PolishForge/Prefabs/UI/ToastManager.prefab
Assets/_PolishForge/Prefabs/VFX/HitSpark.prefab
Assets/_PolishForge/Prefabs/VFX/PickupSparkle.prefab
Assets/_PolishForge/Prefabs/Samples/PolishDemoTarget.prefab
```

`PolishCore` should contain:

```text
FeedbackPlayer
PolishCameraShake
PolishAudioPlayer
PolishSettings
ScreenFlashOverlay
ScreenFadeOverlay
FloatingTextSpawner
ToastManager
```

Acceptance criteria:

- Dragging `PolishCore` into a scene enables framework services.
- Duplicate PolishCore instances are handled safely.
- Feedback effects can auto-find required services.

---

# 32. Sample Scene

Create:

```text
Assets/_PolishForge/Scenes/PolishForge_Demo.unity
```

Demo requirements:

- A target object can be hit.
- Hitting target triggers EnemyHitFeedback.
- Killing target triggers EnemyDeathFeedback.
- Button demonstrates ButtonClickFeedback.
- Pickup demonstrates PickupFeedback.
- Victory button triggers VictoryFeedback.
- Game over button triggers GameOverFeedback.
- Sliders demonstrate camera shake, screen flash, and rumble intensity.
- F2 debug panel works.

Acceptance criteria:

- Demo scene proves every V1 effect works.
- No compile errors.
- No missing scripts.
- No hard dependency on JamForge.

---

# 33. Optional JamForge Integration

Create only if JamForge exists:

```text
Assets/_PolishForge/Scripts/Integrations/JamForge/JamForgeFeedbackBridge.cs
```

Purpose:

Bridge JamForge gameplay events into PolishForge presets.

Possible mappings:

```text
Health damaged -> PlayerHitFeedback or EnemyHitFeedback
Health died -> EnemyDeathFeedback or GameOverFeedback
Score changed -> Score UI punch
GameState Victory -> VictoryFeedback
GameState GameOver -> GameOverFeedback
```

Rules:

- Keep this optional.
- Use assembly definition constraints if possible.
- Do not make core PolishForge depend on JamForge.

Acceptance criteria:

- JamForge project can use bridge.
- Non-JamForge project still compiles.

---

# 34. Coding Style

Rules:

- Use namespace `PolishForge`.
- Use `[SerializeField] private` fields instead of public fields where possible.
- Public APIs should be small and obvious.
- Prefer composition over inheritance.
- Use ScriptableObject for reusable data/presets.
- Use MonoBehaviour for scene execution.
- Use coroutine-based effects.
- Avoid reflection.
- Avoid complex dependency injection.
- Avoid custom node editors in V1.
- Avoid external paid dependencies.
- Do not require DOTween.
- Do not require Cinemachine.
- Optional adapters are allowed.

---

# 35. Recommended Code Patterns

## Pattern Summary

Use these patterns:

```text
Feedback Preset Pattern
ScriptableObject Data Pattern
Small Runtime Service Singleton
Coroutine Effect Pattern
Context Object Pattern
Optional Adapter Pattern
Inspector-First Component Pattern
```

Avoid these patterns in V1:

```text
Complex dependency injection
Reflection-heavy auto binding
Custom graph editor
Event spaghetti
Huge manager classes
Inheritance-heavy hierarchies
Async architecture
Addressables by default
```

---

## 35.1 Feedback Preset Pattern

Use `FeedbackPreset` as the main user-facing data asset.

Good:

```text
EnemyHitFeedback.asset
PickupFeedback.asset
ButtonClickFeedback.asset
VictoryFeedback.asset
```

A preset should be a list of effects.

This makes polish reusable and easy to tune without editing gameplay code.

---

## 35.2 ScriptableObject Data Pattern

Use ScriptableObjects for:

```text
FeedbackPreset
FeedbackEffect
PolishAudioCue
VfxCue
PolishConfig
```

Reason:

- Easy to duplicate
- Easy to tune
- Easy for non-programmers
- Works well in Unity inspector
- Good for game jams

---

## 35.3 Small Runtime Service Singleton

Allowed singleton services:

```text
FeedbackPlayer
PolishCameraShake
PolishAudioPlayer
PolishSettings
FloatingTextSpawner
ToastManager
```

Rules:

- Singletons should be small.
- They should provide obvious services.
- They should be safe when missing.
- They should not control gameplay logic.

Do not turn PolishCore into a giant god object.

---

## 35.4 Coroutine Effect Pattern

Use coroutines for timed feedback:

```text
Flash for 0.1 seconds
Shake for 0.2 seconds
Fade for 0.5 seconds
Punch scale for 0.15 seconds
Hit stop for 0.05 seconds
```

Each `FeedbackEffect` returns an `IEnumerator`.

This keeps effects simple and Unity-friendly.

---

## 35.5 Context Object Pattern

Use `FeedbackContext` instead of many method overloads.

Good:

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

Reason:

- Flexible
- Easy to extend
- Effects can read only what they need
- Avoids messy APIs

---

## 35.6 Optional Adapter Pattern

Core PolishForge must not depend on JamForge.

Use adapters for integrations:

```text
PolishForge.JamForgeIntegration
PolishForge.CinemachineIntegration
PolishForge.URPIntegration
```

Adapters may depend on external packages or frameworks.

Core does not.

---

## 35.7 Inspector-First Component Pattern

Components should be easy to use by dragging into the inspector.

Example:

```text
Add FeedbackTrigger to object
Assign FeedbackPreset
Call Play from UnityEvent or script
```

This is better for game jams than requiring code for every interaction.

---

# 36. Error Handling

Rules:

- Missing preset logs warning.
- Missing effect logs warning.
- Missing camera logs warning.
- Missing audio clip logs warning.
- Missing VFX prefab logs warning.
- Missing target logs warning only if target is required.
- No effect should crash the game during a jam.

Warning message format:

```text
[PolishForge] ComponentName on ObjectName: clear warning message.
```

Example:

```text
[PolishForge] AudioCueEffect: Audio cue has no clips.
```

---

# 37. Accessibility Requirements

Every intense feedback effect must respect PolishSettings.

## Must Respect Settings

```text
CameraShakeEffect -> CameraShakeIntensity, EnableCameraShake
ScreenFlashEffect -> ScreenFlashIntensity, EnableScreenFlash
RumbleEffect -> RumbleIntensity, EnableRumble
HitStopEffect -> EnableHitStop
TimeScaleCurveEffect -> EnableSlowMotion
FloatingTextEffect -> EnableFloatingText
```

Acceptance criteria:

- User can reduce shake.
- User can disable flash.
- User can disable rumble.
- User can disable hit stop.
- These settings persist.

---

# 38. Performance Requirements

Rules:

- Pool floating text.
- Pool common VFX.
- Avoid allocating heavily every frame.
- Avoid constant FindObjectOfType calls.
- Cache component references.
- Do not instantiate materials repeatedly.
- Use MaterialPropertyBlock for mesh color flashes.
- Effects should be short-lived and lightweight.

Acceptance criteria:

- Demo scene runs smoothly.
- Repeated feedback triggers do not create unbounded objects.
- No obvious memory leak from pooled VFX or floating text.

---

# 39. README

Create:

```text
Assets/_PolishForge/README.md
```

README sections:

```text
What is PolishForge?
How to install
How to add PolishCore
How to create a FeedbackPreset
How to trigger feedback from code
How to trigger feedback from UnityEvent
How to create an AudioCue
How to create a VfxCue
How to use UI juice components
How to adjust accessibility settings
How to use the debug panel
How to use the polish checklist
How to integrate with JamForge
Recommended polish checklist before submission
Known limitations
```

---

# 40. Implementation Order

Implement in this order:

```text
Phase 1: Folder structure and assembly definitions
Phase 2: FeedbackContext, FeedbackEffect, FeedbackPreset
Phase 3: FeedbackPlayer and FeedbackTrigger
Phase 4: PolishSettings
Phase 5: CameraShakeEffect and PolishCameraShake
Phase 6: ScreenFlashEffect and ScreenFlashOverlay
Phase 7: HitStopEffect and HitStopController
Phase 8: AudioCueEffect, PolishAudioCue, PolishAudioPlayer
Phase 9: VfxSpawnEffect, VfxCue, VfxSpawner, PooledVfx
Phase 10: ScalePunchEffect and ScalePunch
Phase 11: PositionShakeEffect and PositionShake
Phase 12: ColorFlashEffect and ColorFlasher
Phase 13: FloatingTextEffect and FloatingTextSpawner
Phase 14: RumbleEffect
Phase 15: TimeScaleCurveEffect
Phase 16: UI juice components
Phase 17: Animation helpers
Phase 18: PolishDebugPanel
Phase 19: PolishChecklistWindow
Phase 20: Default presets/assets/prefabs
Phase 21: Demo scene
Phase 22: Optional JamForge integration
Phase 23: README
```

---

# 41. Definition of Done

PolishForge is complete when:

- Project compiles in Unity 6.
- Core runtime compiles without JamForge.
- No missing scripts in demo scene.
- `PolishCore` prefab works when dragged into a scene.
- Feedback presets can play multiple effects.
- Camera shake works.
- Screen flash works.
- Screen fade works.
- Hit stop works.
- Audio cue works.
- VFX spawn works.
- Scale punch works.
- Position shake works.
- Color flash works.
- Floating text works.
- Rumble safely does nothing if no gamepad exists.
- UI button feedback works.
- Polish settings affect intense effects.
- Debug panel works in editor/development builds.
- Polish checklist opens from Tools menu.
- Demo scene proves the framework.
- README explains usage clearly.

---

# 42. Non-Goals

Do not implement in V1:

- Full visual node graph editor
- Complex timeline integration
- Full DOTween clone
- Mandatory DOTween dependency
- Mandatory Cinemachine dependency
- Mandatory URP post-processing dependency
- Full animation state machine
- Full accessibility suite
- Dialogue system
- Inventory system
- Quest system
- Networking
- Save slots
- Online analytics
- Online leaderboard
- Addressables by default
- ECS/DOTS support
- Complex editor tooling beyond checklist

---

# 43. Codex Prompt

Use this prompt when asking Codex to implement:

```text
Implement PolishForge, a Unity 6 game polishing and game-feel framework, using the spec.md file.

Important constraints:
- Use namespace PolishForge.
- Keep the core independent from JamForge.
- JamForge integration must be optional.
- Use ScriptableObject-based feedback presets.
- Use coroutine-based feedback effects.
- Use inspector-friendly MonoBehaviour components.
- Do not require DOTween, Cinemachine, Addressables, or paid assets.
- Use Unity Input System only for optional rumble support.
- Use UGUI/TextMeshPro for UI components.
- All intense feedback must respect PolishSettings.
- Include a demo scene proving every V1 effect.
- Include README.md.
- Keep the architecture simple and game-jam friendly.
```

---

# 44. Recommended V1 Scope

If implementation becomes too large, implement only this first:

```text
FeedbackPreset
FeedbackEffect
FeedbackContext
FeedbackPlayer
FeedbackTrigger
PolishSettings
CameraShakeEffect
ScreenFlashEffect
HitStopEffect
AudioCueEffect
VfxSpawnEffect
ScalePunchEffect
ColorFlashEffect
FloatingTextEffect
JuicyButton
PolishChecklistWindow
README
Demo scene
```

Everything else can be V1.1.

---

# 45. Final Workflow

Recommended usage flow:

```text
1. Build playable prototype using JamForge or normal Unity project.
2. Add PolishCore prefab to scene.
3. Create feedback presets.
4. Attach FeedbackTrigger or call FeedbackPlayer from gameplay events.
5. Add UI juice components to buttons and HUD.
6. Add camera/screen/audio/VFX effects.
7. Tune PolishSettings intensity.
8. Use Polish Checklist before submitting jam build.
```

PolishForge should make a rough prototype feel alive without burying the project under architecture fog.
