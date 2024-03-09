using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDuelInviteMenuRework : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("LBSLogo")]
        private readonly GameObject m_battleRoyaleLogoObject;

        [UIElement("GenericHeader")]
        private readonly GameObject m_genericHeaderObject;

        [UIElement("GenericHeader")]
        private readonly Text m_genericHeaderText;

        [UIElement("GenericDescription")]
        private readonly Text m_genericDescriptionText;

        [UIElement("RulesBox")]
        private readonly GameObject m_rulesBoxObject;

        [UIElementAction(nameof(OnRulesButtonClicked))]
        [UIElement("RulesButton")]
        private readonly Button m_rulesButton;

        [UIElementAction(nameof(OnPlayButtonClicked))]
        [UIElement("PlayPublicMatch")]
        private readonly Button m_playPublicMatchButton;

        [UIElementAction(nameof(OnCreateMatchButtonClicked))]
        [UIElement("CreatePrivateMatchButton")]
        private readonly Button m_createPrivateMatchButton;

        [UIElementAction(nameof(OnJoinMatchButtonClicked))]
        [UIElement("JoinPrivateMatchButton")]
        private readonly Button m_joinPrivateMatchButton;

        [UIElementAction(nameof(OnAutoBuildConfigButtonClicked))]
        [UIElement("AutoBuildConfigButton", false)]
        private readonly Button m_autoBuildConfigButton;

        [UIElement("JoinBox", false)]
        private readonly GameObject m_joinBoxObject;

        [UIElementAction(nameof(OnCancelJoinButtonClicked))]
        [UIElement("CancelButton")]
        private readonly Button m_cancelButton;

        [UIElement("CodeField")]
        private readonly InputField m_codeField;

        [UIElementAction(nameof(OnGoButtonClicked))]
        [UIElement("GoButton")]
        private readonly Button m_goButton;

        [UIElement("LBSStatsBox", typeof(UIElementBattleRoyaleStatsBox))]
        private readonly UIElementBattleRoyaleStatsBox m_battleRoyaleStatsBox;

        [UIElement("Panel")]
        private readonly GameObject m_panelObject;

        public override bool dontRefreshUI => true;

        public GameMode displayingGameMode
        {
            get;
            private set;
        }

        public override void Show()
        {
            base.Show();
            ModCache.titleScreenUI.SetMultiplayerPlayerModeSelectButtonsVisibile(false);
        }

        public override void Hide()
        {
            base.Hide();
            ModCache.titleScreenUI.SetMultiplayerPlayerModeSelectButtonsVisibile(true);
        }

        public void Populate(GameMode gameMode)
        {
            displayingGameMode = gameMode;

            m_genericHeaderObject.SetActive(gameMode != GameMode.BattleRoyale);
            m_battleRoyaleLogoObject.SetActive(gameMode == GameMode.BattleRoyale);
            m_rulesBoxObject.SetActive(gameMode == GameMode.BattleRoyale);
            m_battleRoyaleStatsBox.gameObject.SetActive(gameMode == GameMode.BattleRoyale);
            m_autoBuildConfigButton.gameObject.SetActive(gameMode == GameMode.BattleRoyale);
            m_joinBoxObject.SetActive(false);
            m_codeField.text = string.Empty;

            switch (gameMode)
            {
                case GameMode.MultiplayerDuel:
                    m_genericHeaderText.text = LocalizationManager.Instance.GetTranslatedString("Duels");
                    m_genericDescriptionText.text = LocalizationManager.Instance.GetTranslatedString("Challenge your friend!");
                    break;
                case GameMode.CoopChallenge:
                    m_genericHeaderText.text = LocalizationManager.Instance.GetTranslatedString("Co-op Challenges");
                    m_genericDescriptionText.text = LocalizationManager.Instance.GetTranslatedString("Tackle Challenges with other humans!");
                    break;
                case GameMode.EndlessCoop:
                    m_genericHeaderText.text = LocalizationManager.Instance.GetTranslatedString("Endless Co-op");
                    m_genericDescriptionText.text = LocalizationManager.Instance.GetTranslatedString("Survive the arena with other humans!");
                    break;
            }
        }

        private async void joinMatchFunction()
        {
            if (m_codeField.text.Length != m_codeField.characterLimit || !MultiplayerLoginManager.Instance.IsLoggedIntoPlayfab())
                return;

            m_goButton.interactable = false;
            if (ExperimentalBranchManager.Instance.UseGameye)
                MultiplayerMatchmakingManager.Instance.Matchmaking20Private.StopSearching(false);

            string inviteCodeToJoin = m_codeField.text.ToUpper().Trim();

            GameRequestType gameType = GameRequestType.DuelInviteCodeJoin;
            switch (displayingGameMode)
            {
                case GameMode.BattleRoyale:
                    gameType = GameRequestType.BattleRoyaleInviteCodeJoin;
                    break;
                case GameMode.EndlessCoop:
                    gameType = GameRequestType.CoopInviteCodeJoin;
                    break;
                case GameMode.CoopChallenge:
                    gameType = GameRequestType.CoopChallengeInviteJoin;
                    break;
            }

            DuelInviteMenu._currentCallbackID++;

            Exclusivity exclusivity = await MultiplayerMatchmakingManager.GetExclusivePlatformAsync();
            GameRequest gameRequest = new GameRequest
            {
                GameType = gameType,
                InviteCodeToJoin = inviteCodeToJoin,
                Exclusivity = exclusivity,
            };

            MultiplayerMatchmakingManager.Instance.ResetMatchmakingStartTime();
            CustomMatchmakerClientAPI.Instance.FindMatch(new CustomMatchmakeRequest
            {
                GameRequest = gameRequest
            }, delegate (CustomMatchmakeResult result)
            {
                if (DuelInviteMenu._ignoreCallbackID >= DuelInviteMenu._currentCallbackID)
                    return;

                Hide();
                MultiplayerMatchmakingManager.Instance.ConnectToExternalMatchmakeResult(result, gameRequest);
                m_goButton.interactable = true;
            }, delegate (CustomMatchmakerError error)
            {
                if (DuelInviteMenu._ignoreCallbackID >= DuelInviteMenu._currentCallbackID)
                    return;

                if (error.Type == CustomMatchmakerErrorType.MatchFull)
                {
                    ModUIUtils.MessagePopupOK("Could not join the match", $"Reason: {LocalizedStrings.FULL}", true);
                }
                else if (error.Type == CustomMatchmakerErrorType.InviteMatchFull)
                {
                    ModUIUtils.MessagePopupOK("Could not join the match", $"Reason: {LocalizedStrings.FULL}", true);
                }
                else if (error.Type == CustomMatchmakerErrorType.InviteCodeNotFound)
                {
                    ModUIUtils.MessagePopupOK("Could not join the match", $"Reason: {LocalizedStrings.NOT_FOUND}", true);
                }
                else
                {
                    ModUIUtils.MessagePopupOK("Could not join the match", $"Reason: {StringUtils.AddSpacesToCamelCasedString(error.Type.ToString())}", true);
                }
                m_goButton.interactable = true;
            });
        }

        public void OnPlayButtonClicked()
        {
            if (displayingGameMode == GameMode.CoopChallenge)
            {
                Hide();
                ModUIConstants.ShowChallengesMenuRework(true, false);
                return;
            }

            if (ExperimentalBranchManager.Instance.UseGameye)
                MultiplayerMatchmakingManager.Instance.Matchmaking20Private.StopSearching(false);

            Hide();
            GameRequestType gameType = GameRequestType.RandomDuel;
            switch (displayingGameMode)
            {
                case GameMode.BattleRoyale:
                    gameType = GameRequestType.RandomBattleRoyale;
                    break;
                case GameMode.EndlessCoop:
                    gameType = GameRequestType.RandomEndlessCoop;
                    break;
                case GameMode.CoopChallenge:
                    gameType = GameRequestType.RandomCoopChallenge;
                    break;
            }

            MultiplayerMatchmakingManager.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = gameType
            });
        }

        public void OnCreateMatchButtonClicked()
        {
            if (displayingGameMode == GameMode.CoopChallenge)
            {
                Hide();
                ModUIConstants.ShowChallengesMenuRework(true, true);
                return;
            }

            if (ExperimentalBranchManager.Instance.UseGameye)
                MultiplayerMatchmakingManager.Instance.Matchmaking20Private.StopSearching(false);

            Hide();
            GameRequestType gameType = GameRequestType.DuelInviteCodeCreate;
            switch (displayingGameMode)
            {
                case GameMode.BattleRoyale:
                    gameType = GameRequestType.BattleRoyaleInviteCodeCreate;
                    break;
                case GameMode.EndlessCoop:
                    gameType = GameRequestType.CoopInviteCodeCreate;
                    break;
                case GameMode.CoopChallenge:
                    gameType = GameRequestType.CoopChallengeInviteCreate;
                    break;
            }

            MultiplayerMatchmakingManager.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = gameType
            });
        }

        public void OnJoinMatchButtonClicked()
        {
            m_joinBoxObject.SetActive(true);
        }

        public void OnAutoBuildConfigButtonClicked()
        {
            GameObject gameObject = m_panelObject;
            gameObject.SetActive(false);
            UIAutoBuildMenu ui = ModUIConstants.ShowAutoBuildMenu();
            ui.objectToShow = gameObject;
        }

        public void OnCancelJoinButtonClicked()
        {
            m_joinBoxObject.SetActive(false);
        }

        public void OnGoButtonClicked()
        {
            joinMatchFunction();
        }

        public void OnRulesButtonClicked()
        {
            Application.OpenURL("https://clonedroneinthedangerzone.com/lbsrules");
        }
    }
}
