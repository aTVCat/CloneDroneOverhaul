using OverhaulMod.UI;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;

namespace OverhaulMod.Engine
{
    public class AdvancedPhotoModeManager : Singleton<AdvancedPhotoModeManager>, IGameLoadListener
    {
        [ModSetting(ModSettingsConstants.ADVANCED_PHOTO_MODE, true)]
        public static bool EnableAdvancedPhotoMode;

        [ModSetting(ModSettingsConstants.REQUIRE_RMB_HOLD_WHEN_UI_IS_HIDDEN, false)]
        public static bool RequireHoldingRMBWhenUIIsHidden;

        [ModSetting(ModSettingsConstants.AUTO_RESET_LIGHTING_SETTINGS, true)]
        public static bool AutoResetLightingSettings;

        private LightingInfo m_nonEditedLightingInfo, m_editedLightingInfo;

        private bool m_isActive, m_hasEverEnteredPhotoMode, m_didActiveLightingChangedInGameplay;

        public LevelLightSettings editingLevelLightSettings
        {
            get;
            set;
        }

        private void Start()
        {
            m_nonEditedLightingInfo = new LightingInfo();
            m_editedLightingInfo = new LightingInfo();
        }

        public void OnGameLoaded()
        {
            GlobalEventManager.Instance.AddEventListener("EnteredPhotoMode", onEnteredPhotoMode);
            GlobalEventManager.Instance.AddEventListener("ExitedPhotoMode", onExitedPhotoMode);
        }

        private void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener("EnteredPhotoMode", onEnteredPhotoMode);
            GlobalEventManager.Instance.RemoveEventListener("ExitedPhotoMode", onExitedPhotoMode);
        }

        public void OnLevelLightSettingsChanged(LevelLightSettings changedLightSettings)
        {
            LevelLightSettings editingLightSettings = editingLevelLightSettings;
            if (editingLightSettings != changedLightSettings && !IsActive())
            {
                m_didActiveLightingChangedInGameplay = true;
                return;
            }

            editingLevelLightSettings = changedLightSettings;
            m_nonEditedLightingInfo.SetValues(changedLightSettings);
        }

        public void SetEditedLighting()
        {
            if (!m_isActive)
                return;

            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings)
                return;

            m_editedLightingInfo.ApplyValues(currentLevelLightSettings);
            LevelEditorLightManager.Instance.RefreshLightInScene();
        }

        public void SetNormalLighting()
        {
            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings)
            {
                PostEffectsManager.Instance.RefreshCameraPostEffects();
                return;
            }

            m_nonEditedLightingInfo.ApplyValues(currentLevelLightSettings);
            LevelEditorLightManager.Instance.RefreshLightInScene();
        }

        public void RestoreDefaults()
        {
            PhotoManager.Instance.OverridePausedTimeScale = 0.1f;
            Settings.SetDefaultSettings();

            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings)
                return;

            m_nonEditedLightingInfo.ApplyValues(currentLevelLightSettings);
            m_editedLightingInfo.SetValues(currentLevelLightSettings);
            LevelEditorLightManager.Instance.RefreshLightInScene();
        }

        public LightingInfo GetNormalLightingInfo()
        {
            return m_nonEditedLightingInfo;
        }

        public LightingInfo GetEditedLightingInfo()
        {
            return m_editedLightingInfo;
        }

        public bool IsActive()
        {
            return m_isActive;
        }

        public bool DidActiveLightingChangedInGameplay()
        {
            return m_didActiveLightingChangedInGameplay;
        }

        public void SetActiveLightingChangedInGameplay(bool value)
        {
            m_didActiveLightingChangedInGameplay = value;
        }

        private void onEnteredPhotoMode()
        {
            if (!EnableAdvancedPhotoMode)
                return;

            m_isActive = true;

            LevelLightSettings levelLightSettings = LevelEditorLightManager.Instance.GetActiveLightSettings();
            if (!levelLightSettings)
            {
                editingLevelLightSettings = null;
                return;
            }
            editingLevelLightSettings = levelLightSettings;

            m_nonEditedLightingInfo.SetValues(levelLightSettings);

            if (!m_hasEverEnteredPhotoMode || (m_didActiveLightingChangedInGameplay && AutoResetLightingSettings))
            {
                m_editedLightingInfo.SetValues(levelLightSettings);
                Settings.SetDefaultSettings();

                m_hasEverEnteredPhotoMode = true;
            }
            m_didActiveLightingChangedInGameplay = false;

            SetEditedLighting();
        }

        private void onExitedPhotoMode()
        {
            if (m_isActive)
            {
                m_isActive = false;

                SetNormalLighting();

                UIPhotoModeUIRework photoModeUI = ModUIManager.Instance?.Get<UIPhotoModeUIRework>(AssetBundleConstants.UI, ModUIConstants.UI_PHOTO_MODE_UI_REWORK);
                if (photoModeUI)
                    photoModeUI.ResetEnvironmentFields();
            }
        }

        public static class Settings
        {
            public static bool overrideSettings
            {
                get
                {
                    AdvancedPhotoModeManager advancedPhotoModeManager = Instance;
                    return advancedPhotoModeManager && advancedPhotoModeManager.IsActive();
                }
            }

            public static bool EnableReflectionProbe, EnableGlobalIllumination, EnableSSAO, EnableVignette, EnableDithering, EnableCA, EnableSunShafts, EnableDoF;

            public static float VignetteIntensity;

            public static void SetDefaultSettings()
            {
                VignetteIntensity = 0.23f;
                EnableReflectionProbe = PostEffectsManager.EnableReflectionProbe;
                EnableGlobalIllumination = PostEffectsManager.EnableGlobalIllumination;
                EnableSSAO = PostEffectsManager.EnableSSAO;
                EnableVignette = PostEffectsManager.EnableVignette;
                EnableDithering = PostEffectsManager.EnableDithering;
                EnableCA = PostEffectsManager.EnableChromaticAberration;
                EnableSunShafts = PostEffectsManager.EnableSunShafts;
                EnableDoF = PostEffectsManager.EnableDoF;
            }
        }
    }
}
