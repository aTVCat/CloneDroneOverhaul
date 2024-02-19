using UnityEngine;

namespace OverhaulMod.Engine
{
    public class LightningTransitionManager : Singleton<LightningTransitionManager>
    {
        private LightningTransitionInfo m_currentTransition;

        private LightningInfo m_oldLightningInfo;

        private float m_timeLeft;

        public float transitionTime
        {
            get
            {
                return GameModeManager.IsOnTitleScreen() ? 0.85f : 4f;
            }
        }

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
            if (m_currentTransition == null)
                return;

            float d = Time.fixedDeltaTime;
            m_timeLeft -= d;
            m_currentTransition.completion = Mathf.Clamp01(1f - (m_timeLeft / transitionTime));

            if (m_timeLeft <= 0f)
            {
                m_currentTransition.completion = 1f;
                m_currentTransition = null;
                m_oldLightningInfo = null;
                LevelEditorLightManager.Instance.RefreshLightInScene();
                RealisticLightningManager.Instance.PatchLightning(false);
            }
        }
    }
}
