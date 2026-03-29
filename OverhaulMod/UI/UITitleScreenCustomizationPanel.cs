using OverhaulMod.Engine;
using OverhaulMod.Patches.Behaviours;
using OverhaulMod.Utils;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UITitleScreenCustomizationPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnPreviewButtonClicked))]
        [UIElement("PreviewButton")]
        private readonly Button _previewButton;

        [UIElement("MusicDropdown")]
        private readonly Dropdown _musicDropdown;

        [UIElementAction(nameof(OnVolumeSliderChanged), true)]
        [UIElement("VolumeSlider")]
        private readonly Slider _volumeSlider;

        [UIElement("StaticBackgroundConfig", typeof(UIElementTitleScreenBackgroundConfig))]
        private readonly UIElementTitleScreenBackgroundConfig _staticBgConfig;

        [UIElement("LoadingLevelBG", false)]
        private readonly GameObject _loadingLevelBg;

        [UIElement("Panel", true)]
        private readonly GameObject _panel;

        [UIElement("LockedOverlay", false)]
        private readonly GameObject _lockedOverlay;

        [UIElementAction(nameof(OnLogoParticlesToggled))]
        [UIElement("LogoParticlesToggle")]
        private readonly Toggle _logoParticlesToggle;

        [UIElementAction(nameof(OnSocialMediaPopupsToggled))]
        [UIElement("SocialMediaPopupsToggle")]
        private readonly Toggle _socialMediaPopupsToggle;

        [UIElementAction(nameof(OnSocialMediaPopupsToggled))]
        [UIElement("SocialMediaButtonsToggle")]
        private readonly Toggle _socialMediaButtonsToggle;

        [UIElementAction(nameof(OnModBotAccountInfoToggled))]
        [UIElement("ShowModBotAccountInfoToggle")]
        private readonly Toggle _showModBotAccountInfoToggle;

        [UIElementAction(nameof(OnBGFadePowerChanged))]
        [UIElement("FadePowerSlider")]
        private readonly Slider _bgFadePowerSlider;

        [UIElementAction(nameof(OnPanelPositionChanged))]
        [UIElement("PanelSideDropdown")]
        private readonly Dropdown _panelPositionDropdown;

        private bool _isPreviewing;

        public override bool HideTitleScreen => true;

        public override bool EnableUIOverLogoMode => true;

        protected override void OnInitialized()
        {
            _musicDropdown.options = TitleScreenCustomizationManager.Instance.GetMusicTracks();
            _musicDropdown.value = TitleScreenCustomizationManager.MusicTrackIndex;
            _musicDropdown.onValueChanged.AddListener(onMusicTrackDropdownChanged);

            _staticBgConfig.refreshWhenEdited = true;
            _staticBgConfig.levelIsLoadingBG = _loadingLevelBg;

            _logoParticlesToggle.isOn = TitleScreenCustomizationManager.ShowLogoFireParticles;
            _socialMediaPopupsToggle.isOn = TitleScreenCustomizationManager.ShowSocialMediaPopups;
            _socialMediaButtonsToggle.isOn = TitleScreenCustomizationManager.ShowSocialMediaButtons;
            _showModBotAccountInfoToggle.isOn = TitleScreenCustomizationManager.ShowModBotAccountInfo;

            _bgFadePowerSlider.value = TitleScreenCustomizationManager.BackgroundFadePower;
            _panelPositionDropdown.value = (int)TitleScreenCustomizationManager.PanelPosition;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            stopPreviewing();
        }

        public override void Update()
        {
            base.Update();
            if (_isPreviewing && Input.anyKey)
            {
                stopPreviewing();
            }
        }

        public override void Show()
        {
            base.Show();
            _volumeSlider.value = SettingsManager.Instance.GetMusicVolume();
            _lockedOverlay.SetActive(TitleScreenCustomizationManager.Instance.ShouldLockUserCustomization());
        }

        public override void Hide()
        {
            base.Hide();
            ModSettingsDataManager.Instance.Save();
        }

        private void startPreviewing()
        {
            _isPreviewing = true;
            _panel.SetActive(false);
        }

        private void stopPreviewing()
        {
            _isPreviewing = false;
            _panel.SetActive(true);
        }

        public void OnPreviewButtonClicked()
        {
            startPreviewing();
        }

        private void onMusicTrackDropdownChanged(int index)
        {
            ModSettingsManager.Instance.SetSettingValueFromUI(ModSettingsConstants.TITLE_SCREEN_MUSIC_TRACK_INDEX, index);
            TitleScreenCustomizationManager.Instance.RefreshMusicTrack();
        }

        public void OnVolumeSliderChanged(float value)
        {
            SettingsManager.Instance.SetMusicVolume(value);
        }

        public void OnLogoParticlesToggled(bool value)
        {
            ModSettingsManager.Instance.SetSettingValueFromUI(ModSettingsConstants.CLONE_DRONE_LOGO_FIRE, value);
        }

        public void OnSocialMediaPopupsToggled(bool value)
        {
            ModSettingsManager.Instance.SetSettingValueFromUI(ModSettingsConstants.TITLE_SCREEN_SOCIAL_MEDIA_POPUPS, value);
        }

        public void OnSocialMediaButtonsToggled(bool value)
        {
            ModSettingsManager.Instance.SetSettingValueFromUI(ModSettingsConstants.TITLE_SCREEN_SOCIAL_MEDIA_BUTTONS, value);
        }

        public void OnModBotAccountInfoToggled(bool value)
        {
            ModSettingsManager.Instance.SetSettingValueFromUI(ModSettingsConstants.TITLE_SCREEN_SHOW_MODBOT_ACCOUNT_INFO, value);
        }

        public void OnBGFadePowerChanged(float value)
        {
            ModSettingsManager.Instance.SetSettingValueFromUI(ModSettingsConstants.TITLE_SCREEN_BACKGROUND_FADE_POWER, value);
        }

        public void OnPanelPositionChanged(int value)
        {
            ModSettingsManager.Instance.SetSettingValueFromUI(ModSettingsConstants.TITLE_SCREEN_PANEL_POSITION, value);
        }
    }
}