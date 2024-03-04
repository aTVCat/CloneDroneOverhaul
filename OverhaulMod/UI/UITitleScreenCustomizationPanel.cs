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

        [UIElement("MusicDropdown")]
        private readonly Dropdown m_musicDropdown;

        [UIElementAction(nameof(OnVolumeSliderChanged), true)]
        [UIElement("VolumeSlider")]
        private readonly Slider m_volumeSlider;

        [UIElement("StaticBackgroundConfig", typeof(UIElementTitleScreenBackgroundConfig))]
        private readonly UIElementTitleScreenBackgroundConfig m_staticBgConfig;

        [UIElement("LoadingLevelBG", false)]
        private readonly GameObject m_loadingLevelBg;

        public override bool hideTitleScreen => true;

        public override bool enableUIOverLogoMode => true;

        protected override void OnInitialized()
        {
            m_musicDropdown.options = TitleScreenCustomizationManager.Instance.GetMusicTracks();
            m_musicDropdown.value = TitleScreenCustomizationManager.MusicTrackIndex;
            m_musicDropdown.onValueChanged.AddListener(onMusicTrackDropdownChanged);

            m_staticBgConfig.refreshWhenEdited = true;
            m_staticBgConfig.backgroundInfo = TitleScreenCustomizationManager.Instance.GetStaticBackgroundInfo();
            m_staticBgConfig.levelIsLoadingBG = m_loadingLevelBg;

            if (TitleScreenCustomizationManager.IntroduceCustomization)
            {
                ModSettingsManager.SetBoolValue(ModSettingsConstants.INTRODUCE_TITLE_SCREEN_CUSTOMIZATION, false);
            }
        }

        public override void Show()
        {
            base.Show();
            m_volumeSlider.value = SettingsManager.Instance.GetMusicVolume();
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
