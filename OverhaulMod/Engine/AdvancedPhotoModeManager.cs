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

        private LightingInfo _nonEditedLightingInfo, _editedLightingInfo;

        private bool _isActive, _hasEverEnteredPhotoMode, _didActiveLightingChangedInGameplay;

        public LevelLightSettings editingLevelLightSettings
        {
            get;
            set;
        }

        private void Start()
        {
            _nonEditedLightingInfo = new LightingInfo();
            _editedLightingInfo = new LightingInfo();
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
                _didActiveLightingChangedInGameplay = true;
                return;
            }

            editingLevelLightSettings = changedLightSettings;
            _nonEditedLightingInfo.SetValues(changedLightSettings);
        }

        public void SetEditedLighting()
        {
            if (!_isActive)
                return;

            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings)
                return;

            _editedLightingInfo.ApplyValues(currentLevelLightSettings);
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

            _nonEditedLightingInfo.ApplyValues(currentLevelLightSettings);
            LevelEditorLightManager.Instance.RefreshLightInScene();
        }

        public void RestoreDefaults()
        {
            PhotoManager.Instance.OverridePausedTimeScale = 0.1f;
            Settings.SetDefaultSettings();

            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings)
                return;

            _nonEditedLightingInfo.ApplyValues(currentLevelLightSettings);
            _editedLightingInfo.SetValues(currentLevelLightSettings);
            LevelEditorLightManager.Instance.RefreshLightInScene();
        }

        public LightingInfo GetNormalLightingInfo()
        {
            return _nonEditedLightingInfo;
        }

        public LightingInfo GetEditedLightingInfo()
        {
            return _editedLightingInfo;
        }

        public bool IsActive()
        {
            return _isActive;
        }

        public bool DidActiveLightingChangedInGameplay()
        {
            return _didActiveLightingChangedInGameplay;
        }

        public void SetActiveLightingChangedInGameplay(bool value)
        {
            _didActiveLightingChangedInGameplay = value;
        }

        private void onEnteredPhotoMode()
        {
            if (!EnableAdvancedPhotoMode)
                return;

            _isActive = true;

            LevelLightSettings levelLightSettings = LevelEditorLightManager.Instance.GetActiveLightSettings();
            if (!levelLightSettings)
            {
                editingLevelLightSettings = null;
                return;
            }
            editingLevelLightSettings = levelLightSettings;

            _nonEditedLightingInfo.SetValues(levelLightSettings);

            if (!_hasEverEnteredPhotoMode || (_didActiveLightingChangedInGameplay && AutoResetLightingSettings))
            {
                _editedLightingInfo.SetValues(levelLightSettings);
                Settings.SetDefaultSettings();

                _hasEverEnteredPhotoMode = true;
            }
            _didActiveLightingChangedInGameplay = false;

            SetEditedLighting();
        }

        private void onExitedPhotoMode()
        {
            if (_isActive)
            {
                _isActive = false;

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
