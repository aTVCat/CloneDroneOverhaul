using System;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class LightningTransitionManager : Singleton<LightningTransitionManager>
    {
        private LightningTransitionInfo m_currentTransition;

        private LightningInfo m_oldLightningInfo;

        public float transitionTime
        {
            get
            {
                return GameModeManager.IsOnTitleScreen() ? 2f : 3f;
            }
        }

        private float m_timeLeft;
        private float m_timeToAllowTransitionUpdates;

        public void DoTransition(LevelLightSettings b)
        {
            if (!b || m_oldLightningInfo == null)
                return;

            LightningTransitionInfo lightningTransitionInfo = new LightningTransitionInfo
            {
                LightningA = m_oldLightningInfo,
                LightningB = new LightningInfo(b),
                completion = 0f
            };
            m_timeLeft = transitionTime;
            m_timeToAllowTransitionUpdates = Time.time + 0.5f;
            m_currentTransition = lightningTransitionInfo;
        }

        public void SetOldLightningInfo(LevelLightSettings levelLightSettings)
        {
            m_oldLightningInfo = new LightningInfo(levelLightSettings);
        }

        public bool ShouldDoTransition()
        {
            return m_oldLightningInfo != null;
        }

        public bool IsDoingTransition()
        {
            return m_currentTransition != null;
        }

        private void FixedUpdate()
        {
            if (Time.time < m_timeToAllowTransitionUpdates || m_currentTransition == null)
                return;

            float v = m_timeLeft - Time.fixedDeltaTime;
            m_timeLeft = v;

            if (v <= 0f)
            {
                m_currentTransition.completion = 1f;
                m_currentTransition = null;
                m_oldLightningInfo = null;
                LevelEditorLightManager.Instance.RefreshLightInScene();
                RealisticLightningManager.Instance.PatchLightning(false);
            }
            else
            {
                m_currentTransition.completion = 1f - ParametricBlend(Mathf.Clamp01(v / transitionTime));
            }
        }

        private float ParametricBlend(float t)
        {
            float sqr = t * t;
            return sqr / (2.0f * (sqr - t) + 1.0f);
        }
    }
}
