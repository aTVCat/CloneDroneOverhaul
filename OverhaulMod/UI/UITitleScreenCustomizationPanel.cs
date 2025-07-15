using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UITitleScreenCustomizationPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnPreviewButtonClicked))]
        [UIElement("PreviewButton")]
        private readonly Button m_previewButton;

        [UIElement("MusicDropdown")]
        private readonly Dropdown m_musicDropdown;

        [UIElementAction(nameof(OnVolumeSliderChanged), true)]
        [UIElement("VolumeSlider")]
        private readonly Slider m_volumeSlider;

        [UIElement("StaticBackgroundConfig", typeof(UIElementTitleScreenBackgroundConfig))]
        private readonly UIElementTitleScreenBackgroundConfig m_staticBgConfig;

        [UIElement("LoadingLevelBG", false)]
        private readonly GameObject m_loadingLevelBg;

        [UIElement("Panel", true)]
        private readonly GameObject m_panel;

        [UIElement("LockedOverlay", false)]
        private readonly GameObject m_lockedOverlay;

        private bool m_isPreviewing;

        public override bool hideTitleScreen => true;

        public override bool enableUIOverLogoMode => true;

        protected override void OnInitialized()
        {
            m_musicDropdown.options = TitleScreenCustomizationManager.Instance.GetMusicTracks();
            m_musicDropdown.value = TitleScreenCustomizationManager.MusicTrackIndex;
            m_musicDropdown.onValueChanged.AddListener(onMusicTrackDropdownChanged);

            m_staticBgConfig.refreshWhenEdited = true;
            m_staticBgConfig.levelIsLoadingBG = m_loadingLevelBg;

            if (TitleScreenCustomizationManager.IntroduceCustomization)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.INTRODUCE_TITLE_SCREEN_CUSTOMIZATION, false);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            stopPreviewing();
        }

        public override void Update()
        {
            base.Update();
            if (m_isPreviewing && Input.anyKey)
            {
                stopPreviewing();
            }
        }

        public override void Show()
        {
            base.Show();
            m_volumeSlider.value = SettingsManager.Instance.GetMusicVolume();
            m_lockedOverlay.SetActive(TitleScreenCustomizationManager.Instance.ShouldLockUserCustomization());
        }

        private void startPreviewing()
        {
            m_isPreviewing = true;
            m_panel.SetActive(false);
        }

        private void stopPreviewing()
        {
            m_isPreviewing = false;
            m_panel.SetActive(true);
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
    }
}
