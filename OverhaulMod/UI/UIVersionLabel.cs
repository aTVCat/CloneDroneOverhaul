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

        public static UIVersionLabel instance
        {
            get;
            set;
        }

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

        public bool forceHide
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            instance = this;

            bool debug = ModBuildInfo.debug;
            string versionString = debug ? ModBuildInfo.fullVersionString.Replace('/', '.') : ModBuildInfo.versionString;

            m_versionText.text = "overhaul mod v" + versionString;
            m_debugIcon.SetActive(debug);
            m_gameplayVersionText.text = "overhaul v" + versionString;
            m_gameplayDebugIcon.SetActive(debug);

            ModCache.titleScreenUI.VersionLabel.gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }

        public override void Update()
        {
            if (Time.frameCount % 10 != 0)
                return;

            bool show = !forceHide && showWatermark;
            bool titleScreen = showFullWatermark;
            m_watermark.SetActive(show && ModCache.titleScreenUI.RootButtonsContainerBG.activeInHierarchy && titleScreen);
            m_gameplayWatermark.SetActive(show && !titleScreen);
        }

        public void OnOtherModsButtonClicked()
        {
            ModUIConstants.ShowOtherModsMenu();
        }
    }
}
