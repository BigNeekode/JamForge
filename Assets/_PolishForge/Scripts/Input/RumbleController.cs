using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PolishForge
{
    public class RumbleController : PolishService<RumbleController>
    {
        private Coroutine activeRoutine;

        public void Rumble(float lowFrequency, float highFrequency, float duration)
        {
            if (PolishSettings.Instance != null)
            {
                if (!PolishSettings.Instance.EnableRumble)
                    return;

                float scale = PolishSettings.Instance.MasterPolishIntensity * PolishSettings.Instance.RumbleIntensity;
                lowFrequency *= scale;
                highFrequency *= scale;
            }

            if (Gamepad.current == null)
                return;

            if (activeRoutine != null)
                StopCoroutine(activeRoutine);

            activeRoutine = StartCoroutine(RumbleRoutine(lowFrequency, highFrequency, duration));
        }

        private IEnumerator RumbleRoutine(float lowFrequency, float highFrequency, float duration)
        {
            Gamepad.current?.SetMotorSpeeds(lowFrequency, highFrequency);
            yield return new WaitForSecondsRealtime(Mathf.Max(0.01f, duration));
            Gamepad.current?.SetMotorSpeeds(0f, 0f);
            activeRoutine = null;
        }
    }
}
