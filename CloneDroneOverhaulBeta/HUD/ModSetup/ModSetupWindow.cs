using CDOverhaul.Device;
using CDOverhaul.Graphics;
using CDOverhaul.Patches;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class ModSetupWindow : OverhaulUI
    {
        public const string SettingPath = "Player.Mod.HasConfiguredModV7";

        [OverhaulSetting(SettingPath, false, true)]
        public static bool HasSetTheModUp;

        private Button m_DoneButton;
        private ScrollRect m_ScrollRect;

        private TwoButtonsToggle m_SSAOToggle;
        private TwoButtonsToggle m_BloomOverhaulToggle;
        private TwoButtonsToggle m_VignetteToggle;
        private TwoButtonsToggle m_ChromaticAberrationToggle;

        private TwoButtonsToggle m_CameraRollingToggle;

        private TwoButtonsToggle m_WatermarkToggle;
        private TwoButtonsToggle m_NewEnergyUIToggle;

        private TwoButtonsToggle m_AudioReverbToggle;
        private TwoButtonsToggle m_SendCrashReportsToggle;

        private Button m_BrightAmpColPreset;
        private Button m_DarkAmpColPreset;
        private Button m_DefaultAmpColPreset;

        public override void Initialize()
        {
            base.gameObject.SetActive(false);

            m_DoneButton = MyModdedObject.GetObject<Button>(0);
            m_DoneButton.onClick.AddListener(EndSetup);
            m_ScrollRect = MyModdedObject.GetObject<ScrollRect>(22);
            m_ScrollRect.verticalNormalizedPosition = 1f;

            MyModdedObject.GetObject<Transform>(23).gameObject.AddComponent<RecommendationTooltip>().Initialize(Recommendations.GetSSAORecommendation(), Recommendations.GetSSAORecommendationString());

            m_SSAOToggle = new TwoButtonsToggle(MyModdedObject, 2, 1, SetSSAODisabled, SetSSAOEnabled, GetSSAOEnabled, GetSSAOSupported);
            m_BloomOverhaulToggle = new TwoButtonsToggle(MyModdedObject, 4, 3, SetBloomOverhaulDisabled, SetBloomOverhaulEnabled, GetBloomOverhaulEnabled);
            m_VignetteToggle = new TwoButtonsToggle(MyModdedObject, 6, 5, SetVignetteDisabled, SetVignetteEnabled, GetVignetteEnabled);
            m_ChromaticAberrationToggle = new TwoButtonsToggle(MyModdedObject, 8, 7, SetCADisabled, SetCAEnabled, GetCAEnabled);

            m_CameraRollingToggle = new TwoButtonsToggle(MyModdedObject, 10, 9, SetCRDisabled, SetCREnabled, GetCREnabled);

            m_WatermarkToggle = new TwoButtonsToggle(MyModdedObject, 15, 14, SetWatermarkDisabled, SetWatermarkEnabled, GetWatermarkEnabled);
            m_NewEnergyUIToggle = new TwoButtonsToggle(MyModdedObject, 17, 16, SetNewEnergyUIDisabled, SetNewEnergyUIEnabled, GetNewEnergyUIEnabled);

            m_AudioReverbToggle = new TwoButtonsToggle(MyModdedObject, 19, 18, SetAudioReverbDisabled, SetAudioReverbEnabled, GetAudioReverbEnabled);
            m_SendCrashReportsToggle = new TwoButtonsToggle(MyModdedObject, 21, 20, SetSendCrashReportsDisabled, SetSendCrashReportsEnabled, GetSendCrashReportsEnabled);

            m_BrightAmpColPreset = MyModdedObject.GetObject<Button>(13);
            m_BrightAmpColPreset.onClick.AddListener(SetBrightAmpColPreset);
            m_DarkAmpColPreset = MyModdedObject.GetObject<Button>(12);
            m_DarkAmpColPreset.onClick.AddListener(SetDarkAmpColPreset);
            m_DefaultAmpColPreset = MyModdedObject.GetObject<Button>(11);
            m_DefaultAmpColPreset.onClick.AddListener(SetDefaultAmpColPreset);

            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsFirstUseSetupUIEnabled || ModSetupWindow.HasSetTheModUp)
                return;

            ArenaCameraManager.Instance.ArenaCameraTransform.position = new Vector3(-43, 15, -3);
            ArenaCameraManager.Instance.ArenaCameraTransform.eulerAngles = new Vector3(340, 276, 351);
            DelegateScheduler.Instance.Schedule(Show, 1f);
        }

        public void Show()
        {
            if (!GameModeManager.IsOnTitleScreen())
                return;

            base.gameObject.SetActive(true);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
        }

        public void EndSetup()
        {
            OverhaulSettingsController.SetSettingValue(SettingPath, true);
            Hide();
        }

        public void AllowEndingTheSetup()
        {
            m_DoneButton.interactable = true;
        }

        public void DisallowEndingTheSetup()
        {
            m_DoneButton.interactable = false;
        }

        #region SSAO

        public bool GetSSAOSupported()
        {
            return Recommendations.GetSSAORecommendation() != RecommendationLevel.Unsupported;
        }
        public bool GetSSAOEnabled()
        {
            return GetSSAOSupported() && OverhaulGraphicsController.AOEnabled;
        }
        public void SetSSAOEnabled()
        {
            OverhaulSettingsController.SetSettingValue("Graphics.Amplify Occlusion.Enable", true);
        }
        public void SetSSAODisabled()
        {
            OverhaulSettingsController.SetSettingValue("Graphics.Amplify Occlusion.Enable", false);
        }

        #endregion

        #region Bloom Overhaul

        public bool GetBloomOverhaulEnabled()
        {
            return OverhaulGraphicsController.BloomIterations == 10;
        }
        public void SetBloomOverhaulEnabled()
        {
            OverhaulSettingsController.ResetSettingValue("Graphics.Post effects.Enable bloom");
            OverhaulSettingsController.ResetSettingValue("Graphics.Post effects.Bloom iterations");
            OverhaulSettingsController.ResetSettingValue("Graphics.Post effects.Bloom intensity");
            OverhaulSettingsController.ResetSettingValue("Graphics.Post effects.Bloom Threshold");
        }
        public void SetBloomOverhaulDisabled()
        {
            OverhaulGraphicsController.SetBloomVanilla.EventAction.Invoke();
        }

        #endregion

        #region Vignette

        public bool GetVignetteEnabled()
        {
            return OverhaulGraphicsController.VignetteEnabled;
        }
        public void SetVignetteEnabled()
        {
            OverhaulSettingsController.SetSettingValue("Graphics.Shaders.Vignette", true);
        }
        public void SetVignetteDisabled()
        {
            OverhaulSettingsController.SetSettingValue("Graphics.Shaders.Vignette", false);
        }

        #endregion

        #region Chromatic Aberration

        public bool GetCAEnabled()
        {
            return OverhaulGraphicsController.ChromaticAberrationEnabled;
        }
        public void SetCAEnabled()
        {
            OverhaulSettingsController.SetSettingValue("Graphics.Shaders.Chromatic Aberration", true);
        }
        public void SetCADisabled()
        {
            OverhaulSettingsController.SetSettingValue("Graphics.Shaders.Chromatic Aberration", false);
        }

        #endregion

        #region Camera Rolling

        public bool GetCREnabled()
        {
            return CameraRollingBehaviour.EnableCameraRolling;
        }
        public void SetCREnabled()
        {
            OverhaulSettingsController.SetSettingValue("Graphics.Camera.Rolling", true);
        }
        public void SetCRDisabled()
        {
            OverhaulSettingsController.SetSettingValue("Graphics.Camera.Rolling", false);
        }

        #endregion

        #region Amplify Color

        public void SetBrightAmpColPreset()
        {
            OverhaulGraphicsController.ApplyAmplifyColorPreset1.EventAction.Invoke();
        }

        public void SetDarkAmpColPreset()
        {
            OverhaulGraphicsController.ApplyAmplifyColorPreset2.EventAction.Invoke();
        }

        public void SetDefaultAmpColPreset()
        {
            OverhaulGraphicsController.ApplyAmplifyColorPresetDefault.EventAction.Invoke();
        }

        #endregion


        #region Watermark

        public bool GetWatermarkEnabled()
        {
            return OverhaulVersionLabel.EnableWatermarkInGameplay;
        }
        public void SetWatermarkEnabled()
        {
            OverhaulSettingsController.SetSettingValue("Game interface.Information.Watermark", true);
        }
        public void SetWatermarkDisabled()
        {
            OverhaulSettingsController.SetSettingValue("Game interface.Information.Watermark", false);
        }

        #endregion

        #region New energy UI

        public bool GetNewEnergyUIEnabled()
        {
            return EnergyUIReplacement.PatchHUD;
        }
        public void SetNewEnergyUIEnabled()
        {
            OverhaulSettingsController.SetSettingValue("Game interface.Gameplay.New energy bar design", true);
        }
        public void SetNewEnergyUIDisabled()
        {
            OverhaulSettingsController.SetSettingValue("Game interface.Gameplay.New energy bar design", false);
        }

        #endregion


        #region Audio reverb

        public bool GetAudioReverbEnabled()
        {
            return WorldAudioSource_Patch.EnableReverbFilter;
        }
        public void SetAudioReverbEnabled()
        {
            OverhaulSettingsController.SetSettingValue("Audio.Filters.Reverb", true);
        }
        public void SetAudioReverbDisabled()
        {
            OverhaulSettingsController.SetSettingValue("Audio.Filters.Reverb", false);
        }

        #endregion

        #region Send crash reports

        public bool GetSendCrashReportsEnabled()
        {
            return OverhaulWebhooksController.AllowSendingInformation;
        }
        public void SetSendCrashReportsEnabled()
        {
            OverhaulSettingsController.SetSettingValue("Mod.Information.Send crash reports", true);
        }
        public void SetSendCrashReportsDisabled()
        {
            OverhaulSettingsController.SetSettingValue("Mod.Information.Send crash reports", false);
        }

        #endregion
    }
}
