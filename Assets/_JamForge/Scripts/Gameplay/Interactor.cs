using System.Collections.Generic;
using UnityEngine;

namespace JamForge
{
    public sealed class Interactor : MonoBehaviour
    {
        [SerializeField] private InteractablePrompt prompt;

        private readonly List<IInteractable> nearby = new();

        private void OnEnable()
        {
            if (InputReader.Instance != null)
                InputReader.Instance.OnInteract += Interact;
        }

        private void OnDisable()
        {
            if (InputReader.Instance != null)
                InputReader.Instance.OnInteract -= Interact;
        }

        private void Update()
        {
            IInteractable target = GetClosest();
            if (prompt != null)
                prompt.SetTarget(target);
        }

        private void OnTriggerEnter(Collider other) => AddInteractable(other.gameObject);
        private void OnTriggerExit(Collider other) => RemoveInteractable(other.gameObject);
        private void OnTriggerEnter2D(Collider2D other) => AddInteractable(other.gameObject);
        private void OnTriggerExit2D(Collider2D other) => RemoveInteractable(other.gameObject);

        private void Interact()
        {
            GetClosest()?.Interact(gameObject);
        }

        private void AddInteractable(GameObject target)
        {
            foreach (IInteractable interactable in target.GetComponents<IInteractable>())
            {
                if (!nearby.Contains(interactable))
                    nearby.Add(interactable);
            }
        }

        private void RemoveInteractable(GameObject target)
        {
            foreach (IInteractable interactable in target.GetComponents<IInteractable>())
                nearby.Remove(interactable);
        }

        private IInteractable GetClosest()
        {
            nearby.RemoveAll(item => item == null);

            IInteractable best = null;
            float bestDistance = float.MaxValue;
            Vector3 position = transform.position;

            foreach (IInteractable interactable in nearby)
            {
                if (interactable is not Component component)
                    continue;

                float distance = Vector3.SqrMagnitude(component.transform.position - position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = interactable;
                }
            }

            return best;
        }
    }
}
