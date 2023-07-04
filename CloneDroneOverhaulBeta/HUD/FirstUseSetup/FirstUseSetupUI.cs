using CDOverhaul.Graphics;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class FirstUseSetupUI : OverhaulUI
    {
        public const string SettingPath = "Player.Mod.HasConfiguredModV3";

        [OverhaulSetting(SettingPath, false, true)]
        public static bool HasSetTheModUp;

        private Button m_DoneButton;

        private TwoButtonsToggle m_SSAOToggle;
        private TwoButtonsToggle m_BloomOverhaulToggle;
        private TwoButtonsToggle m_VignetteToggle;
        private TwoButtonsToggle m_ChromaticAberrationToggle;

        private TwoButtonsToggle m_CameraRollingToggle;

        private Button m_BrightAmpColPreset;
        private Button m_DarkAmpColPreset;
        private Button m_DefaultAmpColPreset;

        public override void Initialize()
        {
            base.gameObject.SetActive(false);

            m_DoneButton = MyModdedObject.GetObject<Button>(0);
            m_DoneButton.onClick.AddListener(EndSetup);

            m_SSAOToggle = new TwoButtonsToggle(MyModdedObject, 2, 1, SetSSAODisabled, SetSSAOEnabled, GetSSAOEnabled);
            m_BloomOverhaulToggle = new TwoButtonsToggle(MyModdedObject, 4, 3, SetBloomOverhaulDisabled, SetBloomOverhaulEnabled, GetBloomOverhaulEnabled);
            m_VignetteToggle = new TwoButtonsToggle(MyModdedObject, 6, 5, SetVignetteDisabled, SetVignetteEnabled, GetVignetteEnabled);
            m_ChromaticAberrationToggle = new TwoButtonsToggle(MyModdedObject, 8, 7, SetCADisabled, SetCAEnabled, GetCAEnabled);

            m_CameraRollingToggle = new TwoButtonsToggle(MyModdedObject, 10, 9, SetCRDisabled, SetCREnabled, GetCREnabled);

            m_BrightAmpColPreset = MyModdedObject.GetObject<Button>(13);
            m_BrightAmpColPreset.onClick.AddListener(SetBrightAmpColPreset);
            m_DarkAmpColPreset = MyModdedObject.GetObject<Button>(12);
            m_DarkAmpColPreset.onClick.AddListener(SetDarkAmpColPreset);
            m_DefaultAmpColPreset = MyModdedObject.GetObject<Button>(11);
            m_DefaultAmpColPreset.onClick.AddListener(SetDefaultAmpColPreset);

            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsFirstUseSetupUIEnabled || FirstUseSetupUI.HasSetTheModUp || !GameModeManager.IsOnTitleScreen())
                return;

            DelegateScheduler.Instance.Schedule(Show, 1f);
        }

        public void Show()
        {
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

        public bool GetSSAOEnabled()
        {
            return OverhaulGraphicsController.AOEnabled;
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

        #region Vignette

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
    }
}
