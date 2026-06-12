using UnityEngine;

namespace PolishForge
{
    public class UIShake : MonoBehaviour
    {
        [SerializeField] private float duration = 0.15f;
        [SerializeField] private float strength = 8f;

        public void Play()
        {
            PositionShake shake = GetComponent<PositionShake>() ?? gameObject.AddComponent<PositionShake>();
            shake.Shake(duration, strength, 25f, true);
        }
    }
}
