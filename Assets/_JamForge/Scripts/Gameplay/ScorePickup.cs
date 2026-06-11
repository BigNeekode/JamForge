using UnityEngine;

namespace JamForge
{
    public sealed class ScorePickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private int scoreAmount = 1;
        [SerializeField] private string interactionText = "Pick up";

        public string GetInteractionText() => interactionText;

        public void Interact(GameObject interactor)
        {
            ScoreManager.Instance?.Add(scoreAmount);
            Destroy(gameObject);
        }
    }
}
