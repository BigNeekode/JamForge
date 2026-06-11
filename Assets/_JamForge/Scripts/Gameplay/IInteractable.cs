using UnityEngine;

namespace JamForge
{
    public interface IInteractable
    {
        string GetInteractionText();
        void Interact(GameObject interactor);
    }
}
