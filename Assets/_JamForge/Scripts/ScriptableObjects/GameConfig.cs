using UnityEngine;

namespace JamForge
{
    [CreateAssetMenu(menuName = "JamForge/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        public int startingLives = 3;
        public int startingScore;
        public float gameDuration = 120f;
        public string defaultMusicId = "main_theme";
    }
}
