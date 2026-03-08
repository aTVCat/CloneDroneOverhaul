using OverhaulMod.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDuelInviteMenuRework : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElement("LBSLogo")]
        private readonly GameObject _battleRoyaleLogoObject;

        [UIElement("GenericHeader")]
        private readonly GameObject _genericHeaderObject;

        [UIElement("GenericHeader")]
        private readonly Text _genericHeaderText;

        [UIElement("GenericDescription")]
        private readonly Text _genericDescriptionText;

        [UIElement("RulesBox")]
        private readonly GameObject _rulesBoxObject;

        [UIElementAction(nameof(OnRulesButtonClicked))]
        [UIElement("RulesButton")]
        private readonly Button _rulesButton;

        [UIElementAction(nameof(OnPlayButtonClicked))]
        [UIElement("PlayPublicMatch")]
        private readonly Button _playPublicMatchButton;

        [UIElementAction(nameof(OnCreateMatchButtonClicked))]
        [UIElement("CreatePrivateMatchButton")]
        private readonly Button _createPrivateMatchButton;

        [UIElementAction(nameof(OnJoinMatchButtonClicked))]
        [UIElement("JoinPrivateMatchButton")]
        private readonly Button _joinPrivateMatchButton;

        [UIElementAction(nameof(OnAutoBuildConfigButtonClicked))]
        [UIElement("AutoBuildConfigButton", false)]
        private readonly Button _autoBuildConfigButton;

        [UIElement("JoinBox", false)]
        private readonly GameObject _joinBoxObject;

        [UIElementAction(nameof(OnCancelJoinButtonClicked))]
        [UIElement("CancelButton")]
        private readonly Button _cancelButton;

        [UIElement("CodeField")]
        private readonly InputField _codeField;

        [UIElementAction(nameof(OnGoButtonClicked))]
        [UIElement("GoButton")]
        private readonly Button _goButton;

        [UIElement("LBSStatsBox", typeof(UIElementBattleRoyaleStatsBox))]
        private readonly UIElementBattleRoyaleStatsBox _battleRoyaleStatsBox;

        [UIElement("Panel")]
        private readonly GameObject _panelObject;

        [UIElement("GarbageBotSkinDropdown")]
        private readonly GameObject _garbageBotSkinDropdownObject;

        [UIElementAction(nameof(OnGarbageBotSkinDropdownValueChanged))]
        [UIElement("GarbageBotSkinDropdown")]
        private readonly Dropdown _garbageBotSkinDropdown;

        [UIElement("GarbageBotSkinDropdownOverlay")]
        private readonly GameObject _garbageBotSkinDropdownOverlayObject;

        public override bool refreshOnlyCursor => true;

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

        private IEnumerator waitThenRefreshGarbageBotSkinDropdownCoroutine()
        {
            _garbageBotSkinDropdown.interactable = false;
            _garbageBotSkinDropdownOverlayObject.SetActive(true);
            while (!ModIntegrationUtils.SelectGarbageBotSkin.HasLocalPlayerStats())
            {
                yield return null;
            }
            _garbageBotSkinDropdown.interactable = true;
            _garbageBotSkinDropdownOverlayObject.SetActive(false);
            refreshGarbageBotSkinDropdown();
            yield break;
        }

        private void refreshGarbageBotSkinDropdown()
        {
            List<Dropdown.OptionData> garbageBotSkinOptions = ModIntegrationUtils.SelectGarbageBotSkin.GetGarbageBotSkinOptions();
            _garbageBotSkinDropdown.options = garbageBotSkinOptions;
            int selectedGarbageBotSkinIndex = ModIntegrationUtils.SelectGarbageBotSkin.selectedGarbageBotSkinIndex;
            int value = 0;
            for (int i = 0; i < garbageBotSkinOptions.Count; i++)
            {
                if ((garbageBotSkinOptions[i] as DropdownIntOptionData).IntValue == selectedGarbageBotSkinIndex)
                {
                    value = i;
                    break;
                }
            }
            _garbageBotSkinDropdown.value = value;
        }

        public void Populate(GameMode gameMode)
        {
            displayingGameMode = gameMode;

            _genericHeaderObject.SetActive(gameMode != GameMode.BattleRoyale);
            _battleRoyaleLogoObject.SetActive(gameMode == GameMode.BattleRoyale);
            _rulesBoxObject.SetActive(gameMode == GameMode.BattleRoyale);
            _battleRoyaleStatsBox.gameObject.SetActive(gameMode == GameMode.BattleRoyale);
            _autoBuildConfigButton.gameObject.SetActive(gameMode == GameMode.BattleRoyale);
            _joinBoxObject.SetActive(false);
            _codeField.text = string.Empty;
            _playPublicMatchButton.gameObject.SetActive(gameMode != GameMode.MultiplayerDuel || ModSpecialUtils.IsModEnabled("3cfnb387n78eg"));
            _garbageBotSkinDropdown.gameObject.SetActive(gameMode == GameMode.BattleRoyale && ModIntegrationUtils.SelectGarbageBotSkin.IsModAvailable());
            if (_garbageBotSkinDropdownObject.activeSelf)
            {
                _ = base.StartCoroutine(waitThenRefreshGarbageBotSkinDropdownCoroutine());
            }

            switch (gameMode)
            {
                case GameMode.MultiplayerDuel:
                    _genericHeaderText.text = LocalizationManager.Instance.GetTranslatedString("Duels");
                    _genericDescriptionText.text = LocalizationManager.Instance.GetTranslatedString("Challenge your friend!");
                    break;
                case GameMode.CoopChallenge:
                    _genericHeaderText.text = LocalizationManager.Instance.GetTranslatedString("Co-op Challenges");
                    _genericDescriptionText.text = LocalizationManager.Instance.GetTranslatedString("Tackle Challenges with other humans!");
                    break;
                case GameMode.EndlessCoop:
                    _genericHeaderText.text = LocalizationManager.Instance.GetTranslatedString("Endless Co-op");
                    _genericDescriptionText.text = LocalizationManager.Instance.GetTranslatedString("Survive the arena with other humans!");
                    break;
            }
        }

        private async void joinMatchFunction()
        {
            if (_codeField.text.Length != _codeField.characterLimit || !MultiplayerLoginManager.Instance.IsLoggedIntoPlayfab())
                return;

            _goButton.interactable = false;
            if (ExperimentalBranchManager.Instance.UseGameye)
                MultiplayerMatchmakingManager.Instance.Matchmaking20Private.StopSearching(false);

            string inviteCodeToJoin = _codeField.text.ToUpper().Trim();

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
                _goButton.interactable = true;
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
                _goButton.interactable = true;
            });
        }

        public void OnGarbageBotSkinDropdownValueChanged(int value)
        {
            ModIntegrationUtils.SelectGarbageBotSkin.selectedGarbageBotSkinIndex = (_garbageBotSkinDropdown.options[value] as DropdownIntOptionData).IntValue;
        }

        public void OnPlayButtonClicked()
        {
            if (displayingGameMode == GameMode.CoopChallenge)
            {
                Hide();
                _ = ModUIConstants.ShowChallengesMenuRework(true, false);
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
                _ = ModUIConstants.ShowChallengesMenuRework(true, true);
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
            _joinBoxObject.SetActive(true);
        }

        public void OnAutoBuildConfigButtonClicked()
        {
            GameObject gameObject = _panelObject;
            gameObject.SetActive(false);
            UIAutoBuildMenu ui = ModUIConstants.ShowAutoBuildMenu();
            ui.objectToShow = gameObject;
        }

        public void OnCancelJoinButtonClicked()
        {
            _joinBoxObject.SetActive(false);
        }

        public void OnGoButtonClicked()
        {
            joinMatchFunction();
        }

        public void OnRulesButtonClicked()
        {
            Application.OpenURL("https://support.doborog.com/l/en/clone-drone/last-bot-standing");
        }
    }
}
