using System;
using UnityEngine;

namespace JamForge
{
    [Serializable]
    public sealed class AudioEntry
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        public bool loop;
    }
}
