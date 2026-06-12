using System.Collections;
using UnityEngine;

namespace PolishForge
{
    public class FeedbackPlayer : MonoBehaviour
    {
        private static FeedbackPlayer instance;

        public static FeedbackPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<FeedbackPlayer>();
                    if (instance == null)
                    {
                        GameObject created = new("PolishFeedbackPlayer");
                        instance = created.AddComponent<FeedbackPlayer>();
                    }
                }

                return instance;
            }
        }

        public static int ActiveEffectCount { get; private set; }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Play(FeedbackPreset preset, FeedbackContext context)
        {
            if (preset == null)
            {
                Debug.LogWarning("[PolishForge] FeedbackPlayer: missing feedback preset.");
                return;
            }

            StartCoroutine(PlayPreset(preset, context));
        }

        public static void PlayGlobal(FeedbackPreset preset, FeedbackContext context)
        {
            Instance.Play(preset, context);
        }

        private IEnumerator PlayPreset(FeedbackPreset preset, FeedbackContext context)
        {
            if (preset.PlayInParallel)
            {
                foreach (FeedbackEffect effect in preset.Effects)
                {
                    if (effect == null)
                    {
                        Debug.LogWarning($"[PolishForge] FeedbackPlayer: preset '{preset.name}' contains a missing effect.");
                        continue;
                    }

                    StartCoroutine(PlayEffect(effect, context));
                }
            }
            else
            {
                foreach (FeedbackEffect effect in preset.Effects)
                {
                    if (effect == null)
                    {
                        Debug.LogWarning($"[PolishForge] FeedbackPlayer: preset '{preset.name}' contains a missing effect.");
                        continue;
                    }

                    yield return PlayEffect(effect, context);
                }
            }
        }

        private IEnumerator PlayEffect(FeedbackEffect effect, FeedbackContext context)
        {
            ActiveEffectCount++;
            yield return effect.Play(context);
            ActiveEffectCount = Mathf.Max(0, ActiveEffectCount - 1);
        }
    }
}
