using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class LightingTransitionManager : Singleton<LightingTransitionManager>
    {
        [ModSetting(ModSettingsConstants.ENABLE_LIGHTING_TRANSITION, true)]
        public static bool EnableLightTransitions;

        public static bool allowLightTransitions
        {
            get
            {
                return EnableLightTransitions && (GameModeManager.Is(GameMode.None) || GameModeManager.Is(GameMode.Endless) || GameModeManager.Is(GameMode.EndlessCoop) || GameModeManager.Is(GameMode.CoopChallenge) || GameModeManager.Is(GameMode.Twitch) || GameModeManager.Is(GameMode.CoopChallenge) || (GameModeManager.Is(GameMode.Story) && !LevelManager.Instance.IsCurrentLevelHidingTheArena()));
            }
        }

        private LightingTransitionInfo m_currentTransition;

        private LightingInfo m_oldLevelLightInfo;

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

            LightingInfo aInfo = m_oldLevelLightInfo;
            LightingInfo bInfo = new LightingInfo(b);
            m_oldLevelLightInfo = bInfo;

            if (aInfo == null || aInfo.Equals(bInfo))
                return;

            LightingTransitionInfo lightingTransitionInfo = new LightingTransitionInfo
            {
                LightingA = aInfo,
                LightingB = bInfo,
                completion = 0f
            };
            lightingTransitionInfo.completion = 0f;
            m_timeLeft = transitionTime;
            m_timeToAllowTransitionUpdates = Time.time + 0.3f;
            m_currentTransition = lightingTransitionInfo;
        }

        public bool IsDoingTransition()
        {
            return m_currentTransition != null;
        }

        private void Update()
        {
            if (Time.time < m_timeToAllowTransitionUpdates || m_currentTransition == null)
                return;

            float v = m_timeLeft - Time.deltaTime;
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
            }
            else
            {
                m_currentTransition.completion = 1f - NumberUtils.EaseInOutCubic(0f, 1f, Mathf.Clamp01(v / transitionTime));
            }
        }
    }
}
