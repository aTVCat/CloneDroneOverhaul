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

        [UIElementAction(nameof(OnPlayExpMultiPlayerButtonClicked))]
        [UIElement("ExpPlayMultiplayerButton")]
        private readonly Button m_playExpMultiPlayerButton;

        [UIElementAction(nameof(OnViewMultiplayerErrorButtonClicked))]
        [UIElement("ViewMultiplayerErrorButton")]
        private readonly Button m_viewMultiplayerErrorButton;

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

        [UIElementAction(nameof(OnLocalizationEditorButtonClicked))]
        [UIElement("LocalizationEditorButton")]
        private readonly Button m_localizationEditorButton;

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

        [UIElementAction(nameof(OnCustomizeButtonClicked))]
        [UIElement("CustomizeButton")]
        private readonly Button m_customizeButton;

        [UIElement("OtherLayers")]
        private readonly GameObject m_otherLayersObject;

        [UIElement("ErrorMessage", typeof(UIElementMultiplayerMessageBox), false)]
        public UIElementMultiplayerMessageBox ErrorMessage;

        [UIElement("ViewMultiplayerErrorButton", typeof(UIElementMultiplayerMessageButton))]
        public UIElementMultiplayerMessageButton ErrorMessageButton;

        private TitleScreenUI m_titleScreenUI;
        private CanvasGroup m_canvasGroup;
        private GameObject m_legacyContainer;

        private RectTransform m_socialButtonContainer;
        private RectTransform m_socialButtonPopouts;

        private Vector2 m_initialSocialButtonContainerPosition, m_newSocialButtonContainerPosition;
        private Vector2 m_initialSocialButtonPopoutsPosition, m_newSocialButtonPopoutsPosition;

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

                Transform socialButtons = titleScreenUI.SocialButtonPanel.transform;
                RectTransform socialButtonContainer = TransformUtils.FindChildRecursive(socialButtons, "VerticalSocialButtons") as RectTransform;
                RectTransform socialButtonPopouts = TransformUtils.FindChildRecursive(socialButtons, "PopoutHolder") as RectTransform;
                if (socialButtonContainer && socialButtonPopouts)
                {
                    m_socialButtonContainer = socialButtonContainer;
                    m_initialSocialButtonContainerPosition = socialButtonContainer.anchoredPosition;
                    m_newSocialButtonContainerPosition = socialButtonContainer.anchoredPosition + (Vector2.up * 30f);
                    m_socialButtonPopouts = socialButtonPopouts;
                    m_initialSocialButtonPopoutsPosition = socialButtonPopouts.anchoredPosition;
                    m_newSocialButtonPopoutsPosition = socialButtonPopouts.anchoredPosition + (Vector2.up * 30f);
                }

                HideLegacyUI();

                ModCore.ModStateChanged += onModStateChanged;
                m_hasAddedEventListeners = true;
            }
        }

        public override void Update()
        {
            bool shouldBeActive = enableRework && m_legacyContainer.activeInHierarchy;
            m_container.SetActive(shouldBeActive);
            m_otherLayersObject.SetActive(shouldBeActive);
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

            if (m_socialButtonContainer && m_socialButtonPopouts)
            {
                m_socialButtonContainer.anchoredPosition = m_newSocialButtonContainerPosition;
                m_socialButtonPopouts.anchoredPosition = m_newSocialButtonPopoutsPosition;
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

            if (m_socialButtonContainer && m_socialButtonPopouts)
            {
                m_socialButtonContainer.anchoredPosition = m_initialSocialButtonContainerPosition;
                m_socialButtonPopouts.anchoredPosition = m_initialSocialButtonPopoutsPosition;
            }
        }

        public void SetMultiplayerButtonActive(bool value)
        {
            m_playMultiPlayerButton.interactable = value;
            m_playExpMultiPlayerButton.interactable = value;
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
            m_titleScreenUI.MultiplayerModeSelectScreen.SetMainScreenVisible(true);
        }

        public void OnPlayExpMultiPlayerButtonClicked()
        {
            ModUIConstants.ShowMultiplayerGameModeSelectScreen();
        }

        public void OnViewMultiplayerErrorButtonClicked()
        {
            ErrorMessage.Toggle();
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

        public void OnLocalizationEditorButtonClicked()
        {
            ModUIConstants.ShowLocalizationEditor();
        }

        public void OnExcContentMenuButtonClicked()
        {
            ModUIConstants.ShowExclusiveContentMenu();
        }

        public void OnContentButtonClicked()
        {
            ModUIConstants.ShowAddonsMenu();
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

        public void OnCustomizeButtonClicked()
        {
            ModUIConstants.ShowTitleScreenCustomizationPanel(base.transform);
        }
    }
}
