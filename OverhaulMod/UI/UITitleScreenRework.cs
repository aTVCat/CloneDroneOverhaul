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

        [UIElement("DebugButtons", false)]
        private readonly GameObject m_debugButtonsObject;

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

        [UIElementAction(nameof(OnSetupButtonClicked))]
        [UIElement("SetupScreenButton")]
        private readonly Button m_setupButton;

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

        [UIElementAction(nameof(OnModBotLogInButtonClicked))]
        [UIElement("ModBotLogInButton", false)]
        private readonly Button m_modBotLogInButton;

        [UIElementAction(nameof(OnOldUIButtonClicked))]
        [UIElement("OldUIButton")]
        private readonly Button m_oldUIButton;

        [UIElementAction(nameof(OnNewUIButtonClicked))]
        [UIElement("NewUIButton")]
        private readonly Button m_newUIButton;

        [UIElement("OtherLayers")]
        private readonly GameObject m_otherLayersObject;

        [UIElement("TutorialLayer", false)]
        private readonly GameObject m_tutorialLayerObject;

        [UIElement("ModBotLogonText")]
        private readonly Text m_modBotLogonText;

        [UIElement("ErrorMessage", typeof(UIElementMultiplayerMessageBox), false)]
        public UIElementMultiplayerMessageBox ErrorMessage;

        [UIElement("ViewMultiplayerErrorButton", typeof(UIElementMultiplayerMessageButton))]
        public UIElementMultiplayerMessageButton ErrorMessageButton;

        [UIElement("ContentButton", typeof(UIElementTitleScreenContentButton))]
        public UIElementTitleScreenContentButton ContentButtonController;

        [UIElement("NewsButton", typeof(UIElementTitleScreenButtonWithWarn))]
        public UIElementTitleScreenButtonWithWarn NewsButtonWarnController;

        [UIElement("UpdatesButton", typeof(UIElementTitleScreenButtonWithWarn))]
        public UIElementTitleScreenButtonWithWarn UpdatesButtonWarnController;

        private TitleScreenUI m_titleScreenUI;
        private CanvasGroup m_canvasGroup;
        private GameObject m_legacyContainer;

        private RectTransform m_socialButtonContainer;
        private RectTransform m_socialButtonPopoutHolder;

        private Vector2 m_initialSocialButtonContainerPosition, m_newSocialButtonContainerPosition;
        private Vector2 m_initialSocialButtonPopoutHolderPosition, m_newSocialButtonPopoutHolderPosition;

        private bool m_enableRework;
        public bool enableRework
        {
            get
            {
                return m_enableRework;
            }
            set
            {
                m_enableRework = value;

                CanvasGroup group = m_canvasGroup;
                if (group)
                {
                    group.alpha = value ? 0f : 1f;
                    group.interactable = !value;
                }

                if (m_socialButtonContainer && m_socialButtonPopoutHolder)
                {
                    m_socialButtonContainer.anchoredPosition = value ? m_newSocialButtonContainerPosition : m_initialSocialButtonContainerPosition;
                    m_socialButtonPopoutHolder.anchoredPosition = value ? m_newSocialButtonPopoutHolderPosition : m_initialSocialButtonPopoutHolderPosition;
                }
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            m_modBotLogonText.text = "Not logged in";
            NewsButtonWarnController.isNewsButton = true;
            UpdatesButtonWarnController.isUpdatesButton = true;
            m_debugButtonsObject.SetActive(ModBuildInfo.debug);

            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            if (titleScreenUI)
            {
                m_titleScreenUI = titleScreenUI;
                if (titleScreenUI.RootButtonsContainerBG)
                {
                    CanvasGroup group = titleScreenUI.RootButtonsContainerBG.GetComponent<CanvasGroup>() ?? titleScreenUI.RootButtonsContainerBG.AddComponent<CanvasGroup>();
                    group.blocksRaycasts = true;
                    m_canvasGroup = group;
                    m_legacyContainer = group.gameObject;
                }

                Transform socialButtons = titleScreenUI.SocialButtonPanel?.transform;
                if (socialButtons)
                {
                    RectTransform socialButtonContainer = TransformUtils.FindChildRecursive(socialButtons, "VerticalSocialButtons") as RectTransform;
                    RectTransform socialButtonPopoutHolder = TransformUtils.FindChildRecursive(socialButtons, "PopoutHolder") as RectTransform;
                    if (socialButtonContainer && socialButtonPopoutHolder)
                    {
                        m_socialButtonContainer = socialButtonContainer;
                        m_initialSocialButtonContainerPosition = socialButtonContainer.anchoredPosition;
                        m_newSocialButtonContainerPosition = socialButtonContainer.anchoredPosition + (Vector2.up * 30f);
                        m_socialButtonPopoutHolder = socialButtonPopoutHolder;
                        m_initialSocialButtonPopoutHolderPosition = socialButtonPopoutHolder.anchoredPosition;
                        m_newSocialButtonPopoutHolderPosition = socialButtonPopoutHolder.anchoredPosition + (Vector2.up * 30f);
                    }
                }
            }

            enableRework = true;
        }

        public override void Update()
        {
            bool reworkEnabled = enableRework;
            bool shouldBeActive = m_legacyContainer.activeInHierarchy;
            bool flag = reworkEnabled && shouldBeActive;
            m_container.SetActive(flag);
            m_otherLayersObject.SetActive(flag);

            if (Time.frameCount % 20 == 0)
            {
                //m_excContentMenuButton.interactable = ExclusiveContentManager.Instance.HasDownloadedContent();
                m_tutorialLayerObject.SetActive(TitleScreenCustomizationManager.IntroduceCustomization);

                string userName = ModBotSignInUI._userName;
                if (!userName.IsNullOrEmpty())
                {
                    m_modBotLogonText.text = $"Logged as: {userName.AddColor(Color.white)}";
                    m_modBotLogonText.enabled = true;
                    m_modBotLogInButton.gameObject.SetActive(false);
                }
                else
                {
                    m_modBotLogonText.enabled = false;
                    m_modBotLogInButton.gameObject.SetActive(true);
                }
            }

            m_oldUIButton.gameObject.SetActive(flag);
            m_newUIButton.gameObject.SetActive(!reworkEnabled && shouldBeActive);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            enableRework = false;
        }

        public void SetMultiplayerButtonActive(bool value)
        {
            m_playMultiPlayerButton.interactable = value;
            m_playExpMultiPlayerButton.interactable = value;
        }

        private IEnumerator levelEditorTransitionCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.25f);
            while(LevelManager.Instance.IsSpawningCurrentLevel())
                yield return null;

            yield return null;
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

        public void OnModBotLogInButtonClicked()
        {
            ModBotUIRoot.Instance.ModBotSignInUI.OpenSignInForm();
        }

        public void OnViewMultiplayerErrorButtonClicked()
        {
            ErrorMessage.ToggleVisibility();
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
            if (!ModUIManager.ShowAdvancementsMenuRework)
            {
                ModCache.titleScreenUI.OnAchievementsButtonClicked();
                return;
            }
            ModUIConstants.ShowAdvancementsMenuRework();
        }

        public void OnWorkshopBrowserButtonClicked()
        {
            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.WorkshopBrowserRework) || !ModUIManager.ShowWorkshopBrowserRework)
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
            if (!ModUIManager.ShowSettingsMenuRework)
            {
                ModCache.titleScreenUI.OnOptionsButtonClicked();
                return;
            }
            ModUIConstants.ShowSettingsMenuRework(false);
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

        public void OnSetupButtonClicked()
        {
            ModUIConstants.ShowSettingsMenuRework(true);
        }

        public void OnOldUIButtonClicked()
        {
            enableRework = false;
        }

        public void OnNewUIButtonClicked()
        {
            enableRework = true;
        }
    }
}
