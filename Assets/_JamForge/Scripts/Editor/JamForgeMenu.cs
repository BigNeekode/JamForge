using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JamForge.Editor
{
    public static class JamForgeMenu
    {
        private const string SceneFolder = "Assets/_JamForge/Scenes";
        private const string CorePrefabPath = "Assets/_JamForge/Prefabs/Core/JamCore.prefab";
        private const string BootstrapperPrefabPath = "Assets/_JamForge/Prefabs/Core/JamBootstrapper.prefab";
        private const string UIRootPrefabPath = "Assets/_JamForge/Prefabs/UI/UIRoot.prefab";
        private const string SampleProjectilePath = "Assets/_JamForge/Prefabs/Gameplay/SampleProjectile.prefab";
        private const string InputActionsPath = "Assets/_JamForge/Settings/JamInputActions.inputactions";

        public static void FullSetup()
        {
            CreateCorePrefabs();
            CreateJamScenes();
            ConfigureBuildSettings();
        }

        [MenuItem("Tools/JamForge/Create Jam Scenes")]
        public static void CreateJamScenes()
        {
            Directory.CreateDirectory(SceneFolder);
            if (!File.Exists(SampleProjectilePath))
                CreateCorePrefabs();

            CreateScene("00_Boot", CreateBootScene);
            CreateScene("01_MainMenu", CreateMainMenuScene);
            CreateScene("02_Game", CreateGameScene);
            CreateScene("03_GameOver", CreateGameOverScene);
            ConfigureBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/JamForge/Create Core Prefabs")]
        public static void CreateCorePrefabs()
        {
            Directory.CreateDirectory("Assets/_JamForge/Prefabs/Core");
            Directory.CreateDirectory("Assets/_JamForge/Prefabs/UI");

            GameObject core = new("JamCore");
            core.AddComponent<GameStateManager>();
            core.AddComponent<SettingsManager>();
            core.AddComponent<AudioManager>();
            core.AddComponent<UIManager>();
            core.AddComponent<ScoreManager>();
            InputReader inputReader = core.AddComponent<InputReader>();
            AssignInputAsset(inputReader);
            PrefabUtility.SaveAsPrefabAsset(core, CorePrefabPath);
            Object.DestroyImmediate(core);

            GameObject bootstrapper = new("JamBootstrapper");
            JamBootstrapper bootstrapperComponent = bootstrapper.AddComponent<JamBootstrapper>();
            Object corePrefab = AssetDatabase.LoadAssetAtPath<Object>(CorePrefabPath);
            SetObject(bootstrapperComponent, "jamCorePrefab", corePrefab);
            PrefabUtility.SaveAsPrefabAsset(bootstrapper, BootstrapperPrefabPath);
            Object.DestroyImmediate(bootstrapper);

            GameObject uiRoot = new("UIRoot");
            uiRoot.AddComponent<Canvas>();
            uiRoot.AddComponent<CanvasScaler>();
            uiRoot.AddComponent<GraphicRaycaster>();
            PrefabUtility.SaveAsPrefabAsset(uiRoot, UIRootPrefabPath);
            Object.DestroyImmediate(uiRoot);

            SaveScreenPrefab<MainMenuScreen>("MainMenuScreen");
            SaveScreenPrefab<PauseScreen>("PauseScreen");
            SaveScreenPrefab<GameOverScreen>("GameOverScreen");
            SaveScreenPrefab<VictoryScreen>("VictoryScreen");
            SaveScreenPrefab<HUDScreen>("HUDScreen");
            SaveScreenPrefab<SettingsScreen>("SettingsScreen");
            SaveScreenPrefab<FadeScreen>("FadeScreen");
            SaveScreenPrefab<ConfirmDialog>("ConfirmDialog");
            SaveSampleGameplayPrefabs();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/JamForge/Open Framework Readme")]
        public static void OpenFrameworkReadme()
        {
            Object readme = AssetDatabase.LoadAssetAtPath<Object>("Assets/_JamForge/README.md");
            if (readme != null)
                AssetDatabase.OpenAsset(readme);
        }

        [MenuItem("Tools/JamForge/Reset PlayerPrefs")]
        public static void ResetPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("JamForge PlayerPrefs reset.");
        }

        [MenuItem("Tools/JamForge/Configure Build Settings")]
        public static void ConfigureBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene($"{SceneFolder}/00_Boot.unity", true),
                new EditorBuildSettingsScene($"{SceneFolder}/01_MainMenu.unity", true),
                new EditorBuildSettingsScene($"{SceneFolder}/02_Game.unity", true),
                new EditorBuildSettingsScene($"{SceneFolder}/03_GameOver.unity", true)
            };
        }

        private static void CreateScene(string sceneName, System.Action setup)
        {
            string path = $"{SceneFolder}/{sceneName}.unity";
            if (File.Exists(path))
                return;

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            setup?.Invoke();
            EditorSceneManager.SaveScene(scene, path);
        }

        private static void CreateBootScene()
        {
            GameObject bootstrapper = new("JamBootstrapper");
            JamBootstrapper bootstrapperComponent = bootstrapper.AddComponent<JamBootstrapper>();
            Object corePrefab = AssetDatabase.LoadAssetAtPath<Object>(CorePrefabPath);
            SetObject(bootstrapperComponent, "jamCorePrefab", corePrefab);
            AddCameraAndLight();
        }

        private static void CreateMainMenuScene()
        {
            AddCameraAndLight();
            AddEventSystem();
            MainMenuScreen screen = CreateScreen<MainMenuScreen>("MainMenuScreen");
            SettingsScreen settings = CreateScreen<SettingsScreen>("SettingsScreen");
            Button play = CreateButton(screen.transform, "PlayButton", "Play", new Vector2(0f, 80f));
            Button settingsButton = CreateButton(screen.transform, "SettingsButton", "Settings", new Vector2(0f, 0f));
            Button quit = CreateButton(screen.transform, "QuitButton", "Quit", new Vector2(0f, -80f));
            UnityEventTools.AddPersistentListener(play.onClick, screen.OnPlayClicked);
            UnityEventTools.AddPersistentListener(settingsButton.onClick, settings.Show);
            UnityEventTools.AddPersistentListener(quit.onClick, screen.OnQuitClicked);

            CreateSlider(settings.transform, "MasterVolume", "Master", new Vector2(0f, 80f), settings.SetMasterVolume);
            CreateSlider(settings.transform, "MusicVolume", "Music", new Vector2(0f, 10f), settings.SetMusicVolume);
            CreateSlider(settings.transform, "SfxVolume", "SFX", new Vector2(0f, -60f), settings.SetSfxVolume);
            Button close = CreateButton(settings.transform, "CloseButton", "Close", new Vector2(0f, -140f));
            UnityEventTools.AddPersistentListener(close.onClick, settings.OnCloseClicked);
            settings.Hide();
        }

        private static void CreateGameScene()
        {
            AddCameraAndLight();
            AddEventSystem();
            GameObject debug = new("JamDebug");
            debug.AddComponent<JamDebug>();

            PauseScreen pause = CreateScreen<PauseScreen>("PauseScreen");
            Button resume = CreateButton(pause.transform, "ResumeButton", "Resume", new Vector2(0f, 40f));
            Button menu = CreateButton(pause.transform, "MainMenuButton", "Main Menu", new Vector2(0f, -40f));
            UnityEventTools.AddPersistentListener(resume.onClick, pause.OnResumeClicked);
            UnityEventTools.AddPersistentListener(menu.onClick, pause.OnMainMenuClicked);

            HUDScreen hud = CreateScreen<HUDScreen>("HUDScreen");
            hud.Show();
            TMP_Text scoreLabel = CreateHudLabel(hud.transform, "ScoreText", "Score: 0", new Vector2(20f, -20f), TextAlignmentOptions.TopLeft);
            scoreLabel.gameObject.AddComponent<ScoreText>();
            TMP_Text timerLabel = CreateHudLabel(hud.transform, "TimerText", "Time: 60", new Vector2(-20f, -20f), TextAlignmentOptions.TopRight);
            timerLabel.gameObject.AddComponent<TimerText>();
            TMP_Text debugLabel = CreateHudLabel(hud.transform, "DebugOverlay", string.Empty, new Vector2(20f, -90f), TextAlignmentOptions.TopLeft);
            debugLabel.fontSize = 18f;
            debugLabel.gameObject.AddComponent<DebugOverlay>();

            GameOverScreen gameOver = CreateScreen<GameOverScreen>("GameOverScreen");
            Button retry = CreateButton(gameOver.transform, "RetryButton", "Retry", new Vector2(0f, 40f));
            Button gameOverMenu = CreateButton(gameOver.transform, "MainMenuButton", "Main Menu", new Vector2(0f, -40f));
            UnityEventTools.AddPersistentListener(retry.onClick, gameOver.OnRetryClicked);
            UnityEventTools.AddPersistentListener(gameOverMenu.onClick, gameOver.OnMainMenuClicked);

            VictoryScreen victory = CreateScreen<VictoryScreen>("VictoryScreen");
            Button victoryRetry = CreateButton(victory.transform, "RetryButton", "Retry", new Vector2(0f, 40f));
            Button victoryMenu = CreateButton(victory.transform, "MainMenuButton", "Main Menu", new Vector2(0f, -40f));
            UnityEventTools.AddPersistentListener(victoryRetry.onClick, victory.OnRetryClicked);
            UnityEventTools.AddPersistentListener(victoryMenu.onClick, victory.OnMainMenuClicked);

            GameObject timerObject = new("GameTimer");
            JamTimer timer = timerObject.AddComponent<JamTimer>();
            SetBool(timer, "gameOverWhenFinished", true);

            GameObject controller = new("SampleGameController");
            SampleGameController sampleGameController = controller.AddComponent<SampleGameController>();
            SetObject(sampleGameController, "timer", timer);
            SetFloat(sampleGameController, "gameDuration", 60f);

            GameObject poolObject = new("ProjectilePool");
            ObjectPool pool = poolObject.AddComponent<ObjectPool>();
            Poolable projectilePrefab = AssetDatabase.LoadAssetAtPath<Poolable>(SampleProjectilePath);
            SetObject(pool, "prefab", projectilePrefab);

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "SamplePlayer3D";
            player.transform.position = Vector3.up;
            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            player.AddComponent<CharacterController>();
            player.AddComponent<TopDown3DController>();
            SphereCollider interactionRange = player.AddComponent<SphereCollider>();
            interactionRange.isTrigger = true;
            interactionRange.radius = 1.5f;
            player.AddComponent<Interactor>();
            player.AddComponent<Health>();
            Shooter shooter = player.AddComponent<Shooter>();
            GameObject firePoint = new("FirePoint");
            firePoint.transform.SetParent(player.transform, false);
            firePoint.transform.localPosition = new Vector3(0f, 0.5f, 1f);
            SetObject(shooter, "projectilePool", pool);
            SetObject(shooter, "firePoint", firePoint.transform);

            GameObject pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickup.name = "ScorePickup";
            pickup.transform.position = new Vector3(2f, 0.5f, 2f);
            pickup.AddComponent<ScorePickup>();
            Collider pickupCollider = pickup.GetComponent<Collider>();
            pickupCollider.isTrigger = true;

            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = "SampleEnemy";
            enemy.transform.position = new Vector3(0f, 1f, 6f);
            enemy.AddComponent<Health>();
            enemy.AddComponent<AwardScoreOnDeath>();

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(2f, 1f, 2f);
        }

        private static void CreateGameOverScene()
        {
            AddCameraAndLight();
            AddEventSystem();
            GameOverScreen screen = CreateScreen<GameOverScreen>("GameOverScreen");
            Button retry = CreateButton(screen.transform, "RetryButton", "Retry", new Vector2(0f, 40f));
            Button menu = CreateButton(screen.transform, "MainMenuButton", "Main Menu", new Vector2(0f, -40f));
            UnityEventTools.AddPersistentListener(retry.onClick, screen.OnRetryClicked);
            UnityEventTools.AddPersistentListener(menu.onClick, screen.OnMainMenuClicked);
        }

        private static void AddCameraAndLight()
        {
            GameObject cameraObject = new("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 8f, -10f);
            cameraObject.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

            GameObject lightObject = new("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void AddEventSystem()
        {
            GameObject eventSystemObject = new("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static void AssignInputAsset(InputReader inputReader)
        {
            Object inputAsset = AssetDatabase.LoadAssetAtPath<Object>(InputActionsPath);
            if (inputAsset == null)
                return;

            SerializedObject serializedObject = new(inputReader);
            serializedObject.FindProperty("inputActions").objectReferenceValue = inputAsset;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static T CreateScreen<T>(string name) where T : UIScreen
        {
            GameObject canvasObject = new($"{name}Canvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject screenObject = new(name);
            screenObject.transform.SetParent(canvasObject.transform, false);
            RectTransform rect = screenObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return screenObject.AddComponent<T>();
        }

        private static Button CreateButton(Transform parent, string name, string text, Vector2 anchoredPosition)
        {
            GameObject buttonObject = new(name);
            buttonObject.transform.SetParent(parent, false);
            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(220f, 48f);
            rect.anchoredPosition = anchoredPosition;
            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.12f, 0.13f, 0.16f, 0.95f);
            Button button = buttonObject.AddComponent<Button>();

            GameObject labelObject = new("Label");
            labelObject.transform.SetParent(buttonObject.transform, false);
            RectTransform labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = 24f;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;

            return button;
        }

        private static Slider CreateSlider(Transform parent, string name, string text, Vector2 anchoredPosition, UnityEngine.Events.UnityAction<float> onValueChanged)
        {
            GameObject root = new(name);
            root.transform.SetParent(parent, false);
            RectTransform rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(360f, 48f);
            rootRect.anchoredPosition = anchoredPosition;

            GameObject labelObject = new("Label");
            labelObject.transform.SetParent(root.transform, false);
            RectTransform labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(0f, 1f);
            labelRect.pivot = new Vector2(0f, 0.5f);
            labelRect.sizeDelta = new Vector2(110f, 48f);
            labelRect.anchoredPosition = Vector2.zero;
            TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = 20f;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.color = Color.white;

            GameObject sliderObject = new("Slider");
            sliderObject.transform.SetParent(root.transform, false);
            RectTransform sliderRect = sliderObject.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0f, 0.5f);
            sliderRect.anchorMax = new Vector2(1f, 0.5f);
            sliderRect.offsetMin = new Vector2(120f, -10f);
            sliderRect.offsetMax = new Vector2(0f, 10f);
            Image background = sliderObject.AddComponent<Image>();
            background.color = new Color(0.18f, 0.18f, 0.2f, 1f);
            Slider slider = sliderObject.AddComponent<Slider>();

            GameObject fillAreaObject = new("Fill Area");
            fillAreaObject.transform.SetParent(sliderObject.transform, false);
            RectTransform fillAreaRect = fillAreaObject.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(4f, 4f);
            fillAreaRect.offsetMax = new Vector2(-4f, -4f);

            GameObject fillObject = new("Fill");
            fillObject.transform.SetParent(fillAreaObject.transform, false);
            RectTransform fillRect = fillObject.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fill = fillObject.AddComponent<Image>();
            fill.color = new Color(0.25f, 0.56f, 0.9f, 1f);

            GameObject handleAreaObject = new("Handle Slide Area");
            handleAreaObject.transform.SetParent(sliderObject.transform, false);
            RectTransform handleAreaRect = handleAreaObject.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(8f, -4f);
            handleAreaRect.offsetMax = new Vector2(-8f, 4f);

            GameObject handleObject = new("Handle");
            handleObject.transform.SetParent(handleAreaObject.transform, false);
            RectTransform handleRect = handleObject.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(18f, 28f);
            Image handle = handleObject.AddComponent<Image>();
            handle.color = Color.white;

            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handle;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            UnityEventTools.AddPersistentListener(slider.onValueChanged, onValueChanged);
            return slider;
        }

        private static TMP_Text CreateHudLabel(Transform parent, string name, string text, Vector2 offset, TextAlignmentOptions alignment)
        {
            GameObject labelObject = new(name);
            labelObject.transform.SetParent(parent, false);
            RectTransform rect = labelObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(offset.x >= 0f ? 0f : 1f, 1f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(offset.x >= 0f ? 0f : 1f, 1f);
            rect.sizeDelta = new Vector2(260f, 80f);
            rect.anchoredPosition = offset;
            TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = 24f;
            label.alignment = alignment;
            label.color = Color.white;
            return label;
        }

        private static void SaveScreenPrefab<T>(string name) where T : UIScreen
        {
            GameObject prefab = new(name);
            prefab.AddComponent<RectTransform>();
            prefab.AddComponent<T>();
            PrefabUtility.SaveAsPrefabAsset(prefab, $"Assets/_JamForge/Prefabs/UI/{name}.prefab");
            Object.DestroyImmediate(prefab);
        }

        private static void SaveSampleGameplayPrefabs()
        {
            GameObject player2D = new("SamplePlayer2D");
            player2D.AddComponent<Rigidbody2D>();
            player2D.AddComponent<CircleCollider2D>();
            player2D.AddComponent<TopDown2DController>();
            player2D.AddComponent<Interactor>();
            PrefabUtility.SaveAsPrefabAsset(player2D, "Assets/_JamForge/Prefabs/Gameplay/SamplePlayer2D.prefab");
            Object.DestroyImmediate(player2D);

            GameObject player3D = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player3D.name = "SamplePlayer3D";
            Object.DestroyImmediate(player3D.GetComponent<CapsuleCollider>());
            player3D.AddComponent<CharacterController>();
            player3D.AddComponent<TopDown3DController>();
            player3D.AddComponent<Interactor>();
            PrefabUtility.SaveAsPrefabAsset(player3D, "Assets/_JamForge/Prefabs/Gameplay/SamplePlayer3D.prefab");
            Object.DestroyImmediate(player3D);

            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "SampleProjectile";
            projectile.transform.localScale = Vector3.one * 0.25f;
            projectile.GetComponent<SphereCollider>().isTrigger = true;
            Rigidbody body = projectile.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            projectile.AddComponent<Projectile>();
            PrefabUtility.SaveAsPrefabAsset(projectile, SampleProjectilePath);
            Object.DestroyImmediate(projectile);
        }

        private static void SetObject(Object target, string propertyName, Object value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetBool(Object target, string propertyName, bool value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).boolValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloat(Object target, string propertyName, float value)
        {
            SerializedObject serializedObject = new(target);
            serializedObject.FindProperty(propertyName).floatValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
