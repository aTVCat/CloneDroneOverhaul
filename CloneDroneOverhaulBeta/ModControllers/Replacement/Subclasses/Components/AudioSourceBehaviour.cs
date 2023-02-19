using UnityEngine;

namespace CDOverhaul.Patches
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceBehaviour : MonoBehaviour
    {
        public AudioSource ASource;
        private WorldAudioSource _worldAudioSource;

        private bool _loop;

        public void Initialize(in AudioSource source, in bool loop)
        {
            ASource = source;
            _worldAudioSource = source.GetComponent<WorldAudioSource>();
            _loop = loop;

            PatchSettings(source, _worldAudioSource);
        }

        public static void PatchSettings(in AudioSource source, in WorldAudioSource worldASource)
        {
            if (worldASource != null)
            {
                worldASource.MaxDistance = Mathf.Clamp(worldASource.MaxDistance, 0, 150);
            }

            source.maxDistance = Mathf.Clamp(source.maxDistance, 0, 150);
        }
    }
}