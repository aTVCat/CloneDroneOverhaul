using UnityEngine;

namespace OverhaulMod.Engine
{
    public class TransitionSoundBehaviour : Singleton<TransitionSoundBehaviour>
    {
        public const float VOLUME_MULTIPLIER = 0.7f;

        private AudioSource _source;

        private bool _aboutToDestroy;

        private float _timeSpawned, _timeLeftToDestroy;

        private void Update()
        {
            if (_aboutToDestroy)
            {
                _timeLeftToDestroy -= Mathf.Min(Time.unscaledDeltaTime, 0.02f) * 0.8f;
                if (_timeLeftToDestroy <= 0f)
                {
                    Destroy(base.gameObject);
                }
            }
            _source.volume = VOLUME_MULTIPLIER * Mathf.Clamp01(_aboutToDestroy ? _timeLeftToDestroy : ((Time.unscaledTime - _timeSpawned) * 2f));
        }

        public void Initialize(float volumeOffset)
        {
            _source = GetComponent<AudioSource>();
            _source.volume = volumeOffset * VOLUME_MULTIPLIER;
            _timeSpawned = Time.unscaledTime - volumeOffset;
        }

        public void FadeOutSoundThenDestroySelf()
        {
            _aboutToDestroy = true;
            _timeLeftToDestroy = 1f;
        }
    }
}
