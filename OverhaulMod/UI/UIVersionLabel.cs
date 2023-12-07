using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIVersionLabel : OverhaulUIBehaviour
    {
        [UIElement("NewVesionLabel_TitleScreen")]
        private readonly GameObject m_Watermark;

        [UIElement("DebugLabel_TitleScreen")]
        private readonly GameObject m_DebugIcon;

        [UIElement("Watermark_TitleScreen")]
        private readonly Text m_VersionText;


        [UIElement("NewVesionLabel_Gameplay")]
        private readonly GameObject m_GameplayWatermark;

        [UIElement("DebugLabel_Gameplay")]
        private readonly GameObject m_GameplayDebugIcon;

        [UIElement("Watermark_Gameplay")]
        private readonly Text m_GameplayVersionText;


        [UIElement("TopButtons")]
        private readonly GameObject m_TopButtons;

        [UIElementAction(nameof(OnPatchNotesButtonClicked))]
        [UIElement("PatchNotesButton")]
        private readonly Button m_PatchNotesButton;

        [UIElementAction(nameof(OnOtherModsButtonClicked))]
        [UIElement("OtherModsButton")]
        private readonly Button m_OtherModsButton;

        public bool showWatermark
        {
            get
            {
                return !ModCache.photoManager.IsInPhotoMode();
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
            m_VersionText.text = "overhaul mod " + versionString;
            m_DebugIcon.SetActive(ModBuildInfo.debug);
            m_GameplayVersionText.text = "overhaul " + versionString;
            m_GameplayDebugIcon.SetActive(ModBuildInfo.debug);

            ModCache.gameUIRoot.TitleScreenUI.VersionLabel.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Time.frameCount % 15 != 0)
                return;

            bool show = showWatermark;
            bool showTop = ModCache.titleScreenRootButtonsBG.activeInHierarchy;
            bool titleScreen = showFullWatermark;

            m_Watermark.SetActive(show && titleScreen);
            m_GameplayWatermark.SetActive(show && !titleScreen);
            m_TopButtons.SetActive(titleScreen && showTop);
        }

        public void OnPatchNotesButtonClicked()
        {

        }

        public void OnOtherModsButtonClicked()
        {
            UIConstants.ShowOtherModsMenu();
        }
    }
}
