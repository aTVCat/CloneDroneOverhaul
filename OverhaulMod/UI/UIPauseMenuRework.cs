using InternalModBot;
using OverhaulMod.Content;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPauseMenuRework : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("LegacyUIButton")]
        private readonly Button _legacyUIButton;

        [UIElementAction(nameof(OnResumeButtonClicked))]
        [UIElement("ResumeButton")]
        private readonly Button _resumeButton;

        [UIElementAction(nameof(OnAchievementsButtonClicked))]
        [UIElement("AchievementsButton")]
        private readonly Button _achievementsButton;

        [UIElementAction(nameof(OnCustomizationButtonClicked))]
        [UIElement("CustomizationButton")]
        private readonly Button _customizationButton;

        [UIElementAction(nameof(OnSettingsButtonClicked))]
        [UIElement("SettingsButton")]
        private readonly Button _settingsButton;

        [UIElementAction(nameof(OnModsButtonClicked))]
        [UIElement("ModsButton")]
        private readonly Button _modsButton;

        [UIElementAction(nameof(OnGiveFeedbackButtonClicked))]
        [UIElement("FeedbackButton")]
        private readonly Button _feedbackButton;

        [UIElement("LogoEn")]
        private readonly GameObject _logoEn;

        [UIElement("LogoCh")]
        private readonly GameObject _logoCh;

        [UIElement("LogoJa")]
        private readonly GameObject _logoJa;

        [UIElement("LogoKo")]
        private readonly GameObject _logoKo;

        [UIElement("ExitDialogue", false)]
        private readonly GameObject _exitDialogue;

        [UIElementAction(nameof(OnExitGameButtonClicked), true)]
        [UIElement("ExitGameButton")]
        private readonly Button _exitGameButton;

        [UIElementAction(nameof(OnMainMenuButtonClicked), true)]
        [UIElement("MainMenuButton")]
        private readonly Button _mainMenuButton;

        [UIElementAction(nameof(OnConfirmExitGameButtonClicked), false)]
        [UIElement("ConfirmExitGameButton")]
        private readonly Button _confirmExitGameButton;

        [UIElementAction(nameof(OnConfirmMainMenuButtonClicked), false)]
        [UIElement("ConfirmMainMenuButton")]
        private readonly Button _confirmMainMenuButton;

        [UIElementAction(nameof(OnReturnToLevelEditorButtonClicked), false)]
        [UIElement("ReturnToLevelEditorButton")]
        private readonly Button _returnToLevelEditorButton;

        [UIElementAction(nameof(OnStartMatchButtonClicked), false)]
        [UIElement("StartMatchButton")]
        private readonly Button _startMatchButton;

        [UIElement("StartMatchButtonText")]
        private readonly Text _startMatchButtonText;

        [UIElementAction(nameof(OnSkipLevelButtonClicked), false)]
        [UIElement("SkipLevelButton")]
        private readonly Button _skipLevelButton;

        [UIElementAction(nameof(OnReconnectButtonClicked), false)]
        [UIElement("ReconnectButton")]
        private readonly Button _reconnectButton;

        [UIElement("ConfirmExitGameText", false)]
        private readonly GameObject _confirmExitGameTextObject;

        [UIElement("ConfirmMainMenuText", false)]
        private readonly GameObject _confirmMainMenuTextObject;

        [UIElement("WorkshopPanel")]
        private readonly GameObject _workshopPanelObject;

        [UIElement("WorkshopPanel")]
        private readonly RectTransform _workshopPanelTransform;

        [UIElement("LevelName")]
        private readonly Text _workshopLevelTitleText;

        [UIElement("LevelCreator")]
        private readonly Text _workshopLevelCreatorText;

        [UIElementAction(nameof(OnWorkshopLevelUpVoteButtonClicked))]
        [UIElement("UpVoteButton")]
        private readonly Button _workshopLevelUpVoteButton;

        [UIElementAction(nameof(OnWorkshopLevelDownVoteButtonClicked))]
        [UIElement("DownVoteButton")]
        private readonly Button _workshopLevelDownVoteButton;

        [UIElementAction(nameof(OnWorkshopLevelInfoButtonClicked))]
        [UIElement("SteamPageButton")]
        private readonly Button _workshopLevelPageButton;

        [UIElement("PlayerList", false)]
        private readonly GameObject _playerInfoListObject;

        [UIElement("PlayerList", false)]
        private readonly RectTransform _playerInfoListTransform;

        [UIElement("PlayerListScrollRect")]
        private readonly RectTransform _playerListScrollRectTransform;

        [UIElement("PlayerSpecialStatLabel")]
        private readonly Text _playerInfoSpecialStatLabel;

        [UIElement("PlayerInfoDisplay", false)]
        private readonly ModdedObject _playerInfoDisplayPrefab;

        [UIElement("PlayerInfoDisplayContainer")]
        private readonly Transform _playerInfoDisplayContainer;

        [UIElement("ExtrasContentDownload", typeof(UIElementAddonEmbed))]
        private readonly UIElementAddonEmbed _extrasAddonEmbed;

        [UIElement("CodePanel", false)]
        private readonly GameObject _codePanelObject;

        [UIElement("CodeField")]
        private readonly InputField _codeField;

        [UIElementAction(nameof(OnCopyCodeButtonClicked))]
        [UIElement("CopyCodeButton")]
        private readonly Button _copyCodeButton;

        [UIElementAction(nameof(OnRevealCodeButtonClicked))]
        [UIElement("RevealCodeButton")]
        private readonly Button _revealCodeButton;

        [UIElement("PlayerIconLabel")]
        private readonly GameObject _playerIconLabelObject;

        private ulong _refreshedWorkshopPanelForItem;

        private float _refreshPlayerListTime;

        public override bool enableCursor
        {
            get
            {
                return true;
            }
        }

        public static bool disableOverhauledVersion { get; set; }

        protected override void OnInitialized()
        {
            _refreshPlayerListTime = -1f;

            _extrasAddonEmbed.AddonId = AddonManager.EXTRAS_ADDON_ID;
            _extrasAddonEmbed.Version = 0;
            _extrasAddonEmbed.RefreshDisplays();
            _extrasAddonEmbed.onContentDownloaded.AddListener(refreshPlayers);

            GlobalEventManager.Instance.AddEventListener(GlobalEvents.NumMultiplayerPlayersChanged, refreshPlayersIfActive);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.NumMultiplayerPlayersChanged, refreshPlayersIfActive);
        }

        public override void Show()
        {
            base.Show();
            TimeManager.Instance.OnGamePaused();

            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.HyperdomeSounds))
            {
                _ = AudioManager.Instance.PlayClipGlobal(ModAudioLibrary.Instance.HyperdomeUIPause);
            }
            else
            {
                _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionPress, 0f, 1f, 0f);
            }

            refreshLogo();
            refreshButtons();
            refreshPlayers();
            refreshWorkshopPanel();
            refreshCodePanel();
        }

        public override void Hide()
        {
            base.Hide();
            TimeManager.Instance.OnGameUnPaused();

            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.HyperdomeSounds))
            {
                _ = AudioManager.Instance.PlayClipGlobal(ModAudioLibrary.Instance.HyperdomeUIResume);
            }
            else
            {
                _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionBack, 0f, 1f, 0f);
            }
        }

        public override void Update()
        {
            base.Update();
            if (_refreshPlayerListTime != -1f && Time.unscaledTime >= _refreshPlayerListTime)
            {
                _refreshPlayerListTime = -1f;
                refreshPlayers();
            }
        }

        private void refreshPlayersIfActive()
        {
            if (isVisible)
            {
                _refreshPlayerListTime = Time.unscaledTime + 1f;
            }
        }

        private void refreshLogo()
        {
            string langId = LocalizationManager.Instance.GetCurrentLanguageCode();
            _logoEn.SetActive(langId != "ko" && langId != "ja" && langId != "zh-CN" && langId != "zh-TW");
            _logoCh.SetActive(langId == "zh-CN" || langId == "zh-TW");
            _logoKo.SetActive(langId == "ko");
            _logoJa.SetActive(langId == "ja");
        }

        private void refreshButtons()
        {
            _customizationButton.interactable = !GameModeManager.Is((GameMode)2500) && !GameModeManager.IsInLevelEditor() && CharacterTracker.Instance.GetPlayer();

            ArenaCoopManager arenaCoopManager = ArenaCoopManager.Instance;
            BattleRoyaleManager battleRoyaleManager = BattleRoyaleManager.Instance;
            bool isBattleRoyale = battleRoyaleManager;
            bool isCoop = arenaCoopManager;

            bool isBattleRoyaleWaitingArea = isBattleRoyale && battleRoyaleManager.IsProgress(BattleRoyaleMatchProgress.InWaitingArea);
            bool isBattleRoyaleFightStarted = isBattleRoyale && battleRoyaleManager.IsProgress(BattleRoyaleMatchProgress.FightingStarted);
            bool isCoopMatchNotStarted = isCoop && !arenaCoopManager.IsMatchStarted();

            _startMatchButton.gameObject.SetActive(MultiplayerMatchmakingManager.Instance.IsLocalPlayerHostOfCustomMatch() && (isBattleRoyaleWaitingArea || isBattleRoyaleFightStarted || isCoopMatchNotStarted));
            if (isCoopMatchNotStarted || isBattleRoyaleWaitingArea)
                _startMatchButtonText.text = LocalizationManager.Instance.GetTranslatedString("Start Match!");
            else if (isBattleRoyaleFightStarted)
                _startMatchButtonText.text = LocalizationManager.Instance.GetTranslatedString("Final Zone!");

            _skipLevelButton.gameObject.SetActive(GameModeManager.CanSkipCurrentLevel());
            _returnToLevelEditorButton.gameObject.SetActive(WorkshopLevelManager.Instance.IsPlaytestActive());
            _reconnectButton.gameObject.SetActive(GameModeManager.IsBattleRoyale() && MultiplayerMatchmakingManager.LastDuelRequest.GameType == GameRequestType.RandomBattleRoyale);

            _confirmExitGameTextObject.SetActive(false);
            _confirmMainMenuTextObject.SetActive(false);
            _confirmExitGameButton.gameObject.SetActive(false);
            _confirmMainMenuButton.gameObject.SetActive(false);
            _exitGameButton.gameObject.SetActive(true);
            _mainMenuButton.gameObject.SetActive(true);
        }

        private void refreshPlayers()
        {
            bool isInMultiplayer = GameModeManager.IsMultiplayer();
            _playerInfoListObject.SetActive(isInMultiplayer);
            if (!isInMultiplayer)
                return;

            bool lostContentModEnabled = ModSpecialUtils.IsModEnabled("cool-hidden-content");
            Vector2 sizeDelta = _playerInfoListTransform.sizeDelta;
            sizeDelta.y = lostContentModEnabled ? -145f : -110f;
            _playerInfoListTransform.sizeDelta = sizeDelta;

            Vector2 sizeDelta2 = _playerListScrollRectTransform.sizeDelta;
            sizeDelta2.y = _extrasAddonEmbed.ShouldBeHidden() ? -55f : -105f;
            _playerListScrollRectTransform.sizeDelta = sizeDelta2;

            if (_playerInfoDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(_playerInfoDisplayContainer);

            MultiplayerPlayerInfoManager multiplayerPlayerInfoManager = MultiplayerPlayerInfoManager.Instance;
            if (!multiplayerPlayerInfoManager)
                return;

            System.Collections.Generic.List<MultiplayerPlayerInfoState> infoStates = multiplayerPlayerInfoManager.GetAllPlayerInfoStates();
            if (infoStates.IsNullOrEmpty())
                return;

            bool canShowIcons = _extrasAddonEmbed.ShouldBeHidden();
            bool canShowWins = BattleRoyaleManager.Instance;
            Color killsColor = ModParseUtils.TryParseToColor("EC2711", Color.red);
            Color winsColor = Color.white;

            Text specialStatLabel = _playerInfoSpecialStatLabel;
            if (canShowWins)
            {
                specialStatLabel.color = winsColor;
                specialStatLabel.text = "Wins";
            }
            else
            {
                specialStatLabel.color = killsColor;
                specialStatLabel.text = "Kills";
            }
            specialStatLabel.enabled = GameModeManager.IsBattleRoyale() || GameModeManager.IsMultiplayerDuel();

            _playerIconLabelObject.SetActive(canShowIcons);

            int index = -1;
            foreach (MultiplayerPlayerInfoState infoState in infoStates)
            {
                index++;
                if (!infoState || infoState.IsDetached())
                    continue;

                IPlayerInfoState playerInfoState;
                try
                {
                    playerInfoState = infoState.state;
                }
                catch
                {
                    continue;
                }

                ModdedObject playerDisplay = Instantiate(_playerInfoDisplayPrefab, _playerInfoDisplayContainer);
                playerDisplay.gameObject.SetActive(true);
                playerDisplay.GetObject<Text>(0).text = playerInfoState.DisplayName;
                Text countLabel = playerDisplay.GetObject<Text>(1);
                if (canShowWins)
                {
                    countLabel.color = winsColor;
                    countLabel.text = playerInfoState.LastBotStandingWins.ToString();
                }
                else
                {
                    countLabel.color = killsColor;
                    countLabel.text = playerInfoState.Kills.ToString();
                }
                countLabel.enabled = GameModeManager.IsBattleRoyale() || GameModeManager.IsMultiplayerDuel();
                playerDisplay.GetObject<Text>(2).text = GetPlatformString((PlayFab.ClientModels.LoginIdentityProvider)playerInfoState.PlatformID);
                playerDisplay.GetObject<RawImage>(3).enabled = canShowIcons;
                playerDisplay.GetObject<GameObject>(4).SetActive(playerInfoState.IsDisconnected);
                playerDisplay.GetObject<GameObject>(6).SetActive(canShowIcons);
                playerDisplay.GetObject<GameObject>(7).SetActive(!canShowIcons);

                UIElementPlayerInfoDisplay playerInfoDisplay = playerDisplay.gameObject.AddComponent<UIElementPlayerInfoDisplay>();
                playerInfoDisplay.InitializeElement();
                playerInfoDisplay.LoadRobotHead(playerInfoState.CharacterModelIndex, playerInfoState.FavouriteColor);
            }
        }

        private void refreshWorkshopPanel()
        {
            SteamWorkshopItem item;
            try
            {
                item = WorkshopLevelManager.Instance.GetCurrentLevelWorkshopItem();
            }
            catch
            {
                item = null;
            }

            bool shouldShowPanel = item != null;

            bool lostContentModEnabled = ModSpecialUtils.IsModEnabled("cool-hidden-content");
            Vector2 anchoredPosition = _workshopPanelTransform.anchoredPosition;
            anchoredPosition.y = lostContentModEnabled ? 50f : 15f;
            _workshopPanelTransform.anchoredPosition = anchoredPosition;
            _workshopPanelObject.SetActive(shouldShowPanel);

            if (!shouldShowPanel)
                return;

            ulong itemId = (ulong)item.WorkshopItemID;
            if (itemId != _refreshedWorkshopPanelForItem)
            {
                _refreshedWorkshopPanelForItem = itemId;

                Text titleText = _workshopLevelTitleText;
                titleText.text = item.Title;
                if (item.Title.Contains("color="))
                    titleText.color = Color.white;
                else
                    titleText.color = ModParseUtils.TryParseToColor("#FF4040", Color.red);

                if (!item.CreatorName.IsNullOrEmpty() && item.CreatorName != "[unknown]")
                    _workshopLevelCreatorText.text = $"By {item.CreatorName}";
                else
                    _workshopLevelCreatorText.text = $"By {item.CreatorID}";

                _workshopLevelUpVoteButton.gameObject.SetActive(false);
                _workshopLevelDownVoteButton.gameObject.SetActive(false);

                ModSteamUGCUtils.GetUserVote(item.WorkshopItemID, delegate (WorkshopItemVote workshopItemVote)
                {
                    SteamWorkshopItem item2 = WorkshopLevelManager.Instance.GetCurrentLevelWorkshopItem();
                    if (item != item2)
                        return;

                    _workshopLevelUpVoteButton.gameObject.SetActive(true);
                    _workshopLevelDownVoteButton.gameObject.SetActive(true);

                    if (!workshopItemVote.HasVoted)
                    {
                        _workshopLevelUpVoteButton.interactable = true;
                        _workshopLevelDownVoteButton.interactable = true;
                        return;
                    }
                    _workshopLevelUpVoteButton.interactable = !workshopItemVote.VoteValue;
                    _workshopLevelDownVoteButton.interactable = workshopItemVote.VoteValue;
                });
            }
        }

        private void refreshCodePanel()
        {
            if (MultiplayerMatchmakingManager.Instance.IsLocalPlayerHostOfCustomMatch())
            {
                _codePanelObject.SetActive(true);
                _revealCodeButton.gameObject.SetActive(true);
                _codeField.text = MultiplayerMatchmakingManager.Instance.GetLastInviteCode();
            }
            else
            {
                _codePanelObject.SetActive(false);
            }
        }

        public void OnResumeButtonClicked()
        {
            Hide();
        }

        public void OnAchievementsButtonClicked()
        {
            if (!ModUIManager.ShowAdvancementsMenuRework)
            {
                ModCache.gameUIRoot.EscMenu.OnAchievementsClicked();
                return;
            }
            _ = ModUIConstants.ShowAdvancementsMenuRework();
        }

        public void OnCustomizationButtonClicked()
        {
            Hide();
            _ = ModUIConstants.ShowPersonalizationItemsBrowser();
        }

        public void OnSettingsButtonClicked()
        {
            _ = ModUIConstants.ShowSettingsMenuRework(false);
        }

        public void OnModsButtonClicked()
        {
            ModsPanelManager.Instance.openModsMenu();
        }

        public void OnGiveFeedbackButtonClicked()
        {
            _ = ModUIConstants.ShowFeedbackUIRework(false);
        }

        public void OnMainMenuButtonClicked()
        {
            if (GameModeManager.Is((GameMode)2500) || (GameModeManager.IsInLevelEditor() && LevelEditorDataManager.Instance.CurrentLevelNeedsSaving()))
            {
                ModUIUtils.MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("dialog_exit_editor"), LocalizationManager.Instance.GetTranslatedString("dialog_exit_editor_desc"), 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, OnConfirmMainMenuButtonClicked, null);
                return;
            }

            _confirmExitGameTextObject.SetActive(false);
            _confirmMainMenuTextObject.SetActive(true);
            _confirmExitGameButton.gameObject.SetActive(false);
            _confirmMainMenuButton.gameObject.SetActive(true);
            _mainMenuButton.gameObject.SetActive(false);
            _exitGameButton.gameObject.SetActive(true);
        }

        public void OnConfirmMainMenuButtonClicked()
        {
            WorkshopLevelManager workshopLevelManager = WorkshopLevelManager.Instance;
            workshopLevelManager.ClearPlaytestData();
            workshopLevelManager.SetPlaytestActive(false);

            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }

        public void OnExitGameButtonClicked()
        {
            if (GameModeManager.Is((GameMode)2500) || (GameModeManager.IsInLevelEditor() && LevelEditorDataManager.Instance.CurrentLevelNeedsSaving()))
            {
                ModUIUtils.MessagePopup(true, LocalizationManager.Instance.GetTranslatedString("dialog_exit_editor"), LocalizationManager.Instance.GetTranslatedString("dialog_exit_editor_desc"), 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, OnConfirmExitGameButtonClicked, null);
                return;
            }

            _confirmExitGameTextObject.SetActive(true);
            _confirmMainMenuTextObject.SetActive(false);
            _confirmExitGameButton.gameObject.SetActive(true);
            _confirmMainMenuButton.gameObject.SetActive(false);
            _mainMenuButton.gameObject.SetActive(true);
            _exitGameButton.gameObject.SetActive(false);
        }

        public void OnConfirmExitGameButtonClicked()
        {
            Application.Quit();
        }

        public void OnReturnToLevelEditorButtonClicked()
        {
            ModCache.gameUIRoot.EscMenu.OnBackToLevelEditorButtonClicked();
        }

        public void OnStartMatchButtonClicked()
        {
            ModCache.gameUIRoot.EscMenu.OnStartBattleRoyaleLevelClicked();
            Hide();
        }

        public void OnSkipLevelButtonClicked()
        {
            Hide();
            ModCache.gameUIRoot.EscMenu.OnSkipWorkshopLevelClicked();
        }

        public void OnLegacyUIButtonClicked()
        {
            Hide();
            ModUIUtils.ShowVanillaEscMenu();
        }

        public void OnWorkshopLevelUpVoteButtonClicked()
        {
            SteamWorkshopItem item = WorkshopLevelManager.Instance.GetCurrentLevelWorkshopItem();
            if (item == null)
                return;

            _workshopLevelUpVoteButton.interactable = false;
            ModSteamUGCUtils.SetUserVote(item.WorkshopItemID, true, delegate (SetUserItemVoteResult_t t, bool ioError)
            {
                SteamWorkshopItem item2 = WorkshopLevelManager.Instance.GetCurrentLevelWorkshopItem();
                if (item != item2)
                    return;

                if (ioError || t.m_eResult != EResult.k_EResultOK)
                    ModUIUtils.MessagePopupOK("Vote error", $"Error code: {t.m_eResult} (ioError: {ioError})", 150f, true);
                else
                {
                    _workshopLevelDownVoteButton.interactable = t.m_bVoteUp;
                    _workshopLevelUpVoteButton.interactable = !t.m_bVoteUp;
                    return;
                }

                _workshopLevelUpVoteButton.interactable = true;
            });
        }

        public void OnWorkshopLevelDownVoteButtonClicked()
        {
            SteamWorkshopItem item = WorkshopLevelManager.Instance.GetCurrentLevelWorkshopItem();
            if (item == null)
                return;

            _workshopLevelDownVoteButton.interactable = false;
            ModSteamUGCUtils.SetUserVote(item.WorkshopItemID, false, delegate (SetUserItemVoteResult_t t, bool ioError)
            {
                SteamWorkshopItem item2 = WorkshopLevelManager.Instance.GetCurrentLevelWorkshopItem();
                if (item != item2)
                    return;

                if (ioError || t.m_eResult != EResult.k_EResultOK)
                    ModUIUtils.MessagePopupOK("Vote error", $"Error code: {t.m_eResult} (ioError: {ioError})", 150f, true);
                else
                {
                    _workshopLevelDownVoteButton.interactable = t.m_bVoteUp;
                    _workshopLevelUpVoteButton.interactable = !t.m_bVoteUp;
                    return;
                }

                _workshopLevelDownVoteButton.interactable = true;
            });
        }

        public void OnWorkshopLevelInfoButtonClicked()
        {
            SteamWorkshopItem item = WorkshopLevelManager.Instance.GetCurrentLevelWorkshopItem();
            if (item == null)
                return;

            string link = item.GetURL();
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
                SteamFriends.ActivateGameOverlayToWebPage(link);
            else
                Application.OpenURL(link);
        }

        public void OnRevealCodeButtonClicked()
        {
            _revealCodeButton.gameObject.SetActive(false);
        }

        public void OnCopyCodeButtonClicked()
        {
            GUIUtility.systemCopyBuffer = _codeField.text;
        }

        public void OnReconnectButtonClicked()
        {
            MultiplayerMatchmakingManager.ShouldStartBattleRoyaleGameOnLoad = true;
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }

        public static string GetPlatformString(PlayFab.ClientModels.LoginIdentityProvider login, bool colored = true)
        {
            if (!colored)
            {
                switch (login)
                {
                    case PlayFab.ClientModels.LoginIdentityProvider.Custom:
                    case PlayFab.ClientModels.LoginIdentityProvider.CustomServer:
                        return "Custom";

                    case PlayFab.ClientModels.LoginIdentityProvider.NintendoSwitch:
                    case PlayFab.ClientModels.LoginIdentityProvider.NintendoSwitchAccount:
                        return "Switch";
                    case PlayFab.ClientModels.LoginIdentityProvider.PlayFab:
                        return "PlayFab";
                    case PlayFab.ClientModels.LoginIdentityProvider.PSN:
                        return "PSN";
                    case PlayFab.ClientModels.LoginIdentityProvider.Steam:
                        return "Steam";
                    case PlayFab.ClientModels.LoginIdentityProvider.Twitch:
                        return "Twitch";
                    case PlayFab.ClientModels.LoginIdentityProvider.XBoxLive:
                        return "XBOX";
                }
                return "N/A";
            }

            switch (login)
            {
                case PlayFab.ClientModels.LoginIdentityProvider.Custom:
                case PlayFab.ClientModels.LoginIdentityProvider.CustomServer:
                    return "<color=#cacaca>Custom</color>";

                case PlayFab.ClientModels.LoginIdentityProvider.NintendoSwitch:
                case PlayFab.ClientModels.LoginIdentityProvider.NintendoSwitchAccount:
                    return "<color=#FF3B26>Switch</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.PlayFab:
                    return "<color=#ffffff>PlayFab</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.PSN:
                    return "<color=#4F85FF>PSN</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.Steam:
                    return "<color=#2769E5>Steam</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.Twitch:
                    return "<color=#A426E4>Twitch</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.XBoxLive:
                    return "<color=#0DB30F>XBOX</color>";
            }
            return "<color=#ffffff>N/A</color>";
        }
    }
}
