using UnityEngine;

namespace OverhaulMod.Engine
{
    public class RestoreSpatalizeBlendOnDisable : MonoBehaviour
    {
        private AudioSource _audioSurce;

        public void Initialize(AudioSource audioSource)
        {
            _audioSurce = audioSource;
        }

        private void OnDisable()
        {
            if (_audioSurce) _audioSurce.spatialBlend = 1f;
            Destroy(this);
        }
    }
}