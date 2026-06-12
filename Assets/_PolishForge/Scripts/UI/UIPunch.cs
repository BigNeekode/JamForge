using UnityEngine;

namespace PolishForge
{
    public class UIPunch : MonoBehaviour
    {
        [SerializeField] private Vector3 punchScale = new(1.12f, 1.12f, 1.12f);
        [SerializeField] private float duration = 0.15f;

        public void Play()
        {
            ScalePunch punch = GetComponent<ScalePunch>() ?? gameObject.AddComponent<ScalePunch>();
            punch.Punch(punchScale, duration, null, true);
        }
    }
}
