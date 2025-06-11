using InternalModBot;
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

        [ButtonWithSound(ButtonWithSound.SoundType.Choose)]
        [UIElementAction(nameof(OnPlaySinglePlayerButtonClicked))]
        [UIElement("PlaySingleplayerButton")]
        private readonly Button m_playSinglePlayerButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose)]
        [UIElementAction(nameof(OnPlayMultiPlayerButtonClicked))]
        [UIElement("PlayMultiplayerButton")]
        private readonly Button m_playMultiPlayerButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose)]
        [UIElementAction(nameof(OnPlayExpMultiPlayerButtonClicked))]
        [UIElement("ExpPlayMultiplayerButton")]
        private readonly Button m_playExpMultiPlayerButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnViewMultiplayerErrorButtonClicked))]
        [UIElement("ViewMultiplayerErrorButton")]
        private readonly Button m_viewMultiplayerErrorButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose_NoEcho)]
        [UIElementAction(nameof(OnModsButtonClicked))]
        [UIElement("ModsButton")]
        private readonly Button m_modsButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnInfoButtonClicked))]
        [UIElement("InfoButton")]
        private readonly Button m_infoButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnFeedbackButtonClicked))]
        [UIElement("FeedbackButton")]
        private readonly Button m_feedbackButton;

        [UIElement("BottomSection", true)]
        private readonly GameObject m_bottomSection;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnExcContentMenuButtonClicked))]
        [UIElement("ExclusiveContentMenuButton")]
        private readonly Button m_excContentMenuButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnUpdatesButtonClicked))]
        [UIElement("UpdatesButton")]
        private readonly Button m_updatesButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnNewsButtonClicked))]
        [UIElement("NewsButton")]
        private readonly Button m_newsButton;

        [UIElement("NewBottomSection", false)]
        private readonly GameObject m_newBottomSection;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnExcContentMenuButtonClicked))]
        [UIElement("NewExclusiveContentMenuButton")]
        private readonly Button m_newExcContentMenuButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnUpdatesButtonClicked))]
        [UIElement("NewUpdatesButton")]
        private readonly Button m_newUpdatesButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnNewsButtonClicked))]
        [UIElement("NewNewsButton")]
        private readonly Button m_newNewsButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnOptionsButtonClicked))]
        [UIElement("OptionsButton")]
        private readonly Button m_optionsButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnAdvancementsButtonClicked))]
        [UIElement("AchievementsButton")]
        private readonly Button m_advancementsButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnWorkshopBrowserButtonClicked))]
        [UIElement("WorkshopBrowserButton")]
        private readonly Button m_workshopBrowserButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnHubButtonClicked))]
        [UIElement("HubButton")]
        private readonly Button m_hubButton;

        [UIElementAction(nameof(OnLevelDescriptionsEditorButtonClicked))]
        [UIElement("LevelDescriptionsEditorButton")]
        private readonly Button m_levelDescriptionsEditorButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose)]
        [UIElementAction(nameof(OnPersonalizationEditorButtonClicked))]
        [UIElement("PersonalizationEditorButton")]
        private readonly Button m_personalizationEditorButton;

        [UIElementAction(nameof(OnLocalizationEditorButtonClicked))]
        [UIElement("LocalizationEditorButton")]
        private readonly Button m_localizationEditorButton;

        [UIElementAction(nameof(OnSetupButtonClicked))]
        [UIElement("SetupScreenButton")]
        private readonly Button m_setupButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnContentButtonClicked))]
        [UIElement("ContentButton")]
        private readonly Button m_contentButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose)]
        [UIElementAction(nameof(OnLevelEditorButtonClicked))]
        [UIElement("LevelEditorButton")]
        private readonly Button m_levelEditorButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnCreditsButtonClicked))]
        [UIElement("CreditsButton")]
        private readonly Button m_creditsButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose_NoEcho)]
        [UIElementAction(nameof(OnExitButtonClicked))]
        [UIElement("ExitButton")]
        private readonly Button m_quitButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnCustomizeButtonClicked))]
        [UIElement("CustomizeButton")]
        private readonly Button m_customizeButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnModBotLogInButtonClicked))]
        [UIElement("ModBotLogInButton", false)]
        private readonly Button m_modBotLogInButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnSkinButtonClicked))]
        [UIElement("SkinButton")]
        private readonly Button m_skinButton;

        [UIElement("SkinNameLabel")]
        private readonly Text m_skinNameLabelButton;

        [UIElementAction(nameof(OnRobotEditorButtonClicked))]
        [UIElement("RobotEditorButton")]
        private readonly Button m_robotEditorButton;

        [UIElementAction(nameof(OnWeaponEditorButtonClicked))]
        [UIElement("WeaponEditorButton")]
        private readonly Button m_weaponEditorButton;

        [UIElementAction(nameof(OnStoryReworkButtonClicked))]
        [UIElement("StoryReworkButton")]
        private readonly Button m_storyReworkButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnBehindTheScenesButtonClicked))]
        [UIElement("BehindTheScenesButton")]
        private readonly Button m_behindTheScenesButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnDiscordServerButtonClicked))]
        [UIElement("DiscordServerButton")]
        private readonly Button m_discordServerButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnPatchNotesButtonClicked))]
        [UIElement("PatchNotesButton")]
        private readonly Button m_patchNotesButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnFeaturesButtonClicked))]
        [UIElement("FeaturesButton")]
        private readonly Button m_featuresButton;

        [UIElement("MiscElements")]
        private readonly GameObject m_miscElementsObject;

        [UIElement("Tutorial", false)]
        private readonly GameObject m_tutorialObject;

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

        [UIElement("AdvancementsProgressImage")]
        private readonly Image m_advancementsProgressImage;

        [UIElement("AdvancementsProgressText")]
        private readonly Text m_advancementsProgressText;

        [UIElement("AdvancementsProgressPercentageText")]
        private readonly Text m_advancementsProgressPercentageText;

        [UIElement("HypocrisisSkinHolder")]
        private readonly Transform m_hypocrisisSkinHolder;

        [UIElement("hcLabel", false)]
        private readonly GameObject m_skinButtonHCLabel;

        public override bool closeOnEscapeButtonPress => false;

        private TitleScreenUI m_titleScreenUI;
        private CanvasGroup m_canvasGroup;
        private GameObject m_legacyContainer;

        private RectTransform m_socialButtonContainer;
        private RectTransform m_socialButtonPopoutHolder;

        private Vector2 m_initialSocialButtonContainerPosition, m_newSocialButtonContainerPosition;
        private Vector2 m_initialSocialButtonPopoutHolderPosition, m_newSocialButtonPopoutHolderPosition;

        private bool m_hasSpawnedHypocrisisSkin;

        private bool m_mobBotUsernameAvailable;

        public bool hideVanillaTitleScreen
        {
            get
            {
                return skin != TitleScreenSkinType.Vanilla;
            }
        }

        private TitleScreenSkinType m_skin;
        public TitleScreenSkinType skin
        {
            get
            {
                return m_skin;
            }
            set
            {
                if (m_skin == value)
                    return;

                m_skin = value;

                bool nonVanilla = value != TitleScreenSkinType.Vanilla;
                bool overhaul = value == TitleScreenSkinType.Overhaul;
                bool hypocrisis = value == TitleScreenSkinType.Hypocrisis3;

                CanvasGroup group = m_canvasGroup;
                if (group)
                {
                    group.alpha = nonVanilla ? 0f : 1f;
                    group.interactable = !nonVanilla;
                }

                if (m_socialButtonContainer && m_socialButtonPopoutHolder)
                {
                    m_socialButtonContainer.anchoredPosition = overhaul ? m_newSocialButtonContainerPosition : m_initialSocialButtonContainerPosition;
                    m_socialButtonPopoutHolder.anchoredPosition = overhaul ? m_newSocialButtonPopoutHolderPosition : m_initialSocialButtonPopoutHolderPosition;
                }

                m_hypocrisisSkinHolder.gameObject.SetActive(hypocrisis);

                if (hypocrisis && !m_hasSpawnedHypocrisisSkin)
                {
                    ModUIConstants.ShowTitleScreenHypocrisisSkin(m_hypocrisisSkinHolder);
                    m_hasSpawnedHypocrisisSkin = true;
                }
            }
        }

        protected override void OnInitialized()
        {
            bool debug = ModBuildInfo.debug;

            m_modBotLogonText.text = "Not logged in";
            NewsButtonWarnController.isNewsButton = true;
            UpdatesButtonWarnController.isUpdatesButton = true;
            m_debugButtonsObject.SetActive(debug);
            m_featuresButton.gameObject.SetActive(false);

            float fraction = GameplayAchievementManager.Instance.GetFractionOfAchievementsCompleted();
            m_advancementsProgressImage.fillAmount = fraction;
            m_advancementsProgressText.text = $"{ModGameUtils.GetNumOfAchievementsCompleted()}/{ModGameUtils.GetNumOfAchievements()}";
            m_advancementsProgressPercentageText.text = $"({Mathf.FloorToInt(fraction * 100f)}%)";

            bool updateBottomSection = ModFeatures.IsEnabled(ModFeatures.FeatureType.TitleScreenModdedSectionRework);
            m_bottomSection.SetActive(!updateBottomSection);
            m_newBottomSection.SetActive(updateBottomSection);

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
                        m_newSocialButtonContainerPosition = socialButtonContainer.anchoredPosition + (Vector2.up * 65f);
                        m_socialButtonPopoutHolder = socialButtonPopoutHolder;
                        m_initialSocialButtonPopoutHolderPosition = socialButtonPopoutHolder.anchoredPosition;
                        m_newSocialButtonPopoutHolderPosition = socialButtonPopoutHolder.anchoredPosition + (Vector2.up * 65f);
                    }
                }
            }

            SetSkinAccordingToSettings();

            refreshSkinButtonLabel();

            m_mobBotUsernameAvailable = checkIfModBotUserNameIsAvailable();
        }

        public override void Update()
        {
            bool reworkEnabled = skin == TitleScreenSkinType.Overhaul;
            bool shouldBeActive = m_legacyContainer.activeInHierarchy;
            bool flag = reworkEnabled && shouldBeActive;
            m_container.SetActive(flag);
            m_miscElementsObject.SetActive(flag);

            if (Time.frameCount % 20 == 0)
            {
                //m_excContentMenuButton.interactable = ExclusiveContentManager.Instance.HasDownloadedContent();
                m_tutorialObject.SetActive(TitleScreenCustomizationManager.IntroduceCustomization);

                if (m_mobBotUsernameAvailable)
                {
                    string userName = ModIntegrationUtils.ModBot.GetModBotUsername();
                    if (!userName.IsNullOrEmpty())
                    {
                        m_modBotLogonText.text = $"{LocalizationManager.Instance.GetTranslatedString("modui_modbot_logged_as")} {userName.AddColor(Color.white)}";
                        m_modBotLogonText.enabled = true;
                        m_modBotLogInButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        m_modBotLogonText.enabled = false;
                        m_modBotLogInButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    m_modBotLogonText.enabled = false;
                    m_modBotLogInButton.gameObject.SetActive(false);
                }

                SteamManager steamManager = SteamManager.Instance;
                bool steamInitialized = steamManager && steamManager.Initialized;

                m_personalizationEditorButton.interactable = steamInitialized;
                m_workshopBrowserButton.interactable = steamInitialized;
            }

            m_skinButton.gameObject.SetActive(shouldBeActive);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            skin = TitleScreenSkinType.Vanilla;
        }

        public void SetSkinAccordingToSettings()
        {
            bool hypocrisisModEnabled = ModSpecialUtils.IsModEnabled("hypocrisis-mod");
            if (hypocrisisModEnabled)
            {
                skin = TitleScreenSkinType.Hypocrisis3;
            }
            else
            {
                skin = ModUIManager.ShowTitleScreenRework ? TitleScreenSkinType.Overhaul : TitleScreenSkinType.Vanilla;
            }
        }

        private bool checkIfModBotUserNameIsAvailable()
        {
            try
            {
                string str = ModIntegrationUtils.ModBot.GetModBotUsername();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SetMultiplayerButtonActive(bool value)
        {
            m_playMultiPlayerButton.interactable = value;
            m_playExpMultiPlayerButton.interactable = value;
        }

        private IEnumerator levelEditorTransitionCoroutine()
        {
            yield return new WaitForSecondsRealtime(0.4f);
            while (LevelManager.Instance.IsSpawningCurrentLevel())
                yield return null;

            yield return null;
            m_titleScreenUI.OnLevelEditorButtonClicked();
            yield return new WaitForSecondsRealtime(1f);
            TransitionManager.Instance.EndTransition();
            yield break;
        }

        private void refreshSkinButtonLabel()
        {
            var skinType = skin;
            string displayString;
            switch (skinType)
            {
                case TitleScreenSkinType.Vanilla:
                    displayString = LocalizationManager.Instance.GetTranslatedString("Vanilla");
                    break;
                case TitleScreenSkinType.Overhaul:
                    displayString = "Overhaul";
                    break;
                case TitleScreenSkinType.Hypocrisis3:
                    displayString = "Hypocrisis";
                    break;
                default:
                    displayString = skinType.ToString();
                    break;
            }

            m_skinButtonHCLabel.SetActive(skinType == TitleScreenSkinType.Hypocrisis3);
            m_skinNameLabelButton.text = displayString;
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
            _ = ModUIConstants.ShowMultiplayerGameModeSelectScreen();
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
            _ = ModUIConstants.ShowNewsPanel();
        }

        public void OnFeedbackButtonClicked()
        {
            _ = ModUIConstants.ShowFeedbackUIRework(false);
        }

        public void OnHubButtonClicked()
        {
            _ = ModUIConstants.ShowCommunityHub();
        }

        public void OnLevelDescriptionsEditorButtonClicked()
        {
            _ = ModUIConstants.ShowLevelDescriptionListEditor();
        }

        public void OnPersonalizationEditorButtonClicked()
        {
            PersonalizationEditorManager.Instance.StartEditorGameMode();
        }

        public void OnLocalizationEditorButtonClicked()
        {
            _ = ModUIConstants.ShowLocalizationEditor();
        }

        public void OnExcContentMenuButtonClicked()
        {
            _ = ModUIConstants.ShowExclusivePerksMenu();
        }

        public void OnContentButtonClicked()
        {
            _ = ModUIConstants.ShowAddonsMenu();
        }

        public void OnUpdatesButtonClicked()
        {
            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.UpdatesMenuRework))
            {
                _ = ModUIConstants.ShowUpdatesWindowRework();
                return;
            }
            _ = ModUIConstants.ShowUpdatesWindow();
        }

        public void OnAdvancementsButtonClicked()
        {
            if (!ModUIManager.ShowAdvancementsMenuRework)
            {
                ModCache.titleScreenUI.OnAchievementsButtonClicked();
                return;
            }
            _ = ModUIConstants.ShowAdvancementsMenuRework();
        }

        public void OnWorkshopBrowserButtonClicked()
        {
            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.WorkshopBrowserRework) || !ModUIManager.ShowWorkshopBrowserRework)
            {
                ModCache.titleScreenUI.OnWorkshopBrowserButtonClicked();
                return;
            }
            _ = ModUIConstants.ShowWorkshopBrowserRework();
        }

        public void OnLevelEditorButtonClicked()
        {
            if (!TransitionManager.OverhaulNonSceneTransitions)
            {
                if (LevelManager.Instance.IsSpawningCurrentLevel())
                    return;

                m_titleScreenUI.OnLevelEditorButtonClicked();
                return;
            }
            TransitionManager.Instance.DoNonSceneTransition(levelEditorTransitionCoroutine(), false);
        }

        public void OnOptionsButtonClicked()
        {
            if (!ModUIManager.ShowSettingsMenuRework)
            {
                ModCache.titleScreenUI.OnOptionsButtonClicked();
                return;
            }
            _ = ModUIConstants.ShowSettingsMenuRework(false);
        }

        public void OnCreditsButtonClicked()
        {
            _ = ModUIConstants.ShowInformationSelectMenu();
        }

        public void OnExitButtonClicked()
        {
            if (UIFeedbackMenu.HasEverSentFeedback)
            {
                Application.Quit();
                return;
            }
            _ = ModUIConstants.ShowFeedbackUIRework(true);
        }

        public void OnCustomizeButtonClicked()
        {
            _ = ModUIConstants.ShowTitleScreenCustomizationPanel(base.transform);
        }

        public void OnSetupButtonClicked()
        {
            _ = ModUIConstants.ShowSettingsMenuRework(true);
        }

        public void OnSkinButtonClicked()
        {
            switch (skin)
            {
                case TitleScreenSkinType.Vanilla:
                    skin = TitleScreenSkinType.Overhaul;
                    break;
                case TitleScreenSkinType.Overhaul:
                    if (ModSpecialUtils.IsModEnabled("hypocrisis-mod"))
                    {
                        skin = TitleScreenSkinType.Hypocrisis3;
                    }
                    else
                    {
                        skin = TitleScreenSkinType.Vanilla;
                    }
                    break;
                case TitleScreenSkinType.Hypocrisis3:
                    skin = TitleScreenSkinType.Vanilla;
                    break;
            }
            refreshSkinButtonLabel();
        }

        public void OnRobotEditorButtonClicked()
        {
            ModUIUtils.MessagePopupNotImplemented();
        }

        public void OnWeaponEditorButtonClicked()
        {
            ModUIUtils.MessagePopupNotImplemented();
        }

        public void OnStoryReworkButtonClicked()
        {
            MetagameProgressManager.Instance.SetProgress(MetagameProgress.P1_EmperorArrived);
            GameDataManager.Instance._storyModeData.CurentLevelID = "Story10_Rework";
            GameFlowManager.Instance.StartStoryModeGame(false);
        }

        public void OnBehindTheScenesButtonClicked()
        {
            _ = ModUIConstants.ShowDevelopmentGallery(base.transform);
        }

        public void OnDiscordServerButtonClicked()
        {
            _ = ModUIConstants.ShowDiscordServerMenu(base.transform);
            /*
            ModUIUtils.MessagePopup(true, "Join \"Modded Multiplayer\" Discord server?", null, 125f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
            {
                string link = "https://discord.gg/ezhvabY63m";
                Application.OpenURL(link);
            });*/
        }

        public void OnPatchNotesButtonClicked()
        {
            _ = ModUIConstants.ShowPatchNotes(new UIPatchNotes.ShowArguments()
            {
                CloseButtonActive = true,
                PanelOffset = Vector2.zero,
                ShrinkPanel = false,
                HideVersionList = false,
            });
        }

        public void OnFeaturesButtonClicked()
        {
            _ = ModUIConstants.ShowFeaturesMenu(base.transform);
        }

        public enum TitleScreenSkinType
        {
            Vanilla,

            Overhaul,

            Hypocrisis3
        }
    }
}
