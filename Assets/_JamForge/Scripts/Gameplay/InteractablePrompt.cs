using TMPro;
using UnityEngine;

namespace JamForge
{
    public sealed class InteractablePrompt : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private string format = "{0}";

        private void Awake()
        {
            if (label == null)
                label = GetComponentInChildren<TMP_Text>();
        }

        public void SetTarget(IInteractable interactable)
        {
            bool hasTarget = interactable != null;
            gameObject.SetActive(hasTarget);

            if (hasTarget && label != null)
                label.text = string.Format(format, interactable.GetInteractionText());
        }
    }
}
