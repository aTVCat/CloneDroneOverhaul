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

        [UIElement("StaticBackgroundConfig", typeof(UIElementTitleScreenBackgroundConfig))]
        private readonly UIElementTitleScreenBackgroundConfig m_staticBgConfig;

        public override bool hideTitleScreen => true;

        public override bool enableUIOverLogoMode => true;

        protected override void OnInitialized()
        {
            m_musicDropdown.options = TitleScreenCustomizationManager.Instance.GetMusicTracks();
            m_musicDropdown.value = TitleScreenCustomizationManager.MusicTrackIndex;
            m_musicDropdown.onValueChanged.AddListener(onMusicTrackDropdownChanged);

            m_staticBgConfig.refreshWhenEdited = true;
            m_staticBgConfig.backgroundInfo = TitleScreenCustomizationManager.Instance.GetStaticBackgroundInfo();
        }

        private void onMusicTrackDropdownChanged(int index)
        {
            ModSettingsManager.Instance.SetSettingValueFromUI(ModSettingConstants.TITLE_SCREEN_MUSIC_TRACK_INDEX, index);
            TitleScreenCustomizationManager.Instance.RefreshMusicTrack();
        }
    }
}
