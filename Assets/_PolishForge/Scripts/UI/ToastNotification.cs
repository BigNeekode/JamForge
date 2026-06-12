using TMPro;
using UnityEngine;

namespace PolishForge
{
    public class ToastNotification : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;

        private void Awake()
        {
            if (label == null)
                label = GetComponentInChildren<TMP_Text>();
        }

        public void SetMessage(string message)
        {
            if (label != null)
                label.text = message;
        }
    }
}
