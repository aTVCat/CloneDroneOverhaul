using InternalModBot;
using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPauseMenuRework : OverhaulUIBehaviour
    {
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

        [UIElementAction(nameof(OnGiveFeedbackButtonClicked))]
        [UIElement("FeedbackButton")]
        private readonly Button m_feedbackButton;

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

        [UIElementAction(nameof(OnExitGameButtonClicked), true)]
        [UIElement("ExitGameButton")]
        private readonly Button m_exitGameButton;

        [UIElementAction(nameof(OnMainMenuButtonClicked), true)]
        [UIElement("MainMenuButton")]
        private readonly Button m_mainMenuButton;

        [UIElementAction(nameof(OnConfirmExitGameButtonClicked), false)]
        [UIElement("ConfirmExitGameButton")]
        private readonly Button m_confirmExitGameButton;

        [UIElementAction(nameof(OnConfirmMainMenuButtonClicked), false)]
        [UIElement("ConfirmMainMenuButton")]
        private readonly Button m_confirmMainMenuButton;

        [UIElementAction(nameof(OnReturnToLevelEditorButtonClicked), false)]
        [UIElement("ReturnToLevelEditorButton")]
        private readonly Button m_returnToLevelEditorButton;

        [UIElementAction(nameof(OnStartMatchButtonClicked), false)]
        [UIElement("StartMatchButton")]
        private readonly Button m_startMatchButton;

        [UIElementAction(nameof(OnSkipLevelButtonClicked), false)]
        [UIElement("SkipLevelButton")]
        private readonly Button m_skipLevelButton;

        [UIElement("ConfirmExitGameText", false)]
        private readonly GameObject m_confirmExitGameTextObject;

        [UIElement("ConfirmMainMenuText", false)]
        private readonly GameObject m_confirmMainMenuTextObject;

        [UIElement("PlayerListOld", false)]
        private readonly GameObject m_playerListOldObject;

        [UIElement("PlayerDisplayPrefab", false)]
        private readonly ModdedObject m_playerDisplayOld;

        [UIElement("PlayerDisplayContainer")]
        private readonly Transform m_playerDisplayContainerOld;


        [UIElement("WorkshopPanel")]
        private readonly GameObject m_workshopPanelObject;

        [UIElement("WorkshopPanel")]
        private readonly RectTransform m_workshopPanelTransform;

        [UIElement("LevelName")]
        private readonly Text m_workshopLevelTitleText;

        [UIElement("LevelCreator")]
        private readonly Text m_workshopLevelCreatorText;

        [UIElementAction(nameof(OnWorkshopLevelUpVoteButtonClicked))]
        [UIElement("UpVoteButton")]
        private readonly Button m_workshopLevelUpVoteButton;

        [UIElementAction(nameof(OnWorkshopLevelDownVoteButtonClicked))]
        [UIElement("DownVoteButton")]
        private readonly Button m_workshopLevelDownVoteButton;

        [UIElementAction(nameof(OnWorkshopLevelInfoButtonClicked))]
        [UIElement("SteamPageButton")]
        private readonly Button m_workshopLevelPageButton;

        private ulong m_refreshedWorkshopPanelForItem;

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
            TimeManager.Instance.OnGamePaused();
            _ = AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.UISelectionPress, 0f, 1f, 0f);

            refreshLogo();
            refreshButtons();
            refreshPlayers();
            refreshWorkshopPanel();
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

            ArenaCoopManager arenaCoopManager = ArenaCoopManager.Instance;
            BattleRoyaleManager battleRoyaleManager = BattleRoyaleManager.Instance;
            bool flag1 = battleRoyaleManager && (battleRoyaleManager.IsProgress(BattleRoyaleMatchProgress.InWaitingArea) || battleRoyaleManager.IsProgress(BattleRoyaleMatchProgress.FightingStarted));
            bool flag2 = arenaCoopManager && !arenaCoopManager.IsMatchStarted();
            m_startMatchButton.gameObject.SetActive(MultiplayerMatchmakingManager.Instance.IsLocalPlayerHostOfCustomMatch() && (flag1 || flag2));
            m_skipLevelButton.gameObject.SetActive(GameModeManager.CanSkipCurrentLevel());
            m_returnToLevelEditorButton.gameObject.SetActive(WorkshopLevelManager.Instance.IsPlaytestActive());

            m_confirmExitGameTextObject.SetActive(false);
            m_confirmMainMenuTextObject.SetActive(false);
            m_confirmExitGameButton.gameObject.SetActive(false);
            m_confirmMainMenuButton.gameObject.SetActive(false);
            m_exitGameButton.gameObject.SetActive(true);
            m_mainMenuButton.gameObject.SetActive(true);
        }

        private void refreshPlayers()
        {
            bool isInMultiplayer = GameModeManager.IsMultiplayer();
            m_playerListOldObject.SetActive(isInMultiplayer);
            if (!isInMultiplayer)
                return;

            if (m_playerDisplayContainerOld.childCount != 0)
                TransformUtils.DestroyAllChildren(m_playerDisplayContainerOld);

            MultiplayerPlayerInfoManager multiplayerPlayerInfoManager = MultiplayerPlayerInfoManager.Instance;
            if (!multiplayerPlayerInfoManager)
                return;

            var infoStates = multiplayerPlayerInfoManager.GetAllPlayerInfoStates();
            if (infoStates.IsNullOrEmpty())
                return;

            bool canShowWins = BattleRoyaleManager.Instance;

            int index = -1;
            foreach(var infoState in infoStates)
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

                ModdedObject playerDisplay = Instantiate(m_playerDisplayOld, m_playerDisplayContainerOld);
                playerDisplay.gameObject.SetActive(true);
                playerDisplay.GetObject<Text>(0).text = playerInfoState.DisplayName;
                Text countLabel = playerDisplay.GetObject<Text>(1);
                if (canShowWins)
                {
                    countLabel.color = ModParseUtils.TryParseToColor("BFBFBF", Color.gray);
                    countLabel.text = playerInfoState.LastBotStandingWins.ToString();
                }
                else
                {
                    countLabel.color = ModParseUtils.TryParseToColor("F84141", Color.red);
                    countLabel.text = playerInfoState.Kills.ToString();
                }
                playerDisplay.GetObject<GameObject>(3).SetActive(index % 2 == 0);
            }
        }

        private void refreshWorkshopPanel()
        {
            SteamWorkshopItem item = WorkshopLevelManager.Instance.GetCurrentLevelWorkshopItem();
            bool shouldShowPanel = item != null;

            bool lostContentModEnabled = ModSpecialUtils.IsModEnabled("cool-hidden-content");
            Vector2 anchoredPosition = m_workshopPanelTransform.anchoredPosition;
            anchoredPosition.y = lostContentModEnabled ? 50f : 15f;
            m_workshopPanelTransform.anchoredPosition = anchoredPosition;
            m_workshopPanelObject.SetActive(shouldShowPanel);

            if (!shouldShowPanel)
                return;

            ulong itemId = (ulong)item.WorkshopItemID;
            if (itemId != m_refreshedWorkshopPanelForItem)
            {
                m_refreshedWorkshopPanelForItem = itemId;

                m_workshopLevelTitleText.text = item.Title;
                if (!item.CreatorName.IsNullOrEmpty() && item.CreatorName != "[unknown]")
                    m_workshopLevelCreatorText.text = $"By {item.CreatorName}";
                else
                    m_workshopLevelCreatorText.text = $"By {item.CreatorID}";

                m_workshopLevelUpVoteButton.gameObject.SetActive(false);
                m_workshopLevelDownVoteButton.gameObject.SetActive(false);

                ModSteamUGCUtils.GetUserVote(item.WorkshopItemID, delegate (WorkshopItemVote workshopItemVote)
                {
                    SteamWorkshopItem item2 = WorkshopLevelManager.Instance.GetCurrentLevelWorkshopItem();
                    if (item != item2)
                        return;

                    m_workshopLevelUpVoteButton.gameObject.SetActive(true);
                    m_workshopLevelDownVoteButton.gameObject.SetActive(true);

                    if (!workshopItemVote.HasVoted)
                    {
                        m_workshopLevelUpVoteButton.interactable = true;
                        m_workshopLevelDownVoteButton.interactable = true;
                        return;
                    }
                    m_workshopLevelUpVoteButton.interactable = !workshopItemVote.VoteValue;
                    m_workshopLevelDownVoteButton.interactable = workshopItemVote.VoteValue;
                });
            }
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

        public void OnGiveFeedbackButtonClicked()
        {
            ModUIConstants.ShowFeedbackUIRework(false);
        }

        public void OnMainMenuButtonClicked()
        {
            if (GameModeManager.Is((GameMode)2500) || (GameModeManager.IsInLevelEditor() && LevelEditorDataManager.Instance.CurrentLevelNeedsSaving()))
            {
                ModUIUtils.MessagePopup(true, "Exit editor?", "Make sure you have saved your progress.", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes, exit", "No", null, SceneTransitionManager.Instance.DisconnectAndExitToMainMenu, null);
                return;
            }

            m_confirmExitGameTextObject.SetActive(false);
            m_confirmMainMenuTextObject.SetActive(true);
            m_confirmExitGameButton.gameObject.SetActive(false);
            m_confirmMainMenuButton.gameObject.SetActive(true);
            m_mainMenuButton.gameObject.SetActive(false);
            m_exitGameButton.gameObject.SetActive(true);
        }

        public void OnConfirmMainMenuButtonClicked()
        {
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }

        public void OnExitGameButtonClicked()
        {
            if (GameModeManager.Is((GameMode)2500) || (GameModeManager.IsInLevelEditor() && LevelEditorDataManager.Instance.CurrentLevelNeedsSaving()))
            {
                ModUIUtils.MessagePopup(true, "Exit editor?", "Make sure you have saved your progress.", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes, exit", "No", null, Application.Quit, null);
                return;
            }

            m_confirmExitGameTextObject.SetActive(true);
            m_confirmMainMenuTextObject.SetActive(false);
            m_confirmExitGameButton.gameObject.SetActive(true);
            m_confirmMainMenuButton.gameObject.SetActive(false);
            m_mainMenuButton.gameObject.SetActive(true);
            m_exitGameButton.gameObject.SetActive(false);
        }

        public void OnConfirmExitGameButtonClicked()
        {
            Application.Quit();
        }

        public void OnReturnToLevelEditorButtonClicked()
        {
            GameUIRoot.Instance.EscMenu.OnBackToLevelEditorButtonClicked();
        }

        public void OnStartMatchButtonClicked()
        {
            GameUIRoot.Instance.EscMenu.OnStartBattleRoyaleLevelClicked();
        }

        public void OnSkipLevelButtonClicked()
        {
            Hide();
            GameUIRoot.Instance.EscMenu.OnSkipWorkshopLevelClicked();
        }

        public void OnLegacyUIButtonClicked()
        {
            Hide();
            ModUIUtils.ShowVanillaEscMenu();
        }

        public void OnWorkshopLevelUpVoteButtonClicked()
        {

        }

        public void OnWorkshopLevelDownVoteButtonClicked()
        {

        }

        public void OnWorkshopLevelInfoButtonClicked()
        {

        }
    }
}
