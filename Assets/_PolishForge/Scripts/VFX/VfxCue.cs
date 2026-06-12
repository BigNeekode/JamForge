using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/VFX Cue")]
    public class VfxCue : ScriptableObject
    {
        public GameObject Prefab;
        public float Lifetime = 2f;
        public bool UsePooling = true;
        public bool AttachToAnchor;
        public Vector3 Offset;
    }
}
