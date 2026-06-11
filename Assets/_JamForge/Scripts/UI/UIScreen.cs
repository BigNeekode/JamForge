using UnityEngine;

namespace JamForge
{
    public abstract class UIScreen : MonoBehaviour
    {
        public bool IsVisible => gameObject.activeSelf;

        protected virtual void Awake()
        {
            UIManager.Instance?.Register(this);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
