using System.Collections;
using UnityEngine;

namespace PolishForge
{
    [CreateAssetMenu(menuName = "PolishForge/Effects/VFX Spawn")]
    public class VfxSpawnEffect : FeedbackEffect
    {
        [SerializeField] private VfxCue cue;

        public override IEnumerator Play(FeedbackContext context)
        {
            yield return WaitDelay();
            VfxSpawner.Instance?.Spawn(cue, context);
        }
    }
}
