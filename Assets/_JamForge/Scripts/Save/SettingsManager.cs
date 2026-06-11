using UnityEngine;

namespace JamForge
{
    public sealed class SettingsManager : PersistentSingleton<SettingsManager>
    {
        private const string MasterVolumeKey = "JamForge.MasterVolume";
        private const string MusicVolumeKey = "JamForge.MusicVolume";
        private const string SfxVolumeKey = "JamForge.SfxVolume";
        private const string FullscreenKey = "JamForge.Fullscreen";
        private const string ResolutionIndexKey = "JamForge.ResolutionIndex";
        private const string QualityIndexKey = "JamForge.QualityIndex";
        private const string VSyncKey = "JamForge.VSync";

        public float MasterVolume { get; set; } = 1f;
        public float MusicVolume { get; set; } = 1f;
        public float SfxVolume { get; set; } = 1f;
        public bool Fullscreen { get; set; } = true;
        public int ResolutionIndex { get; set; }
        public int QualityIndex { get; set; }
        public bool VSync { get; set; } = true;

        public void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            Fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
            ResolutionIndex = PlayerPrefs.GetInt(ResolutionIndexKey, 0);
            QualityIndex = PlayerPrefs.GetInt(QualityIndexKey, QualitySettings.GetQualityLevel());
            VSync = PlayerPrefs.GetInt(VSyncKey, 1) == 1;
            Apply();
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
            PlayerPrefs.SetInt(FullscreenKey, Fullscreen ? 1 : 0);
            PlayerPrefs.SetInt(ResolutionIndexKey, ResolutionIndex);
            PlayerPrefs.SetInt(QualityIndexKey, QualityIndex);
            PlayerPrefs.SetInt(VSyncKey, VSync ? 1 : 0);
            PlayerPrefs.Save();
            Apply();
        }

        public void ResetToDefaults()
        {
            MasterVolume = 1f;
            MusicVolume = 1f;
            SfxVolume = 1f;
            Fullscreen = true;
            ResolutionIndex = 0;
            QualityIndex = QualitySettings.GetQualityLevel();
            VSync = true;
            Save();
        }

        private void Apply()
        {
            Screen.fullScreen = Fullscreen;
            QualitySettings.SetQualityLevel(Mathf.Clamp(QualityIndex, 0, QualitySettings.names.Length - 1));
            QualitySettings.vSyncCount = VSync ? 1 : 0;
        }
    }
}
