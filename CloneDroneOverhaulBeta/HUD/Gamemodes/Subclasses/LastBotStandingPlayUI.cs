using System;
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

            MultiplayerMatchmakingManager.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = GameRequestType.RandomBattleRoyale
            });
            GamemodesUI.Hide();
        }

        public void OnCreatePrivateMatchClicked()
        {
            BoltGlobalEventListenerSingleton<MultiplayerMatchmakingManager>.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = GameRequestType.BattleRoyaleInviteCodeCreate
            });
            GamemodesUI.Hide();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnPlayClicked();

            if (Input.GetKeyDown(KeyCode.P))
                OnCreatePrivateMatchClicked();
        }
    }
}
