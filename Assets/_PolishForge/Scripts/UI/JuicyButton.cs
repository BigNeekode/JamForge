using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PolishForge
{
    [RequireComponent(typeof(Button))]
    public class JuicyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float pressScale = 0.95f;
        [SerializeField] private float animationSpeed = 16f;
        [SerializeField] private PolishAudioCue clickSound;
        [SerializeField] private FeedbackPreset clickFeedback;

        private Vector3 originalScale;
        private Coroutine scaleRoutine;

        private void Awake()
        {
            originalScale = transform.localScale;
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void OnPointerEnter(PointerEventData eventData) => AnimateTo(originalScale * hoverScale);
        public void OnPointerExit(PointerEventData eventData) => AnimateTo(originalScale);
        public void OnPointerDown(PointerEventData eventData) => AnimateTo(originalScale * pressScale);
        public void OnPointerUp(PointerEventData eventData) => AnimateTo(originalScale * hoverScale);

        private void OnClick()
        {
            if (clickSound != null)
                PolishAudioPlayer.Instance?.Play(clickSound, transform.position);

            if (clickFeedback != null)
                FeedbackPlayer.PlayGlobal(clickFeedback, FeedbackContext.From(gameObject));
        }

        private void AnimateTo(Vector3 targetScale)
        {
            if (scaleRoutine != null)
                StopCoroutine(scaleRoutine);

            scaleRoutine = StartCoroutine(ScaleRoutine(targetScale));
        }

        private IEnumerator ScaleRoutine(Vector3 targetScale)
        {
            while (Vector3.SqrMagnitude(transform.localScale - targetScale) > 0.0001f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
                yield return null;
            }

            transform.localScale = targetScale;
        }
    }
}
