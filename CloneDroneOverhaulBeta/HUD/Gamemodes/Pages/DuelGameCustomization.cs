using ModLibrary;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class DuelGameCustomization : FullscreenWindowPageBase
    {
        [ObjectReference("Start")]
        private Button m_Start;

        public override Vector2 GetWindowSize() => OverhaulGamemodesUIFullscreenWindow.GameCustomizationWindowSize;

        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);
            OverhaulUIVer2.FillVariables(this);

            m_Start.AddOnClickListener(OnStartClicked);
        }

        public void OnStartClicked()
        {
            BoltGlobalEventListenerSingleton<MultiplayerMatchmakingManager>.Instance.FindAndJoinMatch(new GameRequest
            {
                GameType = GameRequestType.DuelInviteCodeCreate
            });
            FullscreenWindow.GamemodesUI.Hide();
        }
    }
}
