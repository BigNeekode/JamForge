using System.Collections.Generic;
using UnityEngine;

namespace JamForge
{
    [CreateAssetMenu(menuName = "JamForge/Audio Library")]
    public sealed class AudioLibrary : ScriptableObject
    {
        public List<AudioEntry> music = new();
        public List<AudioEntry> sfx = new();
    }
}
