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
        private readonly GameObject _container;

        [UIElement("ButtonsBG")]
        private readonly RectTransform _containerTransform;

        [UIElement("CenterFade")]
        private readonly GameObject _centerFade;

        [UIElement("CenterFade")]
        private readonly Graphic _centerFadeGraphic;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose)]
        [UIElementAction(nameof(OnPlaySinglePlayerButtonClicked))]
        [UIElement("PlaySingleplayerButton")]
        private readonly Button _playSinglePlayerButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose)]
        [UIElementAction(nameof(OnPlayMultiPlayerButtonClicked))]
        [UIElement("PlayMultiplayerButton")]
        private readonly Button _playMultiPlayerButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnViewMultiplayerErrorButtonClicked))]
        [UIElement("ViewMultiplayerErrorButton")]
        private readonly Button _viewMultiplayerErrorButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose_NoEcho)]
        [UIElementAction(nameof(OnModsButtonClicked))]
        [UIElement("ModsButton")]
        private readonly Button _modsButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnExclusivePerksMenuButtonClicked))]
        [UIElement("NewExclusiveContentMenuButton")]
        private readonly Button _newExcContentMenuButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnUpdatesButtonClicked))]
        [UIElement("NewUpdatesButton")]
        private readonly Button _newUpdatesButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnOptionsButtonClicked))]
        [UIElement("OptionsButton")]
        private readonly Button _optionsButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnAdvancementsButtonClicked))]
        [UIElement("AchievementsButton")]
        private readonly Button _advancementsButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnWorkshopBrowserButtonClicked))]
        [UIElement("WorkshopBrowserButton")]
        private readonly Button _workshopBrowserButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose)]
        [UIElementAction(nameof(OnPersonalizationEditorButtonClicked))]
        [UIElement("PersonalizationEditorButton")]
        private readonly Button _personalizationEditorButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnAddonsButtonClicked))]
        [UIElement("AddonsButton", typeof(UIElementTitleScreenAddonsButton))]
        private readonly Button _addonsButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose)]
        [UIElementAction(nameof(OnLevelEditorButtonClicked))]
        [UIElement("LevelEditorButton")]
        private readonly Button _levelEditorButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnCreditsButtonClicked))]
        [UIElement("CreditsButton")]
        private readonly Button _creditsButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Choose_NoEcho)]
        [UIElementAction(nameof(OnExitButtonClicked))]
        [UIElement("ExitButton")]
        private readonly Button _quitButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnCustomizeButtonClicked))]
        [UIElement("CustomizeButton")]
        private readonly Button _customizeButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnFeedbackClicked))]
        [UIElement("FeedbackButton")]
        private readonly Button _feedbackButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnModBotLogInButtonClicked))]
        [UIElement("ModBotLogInButton", false)]
        private readonly Button _modBotLogInButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnSkinButtonClicked))]
        [UIElement("SkinButton")]
        private readonly Button _skinButton;

        [UIElement("SkinNameLabel")]
        private readonly Text _skinNameLabelButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnDiscordServerButtonClicked))]
        [UIElement("DiscordServerButton")]
        private readonly Button _discordServerButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnPatchNotesButtonClicked))]
        [UIElement("PatchNotesButton")]
        private readonly Button _patchNotesButton;

        [ButtonWithSound(ButtonWithSound.SoundType.Click)]
        [UIElementAction(nameof(OnPatchNotesButtonClicked))]
        [UIElement("DebugButton")]
        private readonly Button _debugButton;

        [UIElement("MiscElements")]
        private readonly GameObject _miscElementsObject;

        [UIElement("ModBotLogonText")]
        private readonly Text _modBotLogonText;

        [UIElement("ErrorMessage", typeof(UIElementMultiplayerMessageBox), false)]
        public UIElementMultiplayerMessageBox ErrorMessage;

        [UIElement("ViewMultiplayerErrorButton", typeof(UIElementMultiplayerMessageButton))]
        public UIElementMultiplayerMessageButton ErrorMessageButton;

        [UIElement("AdvancementsProgressImage")]
        private readonly Image _advancementsProgressImage;

        [UIElement("AdvancementsProgressText")]
        private readonly Text _advancementsProgressText;

        [UIElement("AdvancementsProgressPercentageText")]
        private readonly Text _advancementsProgressPercentageText;

        [UIElement("HypocrisisSkinHolder")]
        private readonly Transform _hypocrisisSkinHolder;

        [UIElement("hcLabel", false)]
        private readonly GameObject _skinButtonHCLabel;

        public override bool CloseOnEscapeButtonPress => false;

        private Vector2 _oldOffsetMin, _oldOffsetMax, _oldAnchorMax;
        private Vector2 _newOffsetMin, _newOffsetMax, _newAnchorMax;

        private Vector2 _oldVanillaAnchoredPosition, _oldVanillaAnchorMax, _oldVanillaOffsetMax;
        private Vector2 _newVanillaAnchoredPosition, _newVanillaAnchorMax, _newVanillaOffsetMax;

        private TitleScreenUI _titleScreenUI;
        private CanvasGroup _canvasGroup;
        private GameObject _vanillaContainer;
        private Graphic _leftFadeGraphic;
        private RectTransform _vanillaTitleScreenButtonsContainer;

        private Camera _logoCamera;

        private Graphic _leftSideFadeGraphic;

        private RectTransform _socialButtonContainer;
        private RectTransform _socialButtonPopoutHolder;

        private Vector2 _oldSocialButtonContainerPosition, _newSocialButtonContainerPosition;
        private Vector2 _oldSocialButtonPopoutHolderPosition, _newSocialButtonPopoutHolderPosition;

        private bool _hasSpawnedHypocrisisSkin;

        private bool _mobBotUsernameAvailable;

        public bool hideVanillaTitleScreen
        {
            get
            {
                return skin != TitleScreenSkinType.Vanilla;
            }
        }

        private TitleScreenSkinType _skin;
        public TitleScreenSkinType skin
        {
            get
            {
                return _skin;
            }
            set
            {
                if (_skin == value)
                    return;

                _skin = value;

                bool nonVanilla = value != TitleScreenSkinType.Vanilla;
                bool overhaul = value == TitleScreenSkinType.Overhaul;
                bool hypocrisis = value == TitleScreenSkinType.Hypocrisis3;

                CanvasGroup group = _canvasGroup;
                if (group)
                {
                    group.alpha = nonVanilla ? 0f : 1f;
                    group.interactable = !nonVanilla;
                }

                if (_socialButtonContainer && _socialButtonPopoutHolder)
                {
                    _socialButtonContainer.anchoredPosition = overhaul ? _newSocialButtonContainerPosition : _oldSocialButtonContainerPosition;
                    _socialButtonPopoutHolder.anchoredPosition = overhaul ? _newSocialButtonPopoutHolderPosition : _oldSocialButtonPopoutHolderPosition;
                }

                if (hypocrisis && !_hasSpawnedHypocrisisSkin)
                {
                    ModUIConstants.ShowTitleScreenHypocrisisSkin(_hypocrisisSkinHolder);
                    _hasSpawnedHypocrisisSkin = true;
                }
            }
        }

        protected override void OnInitialized()
        {
            bool debug = ModBuild.IsDebugBuild;
            _debugButton.gameObject.SetActive(debug);
            _modBotLogonText.text = "Not logged in";

            float fraction = GameplayAchievementManager.Instance.GetFractionOfAchievementsCompleted();
            _advancementsProgressImage.fillAmount = fraction;
            _advancementsProgressText.text = $"{ModGameUtils.GetNumOfAchievementsCompleted()}/{ModGameUtils.GetNumOfAchievements()}";
            _advancementsProgressPercentageText.text = $"({Mathf.FloorToInt(fraction * 100f)}%)";

            _oldOffsetMin = _containerTransform.offsetMin;
            _oldOffsetMax = _containerTransform.offsetMax;
            _oldAnchorMax = _containerTransform.anchorMax;
            _newOffsetMin = new Vector2(0f, _oldOffsetMin.y);
            _newOffsetMax = new Vector2(0f, _oldOffsetMax.y);
            _newAnchorMax = new Vector2(1f, 0f);

            TitleScreenUI titleScreenUI = ModCache.titleScreenUI;
            _titleScreenUI = titleScreenUI;
            _vanillaTitleScreenButtonsContainer = titleScreenUI.RootButtonsContainer;
            _leftSideFadeGraphic = titleScreenUI.LeftFadeBG.GetComponent<Graphic>();

            _oldVanillaAnchoredPosition = _vanillaTitleScreenButtonsContainer.anchoredPosition;
            _oldVanillaAnchorMax = _vanillaTitleScreenButtonsContainer.anchorMax;
            _oldVanillaOffsetMax = _vanillaTitleScreenButtonsContainer.offsetMax;
            _newVanillaAnchoredPosition = new Vector2(0f, _oldVanillaAnchoredPosition.y);
            _newVanillaAnchorMax = new Vector2(1f, _oldVanillaAnchorMax.y);
            _newVanillaOffsetMax = new Vector2(0f, _oldVanillaOffsetMax.y);

            _logoCamera = ArenaCameraManager.Instance.TitleScreenLogoCamera;

            // add canvas group
            CanvasGroup group = titleScreenUI.RootButtonsContainerBG.GetComponent<CanvasGroup>() ?? titleScreenUI.RootButtonsContainerBG.AddComponent<CanvasGroup>();
            group.blocksRaycasts = true;
            _canvasGroup = group;
            _vanillaContainer = group.gameObject;

            // adjust social buttons
            Transform socialButtons = titleScreenUI.SocialButtonPanel?.transform;
            RectTransform socialButtonContainer = TransformUtils.FindChildRecursive(socialButtons, "VerticalSocialButtons") as RectTransform;
            RectTransform socialButtonPopoutHolder = TransformUtils.FindChildRecursive(socialButtons, "PopoutHolder") as RectTransform;
            if (socialButtonContainer && socialButtonPopoutHolder)
            {
                _socialButtonContainer = socialButtonContainer;
                _oldSocialButtonContainerPosition = socialButtonContainer.anchoredPosition;
                _newSocialButtonContainerPosition = socialButtonContainer.anchoredPosition + (Vector2.up * 35f);
                _socialButtonPopoutHolder = socialButtonPopoutHolder;
                _oldSocialButtonPopoutHolderPosition = socialButtonPopoutHolder.anchoredPosition;
                _newSocialButtonPopoutHolderPosition = socialButtonPopoutHolder.anchoredPosition + (Vector2.up * 35f);
            }

            SetSkinAccordingToSettings();

            RefreshPosition();
            RefreshFade();

            refreshSkinButtonLabel();

            _mobBotUsernameAvailable = checkIfModBotUserNameIsAvailable();
        }

        public override void Hide()
        {
            base.Hide();
            if (UIVersionLabel.instance) UIVersionLabel.instance.ResetGameplayWatermark();
        }

        public override void Update()
        {
            bool reworkEnabled = skin == TitleScreenSkinType.Overhaul;
            bool shouldBeActive = _vanillaContainer.activeInHierarchy;
            bool flag = reworkEnabled && shouldBeActive;
            _container.SetActive(flag);
            _miscElementsObject.SetActive(flag);
            _hypocrisisSkinHolder.gameObject.SetActive(shouldBeActive && skin == TitleScreenSkinType.Hypocrisis3);

            if (Time.frameCount % 20 == 0)
            {
                if (TitleScreenCustomizationManager.ShowModBotAccountInfo && _mobBotUsernameAvailable)
                {
                    string userName = ModIntegrationUtils.ModBot.GetModBotUsername();
                    if (!userName.IsNullOrEmpty())
                    {
                        _modBotLogonText.text = $"{LocalizationManager.Instance.GetTranslatedString("modui_modbot_logged_as")} {userName.AddColor(Color.white)}";
                        _modBotLogonText.enabled = true;
                        _modBotLogInButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        _modBotLogonText.enabled = false;
                        _modBotLogInButton.gameObject.SetActive(true);
                    }
                }
                else
                {
                    _modBotLogonText.enabled = false;
                    _modBotLogInButton.gameObject.SetActive(false);
                }

                SteamManager steamManager = SteamManager.Instance;
                bool steamInitialized = steamManager && steamManager.Initialized;

                _personalizationEditorButton.interactable = steamInitialized;
                _workshopBrowserButton.interactable = steamInitialized;
            }

            _skinButton.gameObject.SetActive(shouldBeActive);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            skin = TitleScreenSkinType.Vanilla;
        }

        public void RefreshFade()
        {
            _centerFadeGraphic.color = new Color(0f, 0f, 0f, TitleScreenCustomizationManager.BackgroundFadePower);
            _leftSideFadeGraphic.color = new Color(0f, 0f, 0f, TitleScreenCustomizationManager.BackgroundFadePower);
        }

        public void RefreshPosition()
        {
            bool isLeftSide = TitleScreenCustomizationManager.PanelPosition == TitleScreenPanelPosition.LeftSide;
            _containerTransform.anchorMax = isLeftSide ? _oldAnchorMax : _newAnchorMax;
            _containerTransform.offsetMin = isLeftSide ? _oldOffsetMin : _newOffsetMin;
            _containerTransform.offsetMax = isLeftSide ? _oldOffsetMax : _newOffsetMax;

            _vanillaTitleScreenButtonsContainer.anchoredPosition = isLeftSide ? _oldVanillaAnchoredPosition : _newVanillaAnchoredPosition;
            _vanillaTitleScreenButtonsContainer.anchorMax = isLeftSide ? _oldVanillaAnchorMax : _newVanillaAnchorMax;
            _vanillaTitleScreenButtonsContainer.offsetMax = isLeftSide ? _oldVanillaOffsetMax : _newVanillaOffsetMax;

            _centerFade.SetActive(!isLeftSide);

            ArenaCameraManager.Instance.updateLogoCameraRect();

            ModActionUtils.DoInFrames(delegate
            {
                UIVersionLabel versionLabel = UIVersionLabel.instance;
                if (versionLabel)
                {
                    if (isLeftSide)
                    {
                        versionLabel.ResetGameplayWatermark();
                    }
                    else
                    {
                        versionLabel.CenterGameplayWatermark();
                    }
                }
            }, 10);
        }

        public void SetSkinAccordingToSettings()
        {
            bool hypocrisisModEnabled = ModBuild.ShouldShowHypocrisis3Special();
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
            _playMultiPlayerButton.interactable = value;
        }

        private IEnumerator levelEditorTransitionCoroutine()
        {
            yield return new WaitForSecondsRealtime(1f);
            while (LevelManager.Instance.IsSpawningCurrentLevel())
                yield return null;

            yield return null;
            _titleScreenUI.OnLevelEditorButtonClicked();
            yield return new WaitForSecondsRealtime(1f);
            TransitionManager.Instance.EndTransition();
            yield break;
        }

        private void refreshSkinButtonLabel()
        {
            TitleScreenSkinType skinType = skin;
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

            _skinButtonHCLabel.SetActive(skinType == TitleScreenSkinType.Hypocrisis3);
            _skinNameLabelButton.text = displayString;
        }

        public void OnPlaySinglePlayerButtonClicked()
        {
            _titleScreenUI.OnPlaySingleplayerButtonClicked();
        }

        public void OnPlayMultiPlayerButtonClicked()
        {
            _titleScreenUI.OnMultiplayerButtonClicked();
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

        public void OnPersonalizationEditorButtonClicked()
        {
            PersonalizationEditorManager.Instance.StartEditorGameMode();
        }

        public void OnExclusivePerksMenuButtonClicked()
        {
            _ = ModUIConstants.ShowExclusivePerksMenu();
        }

        public void OnAddonsButtonClicked()
        {
            _ = ModUIConstants.ShowAddonsMenu();
        }

        public void OnUpdatesButtonClicked()
        {
            _ = ModUIConstants.ShowUpdatesWindowRework();
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
            if (!ModUIManager.ShowWorkshopBrowserRework)
            {
                ModCache.titleScreenUI.OnWorkshopBrowserButtonClicked();
                return;
            }
            _ = ModUIConstants.ShowWorkshopBrowserRework();
        }

        public void OnLevelEditorButtonClicked()
        {
            if (!TransitionManager.OverhaulSceneTransitions)
            {
                if (LevelManager.Instance.IsSpawningCurrentLevel())
                    return;

                _titleScreenUI.OnLevelEditorButtonClicked();
                return;
            }
            TransitionManager.Instance.DoNonSceneTransition(levelEditorTransitionCoroutine());
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

        public void OnFeedbackClicked()
        {
            _ = ModUIConstants.ShowFeedbackUIRework(false);
        }

        public void OnCustomizeButtonClicked()
        {
            _ = ModUIConstants.ShowTitleScreenCustomizationPanel(base.transform);
        }

        public void OnSkinButtonClicked()
        {
            switch (skin)
            {
                case TitleScreenSkinType.Vanilla:
                    skin = TitleScreenSkinType.Overhaul;
                    break;
                case TitleScreenSkinType.Overhaul:
                    if (ModBuild.ShouldShowHypocrisis3Special())
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

        public void OnDiscordServerButtonClicked()
        {
            _ = ModUIConstants.ShowDiscordServerMenu(base.transform);
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

        public enum TitleScreenSkinType
        {
            Vanilla,

            Overhaul,

            Hypocrisis3
        }
    }
}