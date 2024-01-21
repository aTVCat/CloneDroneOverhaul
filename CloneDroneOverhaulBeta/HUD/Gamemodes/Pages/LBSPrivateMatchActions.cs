using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class LBSPrivateMatchActions : FullscreenWindowPageBase
    {
        private Button m_PublicButton;
        private Button m_PrivateButton;
        private Button m_JoinLobby;

        public override Vector2 GetWindowSize() => new Vector3(300f, 125f);

        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            m_PublicButton = MyModdedObject.GetObject<Button>(0);
            m_PublicButton.onClick.AddListener(OnPublicClick);
            m_PrivateButton = MyModdedObject.GetObject<Button>(1);
            m_PrivateButton.onClick.AddListener(OnPrivateClick);
            m_JoinLobby = MyModdedObject.GetObject<Button>(2);
            m_JoinLobby.onClick.AddListener(OnJoinLobbyClick);
        }

        public void OnPrivateClick()
        {
            BoltGlobalEventListenerSingleton<MultiplayerMatchmakingManager>.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = GameRequestType.BattleRoyaleInviteCodeCreate
            });
            FullscreenWindow.GamemodesUI.Hide();
            //FullscreenWindow.GoToPage(2);
        }

        public void OnJoinLobbyClick()
        {
            FullscreenWindow.GoToPage(3);
        }

        public void OnPublicClick()
        {
            FullscreenWindow.DoQuickStart();
        }
    }
}
