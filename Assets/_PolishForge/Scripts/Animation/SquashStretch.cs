using UnityEngine;

namespace PolishForge
{
    public class SquashStretch : MonoBehaviour
    {
        [SerializeField] private Vector3 squashScale = new(1.2f, 0.8f, 1.2f);
        [SerializeField] private float duration = 0.18f;

        public void Play()
        {
            ScalePunch punch = GetComponent<ScalePunch>() ?? gameObject.AddComponent<ScalePunch>();
            punch.Punch(squashScale, duration, null, false);
        }
    }
}
