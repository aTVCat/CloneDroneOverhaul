using OverhaulMod.UI;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;

namespace OverhaulMod.Engine
{
    public class AdvancedPhotoModeManager : Singleton<AdvancedPhotoModeManager>, IGameLoadListener
    {
        [ModSetting(ModSettingIDs.ADVANCED_PHOTO_MODE, true)]
        public static bool EnableAdvancedPhotoMode;

        [ModSetting(ModSettingIDs.REQUIRE_RMB_HOLD_WHEN_UI_IS_HIDDEN, false)]
        public static bool RequireHoldingRMBWhenUIIsHidden;

        [ModSetting(ModSettingIDs.AUTO_RESET_LIGHTING_SETTINGS, true)]
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
            if (IsActive())
            {
                editingLevelLightSettings = changedLightSettings;
                _nonEditedLightingInfo.SetValues(changedLightSettings);
                _editedLightingInfo.ApplyValues(changedLightSettings);
            }
            else if (editingLevelLightSettings != changedLightSettings)
            {
                _didActiveLightingChangedInGameplay = true;
            }
        }

        public void SetEditedLighting()
        {
            if (!_isActive) return;

            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings) return;

            _editedLightingInfo.ApplyValues(currentLevelLightSettings);
            RefreshCurrentLightInScene();
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
            RefreshCurrentLightInScene();
        }

        public void RestoreDefaults()
        {
            PhotoManager.Instance.OverridePausedTimeScale = 0.1f;
            Settings.SetDefaultSettings();

            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings) return;

            _nonEditedLightingInfo.ApplyValues(currentLevelLightSettings);
            _editedLightingInfo.SetValues(currentLevelLightSettings);
            RefreshCurrentLightInScene();
        }

        public void RefreshCurrentLightInScene()
        {
            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings) return;

            DirectionalLightManager.Instance.RefreshDirectionalLight(currentLevelLightSettings);
            SkyBoxManager.Instance.RefreshSkyboxAmbientLightAndFog(currentLevelLightSettings);
            GlobalEventManager.Instance.Dispatch(GlobalEvents.LightSettingsRefreshed);

            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public LightingInfo GetNormalLightingInfo() => _nonEditedLightingInfo;

        public LightingInfo GetEditedLightingInfo() => _editedLightingInfo;

        public bool IsActive() => _isActive;

        private void onEnteredPhotoMode()
        {
            if (!EnableAdvancedPhotoMode || _isActive) return;
            _isActive = true;

            LevelLightSettings levelLightSettings = LevelEditorLightManager.Instance._selectedLightSettings;
            editingLevelLightSettings = levelLightSettings;
            if (!levelLightSettings) return;

            _nonEditedLightingInfo.SetValues(levelLightSettings);

            if (!_hasEverEnteredPhotoMode || (_didActiveLightingChangedInGameplay && AutoResetLightingSettings))
            {
                _editedLightingInfo.SetValues(levelLightSettings);
                Settings.SetDefaultSettings();
            }

            _hasEverEnteredPhotoMode = true;
            _didActiveLightingChangedInGameplay = false;

            SetEditedLighting();
        }

        private void onExitedPhotoMode()
        {
            if (!_isActive) return;
            _isActive = false;

            SetNormalLighting();

            UIPhotoModeUIRework photoModeUI = ModUIManager.Instance?.Get<UIPhotoModeUIRework>(AssetBundleConstants.UI, ModUIConstants.UI_PHOTO_MODE_UI_REWORK);
            if (photoModeUI) photoModeUI.ResetEnvironmentFields();
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

            public static bool EnableGlobalIllumination, EnableSSAO, EnableVignette, EnableDithering, EnableCA, EnableSunShafts, EnableDoF;

            public static float VignetteIntensity;

            public static void SetDefaultSettings()
            {
                VignetteIntensity = 0.23f;
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