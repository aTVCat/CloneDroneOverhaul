using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class LastBotStandingPlayUI : OverhaulGamemodeUIBase
    {
        private Button m_GoBackButton;

        private Button m_PlayButton;
        private Button m_PlayPrivateButton;

        private readonly Button m_RulesButton;
        private Button m_PrevMatchesButton;

        private Toggle m_RelayToggle;

        protected override void OnInitialize()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_GoBackButton = moddedObject.GetObject<Button>(0);
            m_GoBackButton.AddOnClickListener(goBackToGamemodeSelection);
            m_PlayButton = moddedObject.GetObject<Button>(1);
            m_PlayButton.AddOnClickListener(OnPlayClicked);
            m_PlayPrivateButton = moddedObject.GetObject<Button>(7);
            m_PlayPrivateButton.AddOnClickListener(OnPrivateMatchClick);
            m_RelayToggle = moddedObject.GetObject<Toggle>(6);
            m_RelayToggle.onValueChanged.AddListener(OnRelayToggleClick);
            m_PrevMatchesButton = moddedObject.GetObject<Button>(5);
            m_PrevMatchesButton.AddOnClickListener(OnPrevMatchesClick);
        }

        protected override void OnShow()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/LBSInviteBG_" + UnityEngine.Random.Range(1, 5) + ".jpg");
            m_RelayToggle.isOn = OverhaulCore.IsRelayConnectionEnabled;
            GameUIRoot.Instance.TitleScreenUI.SetMultiplayerPlayerModeSelectButtonsVisibile(false);
            moddedObject.GetObject<CanvasGroup>(11).alpha = 0.2f;

            if (!PlayFabPlayerStatsManager.Instance || !OverhaulPlayerIdentifier.HasPlayfabID())
                return;

            PlayFabPlayerStats localPlayerStats = PlayFabPlayerStatsManager.Instance.GetLocalPlayerStats();
            if (localPlayerStats == null)
                return;

            int lastBotStandingWins = localPlayerStats.LastBotStandingWins;
            moddedObject.GetObject<CanvasGroup>(11).alpha = 1f;
            moddedObject.GetObject<Text>(9).text = lastBotStandingWins.ToString();
            BattleRoyaleGarbageBotPerWin garbageBotInfo = BattleRoyaleGarbageBotCustomizationManager.Instance.GetGarbageBotInfo(lastBotStandingWins);
            moddedObject.GetObject<Image>(8).sprite = garbageBotInfo.PreviewImage;
            BattleRoyaleGarbageBotPerWin upcomingGarbageBotInfo = BattleRoyaleGarbageBotCustomizationManager.Instance.GetUpcomingGarbageBotInfo(lastBotStandingWins);
            if (upcomingGarbageBotInfo != null)
            {
                moddedObject.GetObject<Text>(10).text = upcomingGarbageBotInfo.MinWins.ToString();
                return;
            }
            moddedObject.GetObject<Text>(10).text = "-";
        }

        private void goBackToGamemodeSelection()
        {
            Hide();
            GameUIRoot.Instance.TitleScreenUI.SetMultiplayerPlayerModeSelectButtonsVisibile(true);
        }

        public void OnPlayClicked()
        {
            if (ExperimentalBranchManager.Instance.UseGameye)
                MultiplayerMatchmakingManager.Instance.Matchmaking20Private.StopSearching(false);

            MultiplayerMatchmakingManager.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = GameRequestType.RandomBattleRoyale
            });
            Hide();
        }

        public void OnPrivateMatchClick()
        {
            GamemodesUI.FullscreenWindow.Show(null, 1);
        }

        public void OnPrevMatchesClick()
        {
            OverhaulFullscreenDialogueWindow.ShowUnfinishedFeatureWindow();
        }

        public void OnRelayToggleClick(bool newValue)
        {
            SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Gameplay.Multiplayer.Relay Connection", true), newValue);
        }

        private void Update()
        {
            base.Update();

            if (GamemodesUI.FullscreenWindow.IsActive)
                return;

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnPlayClicked();

            if (Input.GetKeyDown(KeyCode.P))
                OnPrivateMatchClick();

            if (Input.GetKeyDown(KeyCode.Backspace))
                goBackToGamemodeSelection();
        }
    }
}
