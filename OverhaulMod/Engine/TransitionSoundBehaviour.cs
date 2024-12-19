using UnityEngine;

namespace OverhaulMod.Engine
{
    public class TransitionSoundBehaviour : Singleton<TransitionSoundBehaviour>
    {
        private AudioSource m_source;

        private bool m_aboutToDestroy;

        private float m_timeSpawned, m_timeLeftToDestroy;

        private void Update()
        {
            if (m_aboutToDestroy)
            {
                m_timeLeftToDestroy -= Mathf.Min(Time.unscaledDeltaTime, 0.02f);
                if (m_timeLeftToDestroy <= 0f)
                {
                    Destroy(base.gameObject);
                }
            }
            m_source.volume = Mathf.Clamp01(m_aboutToDestroy ? m_timeLeftToDestroy : ((Time.unscaledTime - m_timeSpawned) * 2f));
        }

        public void Initialize(float volumeOffset)
        {
            m_source = GetComponent<AudioSource>();
            m_source.volume = volumeOffset;
            m_timeSpawned = Time.unscaledTime - volumeOffset;
        }

        public void FadeOutSoundThenDestroySelf()
        {
            m_aboutToDestroy = true;
            m_timeLeftToDestroy = 1f;
        }
    }
}
