using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class LBSGameCustomization : FullscreenWindowPageBase
    {
        private Button m_StartButton;

        public override Vector2 GetWindowSize() => OverhaulGamemodesUIFullscreenWindow.GameCustomizationWindowSize;

        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            m_StartButton = MyModdedObject.GetObject<Button>(5);
            m_StartButton.onClick.AddListener(OnStartClicked);
        }

        public void OnStartClicked()
        {
            BoltGlobalEventListenerSingleton<MultiplayerMatchmakingManager>.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = GameRequestType.BattleRoyaleInviteCodeCreate
            });
            FullscreenWindow.GamemodesUI.Hide();
        }
    }
}
