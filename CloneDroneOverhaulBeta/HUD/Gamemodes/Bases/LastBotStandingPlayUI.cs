using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class LastBotStandingPlayUI : OverhaulGamemodeUIBase
    {
        private Button m_GoBackButton;

        private Button m_PlayButton;
        private Button m_PlaySandboxButton;

        private Toggle m_RelayToggle;

        protected override void OnInitialize()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_GoBackButton = moddedObject.GetObject<Button>(0);
            m_GoBackButton.onClick.AddListener(goBackToGamemodeSelection);
            m_PlayButton = moddedObject.GetObject<Button>(1);
            m_PlayButton.onClick.AddListener(OnPlayClicked);
            m_PlaySandboxButton = moddedObject.GetObject<Button>(3);
            //m_PlaySandboxButton.onClick.AddListener(OnSandboxClick);
            m_RelayToggle = moddedObject.GetObject<Toggle>(6);
            m_RelayToggle.onValueChanged.AddListener(OnRelayToggleClick);
        }

        protected override void OnShow()
        {
            GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/LBSInviteBG_" + UnityEngine.Random.Range(1, 5) + ".jpg");
            m_RelayToggle.isOn = OverhaulCore.IsRelayConnectionEnabled;
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

            GamemodesUI.FullscreenWindow.Show(OnPublicMatchClick, 1);
        }

        public void OnPrivateMatchClick()
        {
            BoltGlobalEventListenerSingleton<MultiplayerMatchmakingManager>.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = GameRequestType.BattleRoyaleInviteCodeCreate
            });
            Hide();
        }

        public void OnPublicMatchClick()
        {
            MultiplayerMatchmakingManager.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = GameRequestType.RandomBattleRoyale
            });
            Hide();
        }

        public void OnSandboxClick()
        {
            GamemodesUI.FullscreenWindow.Show(OnPublicMatchClick, 4);
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
