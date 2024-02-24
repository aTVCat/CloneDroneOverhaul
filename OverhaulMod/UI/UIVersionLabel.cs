using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIVersionLabel : OverhaulUIBehaviour
    {
        [ModSetting(ModSettingsConstants.SHOW_VERSION_LABEL, true)]
        public static bool ShowLabel;

        [UIElement("NewVersionLabel_TitleScreen")]
        private readonly GameObject m_watermark;

        [UIElement("DebugLabel_TitleScreen")]
        private readonly GameObject m_debugIcon;

        [UIElement("Watermark_TitleScreen")]
        private readonly Text m_versionText;

        [UIElement("NewVersionLabel_Gameplay")]
        private readonly GameObject m_gameplayWatermark;

        [UIElement("DebugLabel_Gameplay")]
        private readonly GameObject m_gameplayDebugIcon;

        [UIElement("Watermark_Gameplay")]
        private readonly Text m_gameplayVersionText;

        public bool showWatermark
        {
            get
            {
                return ShowLabel && !ModCache.photoManager.IsInPhotoMode();
            }
        }

        public bool showFullWatermark
        {
            get
            {
                return GameModeManager.IsOnTitleScreen();
            }
        }

        protected override void OnInitialized()
        {
            string versionString = ModBuildInfo.fullVersionString;
            if (string.IsNullOrEmpty(versionString))
                return;

            versionString = versionString.Replace('/', '.');

            m_versionText.text = "overhaul mod " + versionString;
            m_debugIcon.SetActive(ModBuildInfo.debug);
            m_gameplayVersionText.text = "overhaul " + versionString;
            m_gameplayDebugIcon.SetActive(ModBuildInfo.debug);

            ModCache.gameUIRoot.TitleScreenUI.VersionLabel.gameObject.SetActive(false);
        }

        public override void Update()
        {
            if (Time.frameCount % 15 != 0)
                return;

            bool show = showWatermark;
            bool titleScreen = showFullWatermark;
            m_watermark.SetActive(show && titleScreen);
            m_gameplayWatermark.SetActive(show && !titleScreen);
        }

        public void OnOtherModsButtonClicked()
        {
            ModUIConstants.ShowOtherModsMenu();
        }
    }
}
