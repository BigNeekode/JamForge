using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PolishForge
{
    public class ToastManager : PolishService<ToastManager>
    {
        [SerializeField] private ToastNotification toastPrefab;
        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private float holdDuration = 1.5f;

        private readonly Queue<string> messages = new();
        private bool showing;

        public void ShowToast(string message)
        {
            messages.Enqueue(message);
            if (!showing)
                StartCoroutine(ProcessQueue());
        }

        private IEnumerator ProcessQueue()
        {
            showing = true;
            while (messages.Count > 0)
            {
                ToastNotification toast = CreateToast();
                CanvasGroup group = toast.GetComponent<CanvasGroup>() ?? toast.gameObject.AddComponent<CanvasGroup>();
                toast.SetMessage(messages.Dequeue());
                yield return Fade(group, 0f, 1f);
                yield return new WaitForSecondsRealtime(holdDuration);
                yield return Fade(group, 1f, 0f);
                Destroy(toast.gameObject);
            }

            showing = false;
        }

        private ToastNotification CreateToast()
        {
            if (toastPrefab != null)
                return Instantiate(toastPrefab, transform);

            GameObject root = new("Toast");
            root.transform.SetParent(transform, false);
            ToastNotification toast = root.AddComponent<ToastNotification>();
            GameObject labelObject = new("Label");
            labelObject.transform.SetParent(root.transform, false);
            labelObject.AddComponent<TextMeshProUGUI>();
            return toast;
        }

        private IEnumerator Fade(CanvasGroup group, float from, float to)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / fadeDuration));
                yield return null;
            }
        }
    }
}
