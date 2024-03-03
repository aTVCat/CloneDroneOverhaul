using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class LightningTransitionManager : Singleton<LightningTransitionManager>
    {
        [ModSetting(ModSettingsConstants.ENABLE_LIGHTNING_TRANSITION, true)]
        public static bool TransitionsEnabled;

        private LightningTransitionInfo m_currentTransition;

        private LightningInfo m_oldLevelLightInfo;

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
            if (!b)
                return;

            LightningInfo aInfo = m_oldLevelLightInfo;
            LightningInfo bInfo = new LightningInfo(b);
            m_oldLevelLightInfo = bInfo;

            if (aInfo == null || aInfo.Equals(bInfo))
                return;

            LightningTransitionInfo lightningTransitionInfo = new LightningTransitionInfo
            {
                LightningA = aInfo,
                LightningB = bInfo,
                completion = 0f
            };
            lightningTransitionInfo.completion = 0f;
            m_timeLeft = transitionTime;
            m_timeToAllowTransitionUpdates = Time.time + 0.3f;
            m_currentTransition = lightningTransitionInfo;
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

            bool levelHidesArena = false;
            LevelManager levelManager = LevelManager.Instance;
            if (levelManager && levelManager._currentLevelHidesTheArena)
                levelHidesArena = true;

            if (v <= 0f || levelHidesArena)
            {
                m_currentTransition.completion = 1f;
                m_currentTransition = null;
                LevelEditorLightManager.Instance.RefreshLightInScene();
                RealisticLightningManager.Instance.PatchLightning(false);
            }
            else
            {
                m_currentTransition.completion = 1f - ModITweenUtils.ParametricBlend(Mathf.Clamp01(v / transitionTime));
            }
        }
    }
}
