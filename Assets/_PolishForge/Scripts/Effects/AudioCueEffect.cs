using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/Audio Cue")]
    public class AudioCueEffect : FeedbackEffect
    {
        [SerializeField] private PolishAudioCue audioCue;
        [SerializeField] private bool playAtContextPosition = true;
        [SerializeField] private bool attachToTarget;
        [SerializeField] private bool scaleVolumeByIntensity = true;

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            Transform attach = attachToTarget && context.Target != null ? context.Target.transform : null;
            Vector3 position = playAtContextPosition ? context.EffectivePosition : Vector3.zero;
            float intensity = scaleVolumeByIntensity ? context.EffectiveIntensity : 1f;
            PolishAudioPlayer.Instance?.Play(audioCue, position, attach, intensity);
        }
    }
}
