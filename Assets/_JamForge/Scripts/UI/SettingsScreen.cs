namespace JamForge
{
    public sealed class SettingsScreen : UIScreen
    {
        public void SetMasterVolume(float value)
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.MasterVolume = value;
            AudioManager.Instance?.SetMasterVolume(value);
            SettingsManager.Instance?.Save();
        }

        public void SetMusicVolume(float value)
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.MusicVolume = value;
            AudioManager.Instance?.SetMusicVolume(value);
            SettingsManager.Instance?.Save();
        }

        public void SetSfxVolume(float value)
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.SfxVolume = value;
            AudioManager.Instance?.SetSfxVolume(value);
            SettingsManager.Instance?.Save();
        }

        public void OnResetClicked()
        {
            SettingsManager.Instance?.ResetToDefaults();
        }

        public void OnCloseClicked() => Hide();
    }
}
