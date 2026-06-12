using UnityEngine;

namespace PolishForge
{
    public class PolishSettings : PolishService<PolishSettings>
    {
        private const string Prefix = "PolishForge.";

        public float MasterPolishIntensity { get; set; } = 1f;
        public float CameraShakeIntensity { get; set; } = 0.75f;
        public float ScreenFlashIntensity { get; set; } = 0.75f;
        public float RumbleIntensity { get; set; } = 0.75f;

        public bool EnableScreenFlash { get; set; } = true;
        public bool EnableCameraShake { get; set; } = true;
        public bool EnableRumble { get; set; } = true;
        public bool EnableHitStop { get; set; } = true;
        public bool EnableSlowMotion { get; set; } = true;
        public bool EnableFloatingText { get; set; } = true;

        protected override void Awake()
        {
            base.Awake();
            if (Instance == this)
                Load();
        }

        public void Load()
        {
            MasterPolishIntensity = PlayerPrefs.GetFloat(Prefix + nameof(MasterPolishIntensity), 1f);
            CameraShakeIntensity = PlayerPrefs.GetFloat(Prefix + nameof(CameraShakeIntensity), 0.75f);
            ScreenFlashIntensity = PlayerPrefs.GetFloat(Prefix + nameof(ScreenFlashIntensity), 0.75f);
            RumbleIntensity = PlayerPrefs.GetFloat(Prefix + nameof(RumbleIntensity), 0.75f);
            EnableScreenFlash = PlayerPrefs.GetInt(Prefix + nameof(EnableScreenFlash), 1) == 1;
            EnableCameraShake = PlayerPrefs.GetInt(Prefix + nameof(EnableCameraShake), 1) == 1;
            EnableRumble = PlayerPrefs.GetInt(Prefix + nameof(EnableRumble), 1) == 1;
            EnableHitStop = PlayerPrefs.GetInt(Prefix + nameof(EnableHitStop), 1) == 1;
            EnableSlowMotion = PlayerPrefs.GetInt(Prefix + nameof(EnableSlowMotion), 1) == 1;
            EnableFloatingText = PlayerPrefs.GetInt(Prefix + nameof(EnableFloatingText), 1) == 1;
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(Prefix + nameof(MasterPolishIntensity), Mathf.Clamp01(MasterPolishIntensity));
            PlayerPrefs.SetFloat(Prefix + nameof(CameraShakeIntensity), Mathf.Clamp01(CameraShakeIntensity));
            PlayerPrefs.SetFloat(Prefix + nameof(ScreenFlashIntensity), Mathf.Clamp01(ScreenFlashIntensity));
            PlayerPrefs.SetFloat(Prefix + nameof(RumbleIntensity), Mathf.Clamp01(RumbleIntensity));
            PlayerPrefs.SetInt(Prefix + nameof(EnableScreenFlash), EnableScreenFlash ? 1 : 0);
            PlayerPrefs.SetInt(Prefix + nameof(EnableCameraShake), EnableCameraShake ? 1 : 0);
            PlayerPrefs.SetInt(Prefix + nameof(EnableRumble), EnableRumble ? 1 : 0);
            PlayerPrefs.SetInt(Prefix + nameof(EnableHitStop), EnableHitStop ? 1 : 0);
            PlayerPrefs.SetInt(Prefix + nameof(EnableSlowMotion), EnableSlowMotion ? 1 : 0);
            PlayerPrefs.SetInt(Prefix + nameof(EnableFloatingText), EnableFloatingText ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void ResetToDefaults()
        {
            MasterPolishIntensity = 1f;
            CameraShakeIntensity = 0.75f;
            ScreenFlashIntensity = 0.75f;
            RumbleIntensity = 0.75f;
            EnableScreenFlash = true;
            EnableCameraShake = true;
            EnableRumble = true;
            EnableHitStop = true;
            EnableSlowMotion = true;
            EnableFloatingText = true;
            Save();
        }

        public static float Master => Instance != null ? Instance.MasterPolishIntensity : 1f;
    }
}
