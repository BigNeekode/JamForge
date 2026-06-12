using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PolishForge.Editor
{
    public static class PolishForgeMenu
    {
        private const string Root = "Assets/_PolishForge";
        private const string PresetFolder = Root + "/ScriptableObjects/FeedbackPresets";
        private const string EffectFolder = Root + "/ScriptableObjects/FeedbackPresets/Effects";
        private const string AudioCueFolder = Root + "/ScriptableObjects/AudioCues";
        private const string VfxCueFolder = Root + "/ScriptableObjects/VfxCues";
        private const string CorePrefabPath = Root + "/Prefabs/Core/PolishCore.prefab";
        private const string FloatingTextPrefabPath = Root + "/Prefabs/UI/FloatingText.prefab";
        private const string ToastManagerPrefabPath = Root + "/Prefabs/UI/ToastManager.prefab";
        private const string ScreenFlashPrefabPath = Root + "/Prefabs/ScreenEffects/ScreenFlashOverlay.prefab";
        private const string ScreenFadePrefabPath = Root + "/Prefabs/ScreenEffects/ScreenFadeOverlay.prefab";
        private const string HitSparkPrefabPath = Root + "/Prefabs/VFX/HitSpark.prefab";
        private const string PickupSparklePrefabPath = Root + "/Prefabs/VFX/PickupSparkle.prefab";
        private const string EnemyDeathPoofPrefabPath = Root + "/Prefabs/VFX/EnemyDeathPoof.prefab";
        private const string DemoTargetPrefabPath = Root + "/Prefabs/Samples/PolishDemoTarget.prefab";
        private const string DemoScenePath = Root + "/Scenes/PolishForge_Demo.unity";

        [MenuItem("Tools/PolishForge/Create Default Assets")]
        public static void CreateDefaultAssets()
        {
            EnsureFolders();
            CreatePlaceholderPrefabs();
            CreateCueAssets();
            CreatePresetAssets();
            CreateCorePrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/PolishForge/Create Core Prefab")]
        public static void CreateCorePrefab()
        {
            EnsureFolders();

            GameObject core = new("PolishCore");
            core.AddComponent<FeedbackPlayer>();
            core.AddComponent<PolishSettings>();
            core.AddComponent<PolishCameraShake>();
            core.AddComponent<PolishAudioPlayer>();
            core.AddComponent<HitStopController>();
            core.AddComponent<VfxSpawner>();
            core.AddComponent<FloatingTextSpawner>();
            core.AddComponent<ToastManager>();
            core.AddComponent<RumbleController>();
            core.AddComponent<PolishDebugPanel>();

            GameObject flash = new("ScreenFlashOverlay");
            flash.transform.SetParent(core.transform, false);
            flash.AddComponent<ScreenFlashOverlay>();

            GameObject fade = new("ScreenFadeOverlay");
            fade.transform.SetParent(core.transform, false);
            fade.AddComponent<ScreenFadeOverlay>();

            PrefabUtility.SaveAsPrefabAsset(core, CorePrefabPath);
            Object.DestroyImmediate(core);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Tools/PolishForge/Create Demo Scene")]
        public static void CreateDemoScene()
        {
            CreateDefaultAssets();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            AddCameraAndLight();
            AddEventSystem();

            GameObject corePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CorePrefabPath);
            if (corePrefab != null)
                PrefabUtility.InstantiatePrefab(corePrefab);

            FeedbackPreset enemyHit = Load<FeedbackPreset>(PresetFolder + "/EnemyHitFeedback.asset");
            FeedbackPreset enemyDeath = Load<FeedbackPreset>(PresetFolder + "/EnemyDeathFeedback.asset");
            FeedbackPreset pickup = Load<FeedbackPreset>(PresetFolder + "/PickupFeedback.asset");
            FeedbackPreset button = Load<FeedbackPreset>(PresetFolder + "/ButtonClickFeedback.asset");
            FeedbackPreset victory = Load<FeedbackPreset>(PresetFolder + "/VictoryFeedback.asset");
            FeedbackPreset gameOver = Load<FeedbackPreset>(PresetFolder + "/GameOverFeedback.asset");

            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = "PolishDemoTarget_ClickMe";
            target.transform.position = new Vector3(0f, 1f, 3f);
            target.AddComponent<RotateMotion>();
            PolishDemoTarget demoTarget = target.AddComponent<PolishDemoTarget>();
            SetObject(demoTarget, "hitFeedback", enemyHit);
            SetObject(demoTarget, "deathFeedback", enemyDeath);

            GameObject pickupObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pickupObject.name = "PickupFeedbackDemo";
            pickupObject.transform.position = new Vector3(-2f, 1f, 3f);
            pickupObject.AddComponent<BobMotion>();
            FeedbackTrigger pickupTrigger = pickupObject.AddComponent<FeedbackTrigger>();
            SetObject(pickupTrigger, "preset", pickup);

            CreateDemoCanvas(button, victory, gameOver, pickupTrigger, demoTarget);

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(3f, 1f, 3f);

            Directory.CreateDirectory(Path.GetDirectoryName(DemoScenePath));
            EditorSceneManager.SaveScene(scene, DemoScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/PolishForge/Full Setup")]
        public static void FullSetup()
        {
            CreateDefaultAssets();
            CreateDemoScene();
        }

        private static void EnsureFolders()
        {
            string[] folders =
            {
                PresetFolder, EffectFolder, AudioCueFolder, VfxCueFolder,
                Root + "/Prefabs/Core", Root + "/Prefabs/UI", Root + "/Prefabs/VFX",
                Root + "/Prefabs/ScreenEffects", Root + "/Prefabs/Samples", Root + "/Scenes"
            };

            foreach (string folder in folders)
                Directory.CreateDirectory(folder);
        }

        private static void CreatePlaceholderPrefabs()
        {
            SavePrimitivePrefab(HitSparkPrefabPath, PrimitiveType.Sphere, new Color(1f, 0.35f, 0.1f));
            SavePrimitivePrefab(PickupSparklePrefabPath, PrimitiveType.Sphere, Color.yellow);
            SavePrimitivePrefab(EnemyDeathPoofPrefabPath, PrimitiveType.Capsule, new Color(0.65f, 0.65f, 0.7f));
            CreateFloatingTextPrefab();
            CreateToastManagerPrefab();
            CreateScreenOverlayPrefab(ScreenFlashPrefabPath, "ScreenFlashOverlay", typeof(ScreenFlashOverlay));
            CreateScreenOverlayPrefab(ScreenFadePrefabPath, "ScreenFadeOverlay", typeof(ScreenFadeOverlay));
            CreateDemoTargetPrefab();
        }

        private static void SavePrimitivePrefab(string path, PrimitiveType type, Color color)
        {
            if (File.Exists(path))
                return;

            GameObject prefab = GameObject.CreatePrimitive(type);
            prefab.name = Path.GetFileNameWithoutExtension(path);
            prefab.transform.localScale = Vector3.one * 0.35f;
            Renderer renderer = prefab.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = CreateMaterial(color, prefab.name + "Material");
            prefab.AddComponent<PooledVfx>();
            PrefabUtility.SaveAsPrefabAsset(prefab, path);
            Object.DestroyImmediate(prefab);
        }

        private static void CreateFloatingTextPrefab()
        {
            if (File.Exists(FloatingTextPrefabPath))
                return;

            GameObject root = new("FloatingText");
            FloatingText floatingText = root.AddComponent<FloatingText>();
            GameObject labelObject = new("Label");
            labelObject.transform.SetParent(root.transform, false);
            TextMeshPro label = labelObject.AddComponent<TextMeshPro>();
            label.text = "+1";
            label.fontSize = 4f;
            label.alignment = TextAlignmentOptions.Center;
            SetObject(floatingText, "label", label);
            PrefabUtility.SaveAsPrefabAsset(root, FloatingTextPrefabPath);
            Object.DestroyImmediate(root);
        }

        private static void CreateToastManagerPrefab()
        {
            if (File.Exists(ToastManagerPrefabPath))
                return;

            GameObject root = new("ToastManager");
            root.AddComponent<ToastManager>();
            PrefabUtility.SaveAsPrefabAsset(root, ToastManagerPrefabPath);
            Object.DestroyImmediate(root);
        }

        private static void CreateDemoTargetPrefab()
        {
            if (File.Exists(DemoTargetPrefabPath))
                return;

            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = "PolishDemoTarget";
            target.AddComponent<RotateMotion>();
            target.AddComponent<PolishDemoTarget>();
            PrefabUtility.SaveAsPrefabAsset(target, DemoTargetPrefabPath);
            Object.DestroyImmediate(target);
        }

        private static void CreateScreenOverlayPrefab(string path, string name, System.Type componentType)
        {
            if (File.Exists(path))
                return;

            GameObject root = new(name);
            root.AddComponent(componentType);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void CreateCueAssets()
        {
            CreateAsset<PolishAudioCue>(AudioCueFolder + "/ButtonClickCue.asset");
            CreateAsset<PolishAudioCue>(AudioCueFolder + "/PickupCue.asset");
            CreateAsset<PolishAudioCue>(AudioCueFolder + "/HitCue.asset");
            CreateAsset<PolishAudioCue>(AudioCueFolder + "/ErrorCue.asset");

            VfxCue hit = CreateAsset<VfxCue>(VfxCueFolder + "/HitSparkCue.asset");
            SetObject(hit, "Prefab", Load<GameObject>(HitSparkPrefabPath));
            VfxCue pickup = CreateAsset<VfxCue>(VfxCueFolder + "/PickupSparkleCue.asset");
            SetObject(pickup, "Prefab", Load<GameObject>(PickupSparklePrefabPath));
            VfxCue death = CreateAsset<VfxCue>(VfxCueFolder + "/EnemyDeathPoofCue.asset");
            SetObject(death, "Prefab", Load<GameObject>(EnemyDeathPoofPrefabPath));
        }

        private static void CreatePresetAssets()
        {
            FeedbackEffect cameraShake = CreateAsset<CameraShakeEffect>(EffectFolder + "/CameraShakeEffect.asset");
            FeedbackEffect screenFlash = CreateAsset<ScreenFlashEffect>(EffectFolder + "/ScreenFlashEffect.asset");
            FeedbackEffect hitStop = CreateAsset<HitStopEffect>(EffectFolder + "/HitStopEffect.asset");
            FeedbackEffect scalePunch = CreateAsset<ScalePunchEffect>(EffectFolder + "/ScalePunchEffect.asset");
            FeedbackEffect positionShake = CreateAsset<PositionShakeEffect>(EffectFolder + "/PositionShakeEffect.asset");
            FeedbackEffect colorFlash = CreateAsset<ColorFlashEffect>(EffectFolder + "/ColorFlashEffect.asset");
            FeedbackEffect floatingText = CreateAsset<FloatingTextEffect>(EffectFolder + "/FloatingTextEffect.asset");
            FeedbackEffect vfxHit = CreateAsset<VfxSpawnEffect>(EffectFolder + "/HitSparkEffect.asset");
            SetObject(vfxHit, "cue", Load<VfxCue>(VfxCueFolder + "/HitSparkCue.asset"));
            FeedbackEffect vfxPickup = CreateAsset<VfxSpawnEffect>(EffectFolder + "/PickupSparkleEffect.asset");
            SetObject(vfxPickup, "cue", Load<VfxCue>(VfxCueFolder + "/PickupSparkleCue.asset"));
            FeedbackEffect vfxDeath = CreateAsset<VfxSpawnEffect>(EffectFolder + "/EnemyDeathPoofEffect.asset");
            SetObject(vfxDeath, "cue", Load<VfxCue>(VfxCueFolder + "/EnemyDeathPoofCue.asset"));
            FeedbackEffect fade = CreateAsset<ScreenFadeEffect>(EffectFolder + "/ScreenFadeEffect.asset");

            CreatePreset("PlayerHitFeedback", cameraShake, screenFlash, hitStop, colorFlash, floatingText);
            CreatePreset("EnemyHitFeedback", cameraShake, hitStop, colorFlash, positionShake, vfxHit, floatingText);
            CreatePreset("EnemyDeathFeedback", cameraShake, screenFlash, vfxDeath, floatingText);
            CreatePreset("PickupFeedback", scalePunch, vfxPickup, floatingText);
            CreatePreset("ButtonClickFeedback", scalePunch);
            CreatePreset("VictoryFeedback", screenFlash, fade, floatingText);
            CreatePreset("GameOverFeedback", cameraShake, fade, floatingText);
            CreatePreset("ErrorFeedback", screenFlash, positionShake);
        }

        private static void CreatePreset(string name, params FeedbackEffect[] effects)
        {
            string path = $"{PresetFolder}/{name}.asset";
            FeedbackPreset preset = CreateAsset<FeedbackPreset>(path);
            SerializedObject serializedObject = new(preset);
            SerializedProperty effectsProperty = serializedObject.FindProperty("Effects");
            effectsProperty.ClearArray();
            for (int i = 0; i < effects.Length; i++)
            {
                effectsProperty.InsertArrayElementAtIndex(i);
                effectsProperty.GetArrayElementAtIndex(i).objectReferenceValue = effects[i];
            }
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(preset);
        }

        private static void CreateDemoCanvas(FeedbackPreset buttonPreset, FeedbackPreset victoryPreset, FeedbackPreset gameOverPreset, FeedbackTrigger pickupTrigger, PolishDemoTarget target)
        {
            GameObject canvasObject = new("DemoCanvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            PolishDemoActions actions = canvasObject.AddComponent<PolishDemoActions>();
            SetObject(actions, "target", target);
            SetObject(actions, "pickupTrigger", pickupTrigger);
            SetObject(actions, "victoryFeedback", victoryPreset);
            SetObject(actions, "gameOverFeedback", gameOverPreset);

            TMP_Text title = CreateLabel(canvasObject.transform, "Title", "PolishForge Demo", new Vector2(0f, -40f), 34f);
            title.alignment = TextAlignmentOptions.Center;

            CreateButton(canvasObject.transform, "Hit Target", new Vector2(-240f, -120f), actions.HitTarget, buttonPreset);
            CreateButton(canvasObject.transform, "Pickup", new Vector2(0f, -120f), actions.TriggerPickup, buttonPreset);
            CreateButton(canvasObject.transform, "Victory", new Vector2(240f, -120f), actions.TriggerVictory, buttonPreset);
            CreateButton(canvasObject.transform, "Game Over", new Vector2(0f, -190f), actions.TriggerGameOver, buttonPreset);
            CreateSlider(canvasObject.transform, "Camera Shake", new Vector2(-240f, -280f), actions.SetCameraShakeIntensity);
            CreateSlider(canvasObject.transform, "Screen Flash", new Vector2(0f, -280f), actions.SetScreenFlashIntensity);
            CreateSlider(canvasObject.transform, "Rumble", new Vector2(240f, -280f), actions.SetRumbleIntensity);
        }

        private static TMP_Text CreateLabel(Transform parent, string name, string text, Vector2 anchoredPosition, float fontSize)
        {
            GameObject labelObject = new(name);
            labelObject.transform.SetParent(parent, false);
            RectTransform rect = labelObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(520f, 60f);
            rect.anchoredPosition = anchoredPosition;
            TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = fontSize;
            label.color = Color.white;
            return label;
        }

        private static void CreateButton(Transform parent, string text, Vector2 anchoredPosition, UnityEngine.Events.UnityAction action, FeedbackPreset clickPreset)
        {
            GameObject buttonObject = new(text + "Button");
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(180f, 48f);
            rect.anchoredPosition = anchoredPosition;
            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.12f, 0.14f, 0.18f, 0.95f);
            Button button = buttonObject.AddComponent<Button>();
            UnityEventTools.AddPersistentListener(button.onClick, action);
            JuicyButton juicy = buttonObject.AddComponent<JuicyButton>();
            SetObject(juicy, "clickFeedback", clickPreset);

            GameObject labelObject = new("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            RectTransform labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = 20f;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
        }

        private static void CreateSlider(Transform parent, string text, Vector2 anchoredPosition, UnityEngine.Events.UnityAction<float> action)
        {
            GameObject root = new(text + "SliderRoot");
            root.transform.SetParent(parent, false);
            RectTransform rootRect = root.AddComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 1f);
            rootRect.anchorMax = new Vector2(0.5f, 1f);
            rootRect.sizeDelta = new Vector2(200f, 54f);
            rootRect.anchoredPosition = anchoredPosition;

            TMP_Text label = CreateLabel(root.transform, "Label", text, new Vector2(0f, 0f), 16f);
            RectTransform labelRect = label.transform as RectTransform;
            labelRect.anchorMin = new Vector2(0.5f, 1f);
            labelRect.anchorMax = new Vector2(0.5f, 1f);
            labelRect.sizeDelta = new Vector2(200f, 24f);
            labelRect.anchoredPosition = new Vector2(0f, 0f);

            GameObject sliderObject = new("Slider");
            sliderObject.transform.SetParent(root.transform, false);
            RectTransform sliderRect = sliderObject.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.5f, 1f);
            sliderRect.anchorMax = new Vector2(0.5f, 1f);
            sliderRect.sizeDelta = new Vector2(180f, 18f);
            sliderRect.anchoredPosition = new Vector2(0f, -28f);
            Image background = sliderObject.AddComponent<Image>();
            background.color = new Color(0.18f, 0.18f, 0.2f, 1f);
            Slider slider = sliderObject.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0.75f;

            GameObject fillArea = new("Fill Area");
            fillArea.transform.SetParent(sliderObject.transform, false);
            RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(4f, 4f);
            fillAreaRect.offsetMax = new Vector2(-4f, -4f);

            GameObject fillObject = new("Fill");
            fillObject.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fillObject.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fill = fillObject.AddComponent<Image>();
            fill.color = new Color(0.25f, 0.56f, 0.9f, 1f);

            GameObject handleArea = new("Handle Slide Area");
            handleArea.transform.SetParent(sliderObject.transform, false);
            RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(8f, -4f);
            handleAreaRect.offsetMax = new Vector2(-8f, 4f);

            GameObject handleObject = new("Handle");
            handleObject.transform.SetParent(handleArea.transform, false);
            RectTransform handleRect = handleObject.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(16f, 24f);
            Image handle = handleObject.AddComponent<Image>();
            handle.color = Color.white;

            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handle;
            UnityEventTools.AddPersistentListener(slider.onValueChanged, action);
        }

        private static void AddCameraAndLight()
        {
            GameObject cameraObject = new("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 3f, -6f);
            cameraObject.transform.rotation = Quaternion.Euler(18f, 0f, 0f);

            GameObject lightObject = new("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void AddEventSystem()
        {
            GameObject eventSystem = new("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static Material CreateMaterial(Color color, string name)
        {
            string path = $"{Root}/Art/Materials/{name}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
                return material;

            material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            material.color = color;
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static T Load<T>(string path) where T : Object => AssetDatabase.LoadAssetAtPath<T>(path);

        private static void SetObject(Object target, string propertyName, Object value)
        {
            SerializedObject serializedObject = new(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
                property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }
}
