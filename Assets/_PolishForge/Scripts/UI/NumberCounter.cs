using System.Collections;
using TMPro;
using UnityEngine;

namespace PolishForge
{
    public class NumberCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private string prefix;
        [SerializeField] private string suffix;
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private bool punchOnChange = true;

        private int currentValue;
        private Coroutine activeRoutine;

        private void Awake()
        {
            if (label == null)
                label = GetComponent<TMP_Text>();
        }

        public void SetValue(int value)
        {
            if (activeRoutine != null)
                StopCoroutine(activeRoutine);

            activeRoutine = StartCoroutine(CountRoutine(currentValue, value));
            if (punchOnChange)
            {
                ScalePunch punch = GetComponent<ScalePunch>() ?? gameObject.AddComponent<ScalePunch>();
                punch.Punch(Vector3.one * 1.12f, 0.15f, null, true);
            }
        }

        private IEnumerator CountRoutine(int from, int to)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                int shown = Mathf.RoundToInt(Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration)));
                SetLabel(shown);
                yield return null;
            }

            currentValue = to;
            SetLabel(to);
        }

        private void SetLabel(int value)
        {
            if (label != null)
                label.text = $"{prefix}{value}{suffix}";
        }
    }
}
