using System.Collections.Generic;
using UnityEngine;

namespace JamForge
{
    public sealed class AudioManager : PersistentSingleton<AudioManager>
    {
        [SerializeField] private AudioLibrary library;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        private readonly Dictionary<string, AudioEntry> music = new();
        private readonly Dictionary<string, AudioEntry> sfx = new();

        protected override void OnAwakeSingleton()
        {
            EnsureSources();
            RebuildLibrary();
            ApplyVolumes();
        }

        public void PlayMusic(string id, bool fade = true)
        {
            if (!music.TryGetValue(id, out AudioEntry entry) || entry.clip == null)
            {
                Debug.LogWarning($"Music id not found: {id}", this);
                return;
            }

            musicSource.clip = entry.clip;
            musicSource.volume = entry.volume * GetSettingsMusicVolume();
            musicSource.pitch = entry.pitch;
            musicSource.loop = entry.loop;
            musicSource.Play();
        }

        public void StopMusic(bool fade = true)
        {
            musicSource.Stop();
        }

        public void PlaySfx(string id)
        {
            if (!sfx.TryGetValue(id, out AudioEntry entry) || entry.clip == null)
            {
                Debug.LogWarning($"SFX id not found: {id}", this);
                return;
            }

            sfxSource.pitch = entry.pitch;
            sfxSource.PlayOneShot(entry.clip, entry.volume * GetSettingsSfxVolume());
        }

        public void PlaySfxAtPosition(string id, Vector3 position)
        {
            if (!sfx.TryGetValue(id, out AudioEntry entry) || entry.clip == null)
            {
                Debug.LogWarning($"SFX id not found: {id}", this);
                return;
            }

            AudioSource.PlayClipAtPoint(entry.clip, position, entry.volume * GetSettingsSfxVolume());
        }

        public void SetMasterVolume(float value)
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.MasterVolume = Mathf.Clamp01(value);

            ApplyVolumes();
        }

        public void SetMusicVolume(float value)
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.MusicVolume = Mathf.Clamp01(value);

            ApplyVolumes();
        }

        public void SetSfxVolume(float value)
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.SfxVolume = Mathf.Clamp01(value);

            ApplyVolumes();
        }

        private void EnsureSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.playOnAwake = false;
                musicSource.loop = true;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }
        }

        private void RebuildLibrary()
        {
            music.Clear();
            sfx.Clear();

            if (library == null)
                return;

            foreach (AudioEntry entry in library.music)
            {
                if (!string.IsNullOrWhiteSpace(entry.id))
                    music[entry.id] = entry;
            }

            foreach (AudioEntry entry in library.sfx)
            {
                if (!string.IsNullOrWhiteSpace(entry.id))
                    sfx[entry.id] = entry;
            }
        }

        private void ApplyVolumes()
        {
            AudioListener.volume = GetSettingsMasterVolume();
            if (musicSource != null)
                musicSource.volume = GetSettingsMusicVolume();
        }

        private static float GetSettingsMasterVolume() => SettingsManager.Instance != null ? SettingsManager.Instance.MasterVolume : 1f;
        private static float GetSettingsMusicVolume() => GetSettingsMasterVolume() * (SettingsManager.Instance != null ? SettingsManager.Instance.MusicVolume : 1f);
        private static float GetSettingsSfxVolume() => GetSettingsMasterVolume() * (SettingsManager.Instance != null ? SettingsManager.Instance.SfxVolume : 1f);
    }
}
