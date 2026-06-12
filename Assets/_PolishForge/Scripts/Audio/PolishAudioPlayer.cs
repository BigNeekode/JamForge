using UnityEngine;

namespace PolishForge
{
    public class PolishAudioPlayer : PolishService<PolishAudioPlayer>
    {
        [SerializeField] private AudioSource source2D;

        protected override void Awake()
        {
            base.Awake();
            if (source2D == null)
            {
                source2D = gameObject.AddComponent<AudioSource>();
                source2D.playOnAwake = false;
            }
        }

        public void Play(PolishAudioCue cue, Vector3 position, Transform attachTo = null, float intensity = 1f)
        {
            if (cue == null || cue.Clips == null || cue.Clips.Count == 0)
            {
                Debug.LogWarning("[PolishForge] PolishAudioPlayer: audio cue has no clips.");
                return;
            }

            if (!cue.CanPlay)
                return;

            AudioClip clip = cue.Clips[Random.Range(0, cue.Clips.Count)];
            if (clip == null)
            {
                Debug.LogWarning("[PolishForge] PolishAudioPlayer: selected audio clip is missing.");
                return;
            }

            float volume = Random.Range(cue.VolumeRange.x, cue.VolumeRange.y) * Mathf.Max(0f, intensity);
            float pitch = Random.Range(cue.PitchRange.x, cue.PitchRange.y);
            cue.MarkPlayed();

            if (!cue.Spatial)
            {
                source2D.pitch = pitch;
                source2D.PlayOneShot(clip, volume);
                return;
            }

            GameObject audioObject = new($"PolishAudio_{clip.name}");
            audioObject.transform.position = attachTo != null ? attachTo.position : position;
            if (attachTo != null)
                audioObject.transform.SetParent(attachTo, true);

            AudioSource spatialSource = audioObject.AddComponent<AudioSource>();
            spatialSource.clip = clip;
            spatialSource.volume = volume;
            spatialSource.pitch = pitch;
            spatialSource.spatialBlend = cue.SpatialBlend;
            spatialSource.Play();
            Destroy(audioObject, clip.length / Mathf.Max(0.01f, Mathf.Abs(pitch)) + 0.1f);
        }
    }
}
