using UnityEngine;

namespace CDOverhaul.HUD.Gamemodes
{
    public class LastBotStandingPlayUI : OverhaulGamemodeUIBase
    {
        protected override void OnShow()
        {
            GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/LBSInviteBG_" + UnityEngine.Random.Range(1, 5) + ".jpg");
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
        }
    }
}
