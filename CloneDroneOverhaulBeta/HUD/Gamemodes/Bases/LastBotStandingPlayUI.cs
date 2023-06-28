using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class LastBotStandingPlayUI : OverhaulGamemodeUIBase
    {
        private Button m_GoBackButton;

        private Button m_PlayButton;

        protected override void OnInitialize()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_GoBackButton = moddedObject.GetObject<Button>(0);
            m_GoBackButton.onClick.AddListener(goBackToGamemodeSelection);
            m_PlayButton = moddedObject.GetObject<Button>(1);
            m_PlayButton.onClick.AddListener(OnPlayClicked);
        }

        protected override void OnShow()
        {
            GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/LBSInviteBG_" + UnityEngine.Random.Range(1, 5) + ".jpg");
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

        private void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnPlayClicked();

            if (Input.GetKeyDown(KeyCode.P))
                OnPrivateMatchClick();

            if (Input.GetKeyDown(KeyCode.Backspace))
                goBackToGamemodeSelection();
        }
    }
}
