using UnityEngine;

namespace JamForge
{
    public sealed class DestroyAfterSeconds : MonoBehaviour
    {
        [SerializeField] private float seconds = 2f;

        private void OnEnable()
        {
            Destroy(gameObject, seconds);
        }
    }
}
