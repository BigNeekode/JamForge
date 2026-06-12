using System.Collections.Generic;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Audio Cue")]
    public class PolishAudioCue : ScriptableObject
    {
        public List<AudioClip> Clips = new();
        public Vector2 VolumeRange = new(0.9f, 1f);
        public Vector2 PitchRange = new(0.95f, 1.05f);
        public float Cooldown;
        public bool Spatial;
        public float SpatialBlend = 1f;

        private float lastPlayTime = -999f;

        public bool CanPlay => Time.unscaledTime >= lastPlayTime + Cooldown;
        public void MarkPlayed() => lastPlayTime = Time.unscaledTime;
    }
}
