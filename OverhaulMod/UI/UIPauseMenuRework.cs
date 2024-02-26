using InternalModBot;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPauseMenuRework : OverhaulUIBehaviour
    {
        public static readonly bool UseMessagePopup = false;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("LegacyUIButton")]
        private readonly Button m_legacyUIButton;

        [UIElementAction(nameof(OnResumeButtonClicked))]
        [UIElement("ResumeButton")]
        private readonly Button m_resumeButton;

        [UIElementAction(nameof(OnAchievementsButtonClicked))]
        [UIElement("AchievementsButton")]
        private readonly Button m_achievementsButton;

        [UIElementAction(nameof(OnCustomizationButtonClicked))]
        [UIElement("CustomizationButton")]
        private readonly Button m_customizationButton;

        [UIElementAction(nameof(OnSettingsButtonClicked))]
        [UIElement("SettingsButton")]
        private readonly Button m_settingsButton;

        [UIElementAction(nameof(OnModsButtonClicked))]
        [UIElement("ModsButton")]
        private readonly Button m_modsButton;

        [UIElement("LogoEn")]
        private readonly GameObject m_logoEn;

        [UIElement("LogoCh")]
        private readonly GameObject m_logoCh;

        [UIElement("LogoJa")]
        private readonly GameObject m_logoJa;

        [UIElement("LogoKo")]
        private readonly GameObject m_logoKo;

        [UIElement("ExitDialogue", false)]
        private readonly GameObject m_exitDialogue;

        [UIElementAction(nameof(OnDesktopButtonClicked))]
        [UIElement("DesktopButton")]
        private readonly Button m_desktopButton;

        [UIElementAction(nameof(OnMainMenuButtonClicked))]
        [UIElement("MainMenuButton")]
        private readonly Button m_mainMenuButton;

        public override bool enableCursor
        {
            get
            {
                return true;
            }
        }

        public static bool disableOverhauledVersion { get; set; }

        public override void Show()
        {
            base.Show();
            refreshLogo();
            refreshButtons();
            TimeManager.Instance.OnGamePaused();
            _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionPress, 0f, 1f, 0f);
        }

        public override void Hide()
        {
            base.Hide();
            TimeManager.Instance.OnGameUnPaused();
            _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionBack, 0f, 1f, 0f);
        }

        private void refreshLogo()
        {
            string langId = LocalizationManager.Instance.GetCurrentLanguageCode();
            m_logoEn.SetActive(langId != "ko" && langId != "ja" && langId != "zh-CN" && langId != "zh-TW");
            m_logoCh.SetActive(langId == "zh-CN" || langId == "zh-TW");
            m_logoKo.SetActive(langId == "ko");
            m_logoJa.SetActive(langId == "ja");
        }

        private void refreshButtons()
        {
            bool customizationSupported = !GameModeManager.Is((GameMode)2500) && !GameModeManager.IsInLevelEditor();
            m_customizationButton.interactable = customizationSupported;
        }

        public void OnResumeButtonClicked()
        {
            Hide();
        }

        public void OnAchievementsButtonClicked()
        {
            ModUIConstants.ShowAdvancementsMenuRework();
        }

        public void OnCustomizationButtonClicked()
        {
            Hide();
            ModUIConstants.ShowPersonalizationItemsBrowser();
        }

        public void OnSettingsButtonClicked()
        {
            ModUIConstants.ShowSettingsMenuRework(false);
        }

        public void OnModsButtonClicked()
        {
            ModsPanelManager.Instance.openModsMenu();
        }

        public void OnMainMenuButtonClicked()
        {
            if (GameModeManager.Is((GameMode)2500) || (GameModeManager.IsInLevelEditor() && LevelEditorDataManager.Instance.CurrentLevelNeedsSaving()))
            {
                ModUIUtils.MessagePopup(true, "Exit editor?", "Make sure you have saved your progress.", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes, exit", "No", null, SceneTransitionManager.Instance.DisconnectAndExitToMainMenu, null);
                return;
            }
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }

        public void OnDesktopButtonClicked()
        {
            if (GameModeManager.Is((GameMode)2500) || (GameModeManager.IsInLevelEditor() && LevelEditorDataManager.Instance.CurrentLevelNeedsSaving()))
            {
                ModUIUtils.MessagePopup(true, "Exit editor?", "Make sure you have saved your progress.", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes, exit", "No", null, Application.Quit, null);
                return;
            }
            Application.Quit();
        }

        public void OnLegacyUIButtonClicked()
        {
            Hide();
            ModUIUtils.ShowVanillaEscMenu();
        }
    }
}
