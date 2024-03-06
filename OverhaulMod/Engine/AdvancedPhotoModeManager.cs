using OverhaulMod.UI;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace OverhaulMod.Engine
{
    public class AdvancedPhotoModeManager : Singleton<AdvancedPhotoModeManager>, IGameLoadListener
    {
        private LightningInfo m_nonEditedLightningInfo, m_editedLightningInfo;

        private bool m_isInPhotoMode, m_hasEverEnteredPhotoMode;

        public LevelLightSettings editingLevelLightSettings
        {
            get;
            set;
        }

        private void Start()
        {
            m_nonEditedLightningInfo = new LightningInfo();
            m_editedLightningInfo = new LightningInfo();
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
            LevelLightSettings currentLightSettings = editingLevelLightSettings;
            if (currentLightSettings != changedLightSettings)
            {
                LevelLightSettings newLightSettings = LevelEditorLightManager.Instance.GetActiveLightSettings();
                if(newLightSettings != changedLightSettings)
                    return;
            }

            editingLevelLightSettings = changedLightSettings;
            m_nonEditedLightningInfo.SetValues(currentLightSettings);
        }

        public void RefreshLightningWithEditedInfo()
        {
            if (!m_isInPhotoMode)
                return;

            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings)
                return;

            m_editedLightningInfo.ApplyValues(currentLevelLightSettings);
            LevelEditorLightManager.Instance.RefreshLightInScene();
        }

        public void RefreshLightningWithNormalInfo()
        {
            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings)
            {
                PostEffectsManager.Instance.RefreshCameraPostEffects();
                return;
            }

            m_nonEditedLightningInfo.ApplyValues(currentLevelLightSettings);
            LevelEditorLightManager.Instance.RefreshLightInScene();
        }

        public void RestoreDefaults()
        {
            PhotoManager.Instance.OverridePausedTimeScale = 0.1f;
            Settings.SetDefaultSettings();

            LevelLightSettings currentLevelLightSettings = editingLevelLightSettings;
            if (!currentLevelLightSettings)
                return;

            m_nonEditedLightningInfo.ApplyValues(currentLevelLightSettings);
            m_editedLightningInfo.SetValues(currentLevelLightSettings);
            LevelEditorLightManager.Instance.RefreshLightInScene();
        }

        public LightningInfo GetNormalLightningInfo()
        {
            return m_nonEditedLightningInfo;
        }

        public LightningInfo GetEditedLightningInfo()
        {
            return m_editedLightningInfo;
        }

        public bool IsInPhotoMode()
        {
            return m_isInPhotoMode;
        }

        private void onEnteredPhotoMode()
        {
            m_isInPhotoMode = true;

            LevelLightSettings levelLightSettings = LevelEditorLightManager.Instance.GetActiveLightSettings();
            if(!levelLightSettings)
            {
                editingLevelLightSettings = null;
                return;
            }
            editingLevelLightSettings = levelLightSettings;

            m_nonEditedLightningInfo.SetValues(levelLightSettings);

            if (!m_hasEverEnteredPhotoMode)
            {
                m_editedLightningInfo.SetValues(levelLightSettings);
                Settings.SetDefaultSettings();

                m_hasEverEnteredPhotoMode = true;
            }

            RefreshLightningWithEditedInfo();
        }

        private void onExitedPhotoMode()
        {
            m_isInPhotoMode = false;

            RealisticLightningManager.Instance.PatchLevelLightSettings();
            RefreshLightningWithNormalInfo();

            UIPhotoModeUIRework photoModeUI = ModUIManager.Instance?.Get<UIPhotoModeUIRework>(AssetBundleConstants.UI, ModUIConstants.UI_PHOTO_MODE_UI_REWORK);
            if (photoModeUI)
                photoModeUI.ResetEnvironmentFields();
        }

        public static class Settings
        {
            public static bool overrideSettings
            {
                get
                {
                    AdvancedPhotoModeManager advancedPhotoModeManager = AdvancedPhotoModeManager.Instance;
                    return advancedPhotoModeManager && advancedPhotoModeManager.IsInPhotoMode();
                }
            }

            public static bool EnableVignette, EnableDithering, EnableSSAO;

            public static float VignetteIntensity;

            public static void SetDefaultSettings()
            {
                VignetteIntensity = 0.5f;
                EnableVignette = UIImageEffects.EnableVignette;
                EnableDithering = UIImageEffects.EnableDithering;
                EnableSSAO = PostEffectsManager.EnableSSAO;
            }
        }
    }
}
