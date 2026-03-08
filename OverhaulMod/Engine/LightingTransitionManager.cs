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

        private LightingTransitionInfo _currentTransition;

        private LightingInfo _oldLevelLightInfo;

        public float transitionTime
        {
            get
            {
                return GameModeManager.IsOnTitleScreen() ? 2f : 3f;
            }
        }

        private float _timeLeft;
        private float _timeToAllowTransitionUpdates;

        public void DoTransition(LevelLightSettings b)
        {
            if (!b)
                return;

            LightingInfo aInfo = _oldLevelLightInfo;
            LightingInfo bInfo = new LightingInfo(b);
            _oldLevelLightInfo = bInfo;

            if (aInfo == null || aInfo.Equals(bInfo))
                return;

            LightingTransitionInfo lightingTransitionInfo = new LightingTransitionInfo
            {
                LightingA = aInfo,
                LightingB = bInfo,
                completion = 0f
            };
            lightingTransitionInfo.completion = 0f;
            _timeLeft = transitionTime;
            _timeToAllowTransitionUpdates = Time.time + 0.3f;
            _currentTransition = lightingTransitionInfo;
        }

        public bool IsDoingTransition()
        {
            return _currentTransition != null;
        }

        private void Update()
        {
            if (Time.time < _timeToAllowTransitionUpdates || _currentTransition == null || AdvancedPhotoModeManager.Instance.IsActive())
                return;

            float v = _timeLeft - Time.deltaTime;
            _timeLeft = v;

            bool levelHidesArena = false;
            LevelManager levelManager = LevelManager.Instance;
            if (levelManager && levelManager._currentLevelHidesTheArena)
                levelHidesArena = true;

            if (v <= 0f || levelHidesArena)
            {
                _currentTransition.completion = 1f;
                _currentTransition = null;
                LevelEditorLightManager.Instance.RefreshLightInScene();
            }
            else
            {
                _currentTransition.completion = 1f - NumberUtils.EaseInOutQuad(0f, 1f, Mathf.Clamp01(v / transitionTime));
            }
        }
    }
}
