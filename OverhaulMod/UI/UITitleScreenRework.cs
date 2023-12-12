using InternalModBot;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UITitleScreenRework : OverhaulUIBehaviour
    {
        [UIElement("ButtonsBG")]
        private readonly GameObject m_container;

        [UIElementAction(nameof(OnPlaySinglePlayerButtonClicked))]
        [UIElement("PlaySingleplayerButton")]
        private readonly Button m_playSinglePlayerButton;

        [UIElementAction(nameof(OnPlayMultiPlayerButtonClicked))]
        [UIElement("PlayMultiplayerButton")]
        private readonly Button m_playMultiPlayerButton;

        [UIElementAction(nameof(OnModsButtonClicked))]
        [UIElement("ModsButton")]
        private readonly Button m_modsButton;

        [UIElementAction(nameof(OnInfoButtonClicked))]
        [UIElement("InfoButton")]
        private readonly Button m_infoButton;

        [UIElementAction(nameof(OnFeedbackButtonClicked))]
        [UIElement("FeedbackButton")]
        private readonly Button m_feedbackButton;

        [UIElementAction(nameof(OnChangelogButtonClicked))]
        [UIElement("PatchNotesButton")]
        private readonly Button m_changelogButton;

        [UIElementAction(nameof(OnOptionsButtonClicked))]
        [UIElement("OptionsButton")]
        private readonly Button m_optionsButton;

        [UIElementAction(nameof(OnAdvancementsButtonClicked))]
        [UIElement("AchievementsButton")]
        private readonly Button m_advancementsButton;

        [UIElementAction(nameof(OnWorkshopBrowserButtonClicked))]
        [UIElement("WorkshopBrowserButton")]
        private readonly Button m_workshopBrowserButton;

        [UIElementAction(nameof(OnHubButtonClicked))]
        [UIElement("HubButton")]
        private readonly Button m_hubButton;

        [UIElementAction(nameof(OnLevelEditorButtonClicked))]
        [UIElement("LevelEditorButton")]
        private readonly Button m_levelEditorButton;

        [UIElementAction(nameof(OnCreditsButtonClicked))]
        [UIElement("CreditsButton")]
        private readonly Button m_creditsButton;

        [UIElementAction(nameof(OnExitButtonClicked))]
        [UIElement("ExitButton")]
        private readonly Button m_quitButton;

        private TitleScreenUI m_titleScreenUI;
        private CanvasGroup m_canvasGroup;
        private GameObject m_legacyContainer;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            if (titleScreenUI)
            {
                CanvasGroup group = titleScreenUI.RootButtonsContainerBG.AddComponent<CanvasGroup>();
                group.alpha = 0f;
                group.interactable = false;
                m_canvasGroup = group;
                m_legacyContainer = group.gameObject;
                m_titleScreenUI = titleScreenUI;
            }
        }

        public override void Update()
        {
            m_container.SetActive(m_legacyContainer.activeInHierarchy);
        }

        public void OnPlaySinglePlayerButtonClicked()
        {
            m_titleScreenUI.OnPlaySingleplayerButtonClicked();
        }

        public void OnPlayMultiPlayerButtonClicked()
        {
            m_titleScreenUI.OnMultiplayerButtonClicked();
        }

        public void OnModsButtonClicked()
        {
            ModsPanelManager.Instance.openModsMenu();
        }

        public void OnInfoButtonClicked()
        {
            ModUIUtility.MessagePopupOK("This button doesn't work now!", "Wait for an update which will add functionality to this button...");
        }

        public void OnChangelogButtonClicked()
        {
            ModUIUtility.MessagePopupOK("This button doesn't work now!", "Wait for an update which will add functionality to this button...");
        }

        public void OnFeedbackButtonClicked()
        {
            ModUIConstants.ShowFeedbackUIRework();
        }

        public void OnHubButtonClicked()
        {
            ModUIConstants.ShowCommunityHub();
        }

        public void OnAdvancementsButtonClicked()
        {
            ModUIConstants.ShowAdvancementsMenuRework();
        }

        public void OnWorkshopBrowserButtonClicked()
        {
            ModUIConstants.ShowWorkshopBrowserRework();
        }

        public void OnLevelEditorButtonClicked()
        {
            m_titleScreenUI.OnLevelEditorButtonClicked();
        }

        public void OnOptionsButtonClicked()
        {
            ModUIConstants.ShowSettingsMenuRework();
        }

        public void OnCreditsButtonClicked()
        {
            m_titleScreenUI.OnCreditsButtonClicked();
        }

        public void OnExitButtonClicked()
        {
            Application.Quit();
        }
    }
}
