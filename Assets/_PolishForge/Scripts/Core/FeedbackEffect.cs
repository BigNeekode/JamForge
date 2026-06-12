using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public abstract class FeedbackEffect : ScriptableObject
    {
        public string DisplayName = "Feedback Effect";
        public float Delay;
        public bool UseUnscaledTime;

        public abstract IEnumerator Play(FeedbackContext context);

        protected IEnumerator WaitDelay()
        {
            if (Delay <= 0f)
                yield break;

            if (UseUnscaledTime)
                yield return new WaitForSecondsRealtime(Delay);
            else
                yield return new WaitForSeconds(Delay);
        }
    }
}
