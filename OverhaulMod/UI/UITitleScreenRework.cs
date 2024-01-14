using InternalModBot;
using OverhaulMod.Content;
using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UITitleScreenRework : OverhaulUIBehaviour
    {
        [UIElement("ButtonsBG")]
        private readonly GameObject m_container;

        [UIElement("NewsWarnIcon", false)]
        private readonly GameObject m_newsWarnIcon;

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

        [UIElementAction(nameof(OnNewsButtonClicked))]
        [UIElement("NewsButton")]
        private readonly Button m_newsButton;

        [UIElementAction(nameof(OnExcContentMenuButtonClicked))]
        [UIElement("ExclusiveContentMenuButton")]
        private readonly Button m_excContentMenuButton;

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

        [UIElementAction(nameof(OnLevelDescriptionsEditorButtonClicked))]
        [UIElement("LevelDescriptionsEditorButton")]
        private readonly Button m_levelDescriptionsEditorButton;

        [UIElementAction(nameof(OnPersonalizationEditorButtonClicked))]
        [UIElement("PersonalizationEditorButton")]
        private readonly Button m_personalizationEditorButton;

        [UIElementAction(nameof(OnContentButtonClicked))]
        [UIElement("ContentButton")]
        private readonly Button m_contentButton;

        [UIElementAction(nameof(OnUpdatesButtonClicked))]
        [UIElement("UpdatesButton")]
        private readonly Button m_updatesButton;

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

        private bool m_hasAddedEventListeners;

        public bool enableRework
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            if (titleScreenUI)
            {
                CanvasGroup group = titleScreenUI.RootButtonsContainerBG.AddComponent<CanvasGroup>();
                group.blocksRaycasts = true;
                m_canvasGroup = group;
                m_legacyContainer = group.gameObject;
                m_titleScreenUI = titleScreenUI;
                HideLegacyUI();

                ModCore.ModStateChanged += onModStateChanged;
                m_hasAddedEventListeners = true;
            }
        }

        public override void Update()
        {
            m_container.SetActive(enableRework && m_legacyContainer.activeInHierarchy);
            m_excContentMenuButton.interactable = ExclusiveContentManager.Instance.HasDownloadedContent();
        }

        public override void OnDestroy()
        {
            if (m_hasAddedEventListeners)
                ModCore.ModStateChanged -= onModStateChanged;
        }

        private void onModStateChanged(bool enabled)
        {
            if (enabled)
                HideLegacyUI();
            else
                ShowLegacyUI();
        }

        public void HideLegacyUI()
        {
            CanvasGroup group = m_canvasGroup;
            if (group)
            {
                group.alpha = 0f;
                group.interactable = false;
                enableRework = true;
            }
        }

        public void ShowLegacyUI()
        {
            CanvasGroup group = m_canvasGroup;
            if (group)
            {
                group.alpha = 1f;
                group.interactable = true;
                enableRework = false;
            }
        }

        private IEnumerator levelEditorTransitionCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.25f);
            m_titleScreenUI.OnLevelEditorButtonClicked();
            yield return new WaitForSecondsRealtime(1f);
            TransitionManager.Instance.EndTransition();
            yield break;
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
            ModUIUtils.MessagePopupNotImplemented();
        }

        public void OnNewsButtonClicked()
        {
            ModUIConstants.ShowNewsPanel();
        }

        public void OnFeedbackButtonClicked()
        {
            ModUIConstants.ShowFeedbackUIRework();
        }

        public void OnHubButtonClicked()
        {
            ModUIConstants.ShowCommunityHub();
        }

        public void OnLevelDescriptionsEditorButtonClicked()
        {
            ModUIConstants.ShowLevelDescriptionListEditor();
        }

        public void OnPersonalizationEditorButtonClicked()
        {
            PersonalizationEditorManager.Instance.StartEditorGameMode();
        }

        public void OnExcContentMenuButtonClicked()
        {
            ModUIConstants.ShowExclusiveContentMenu();
        }

        public void OnContentButtonClicked()
        {
            ModUIConstants.ShowContentDownloadWindow();
        }

        public void OnUpdatesButtonClicked()
        {
            ModUIConstants.ShowUpdatesWindow();
        }

        public void OnAdvancementsButtonClicked()
        {
            ModUIConstants.ShowAdvancementsMenuRework();
        }

        public void OnWorkshopBrowserButtonClicked()
        {
            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.WorkshopBrowserRework))
            {
                ModCache.titleScreenUI.OnWorkshopBrowserButtonClicked();
                return;
            }
            ModUIConstants.ShowWorkshopBrowserRework();
        }

        public void OnLevelEditorButtonClicked()
        {
            TransitionManager.Instance.DoNonSceneTransition(levelEditorTransitionCoroutine());
        }

        public void OnOptionsButtonClicked()
        {
            ModUIConstants.ShowSettingsMenuRework();
        }

        public void OnCreditsButtonClicked()
        {
            ModUIConstants.ShowInformationSelectMenu();
        }

        public void OnExitButtonClicked()
        {
            Application.Quit();
        }
    }
}
