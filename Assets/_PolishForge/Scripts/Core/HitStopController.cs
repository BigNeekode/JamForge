using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public class HitStopController : PolishService<HitStopController>
    {
        private Coroutine activeRoutine;
        private float previousTimeScale = 1f;

        public void HitStop(float duration, float timeScaleDuringStop, bool useUnscaledWait)
        {
            if (PolishSettings.Instance != null && !PolishSettings.Instance.EnableHitStop)
                return;

            if (duration <= 0f)
                return;

            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
                Time.timeScale = previousTimeScale;
            }

            activeRoutine = StartCoroutine(HitStopRoutine(duration, Mathf.Clamp01(timeScaleDuringStop), useUnscaledWait));
        }

        private IEnumerator HitStopRoutine(float duration, float timeScaleDuringStop, bool useUnscaledWait)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = timeScaleDuringStop;

            if (useUnscaledWait)
                yield return new WaitForSecondsRealtime(duration);
            else
                yield return new WaitForSeconds(duration);

            Time.timeScale = previousTimeScale;
            activeRoutine = null;
        }
    }
}
