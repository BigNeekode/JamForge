using System.Collections.Generic;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Feedback Preset")]
    public class FeedbackPreset : ScriptableObject
    {
        public List<FeedbackEffect> Effects = new();
        public bool PlayInParallel = true;
    }
}
