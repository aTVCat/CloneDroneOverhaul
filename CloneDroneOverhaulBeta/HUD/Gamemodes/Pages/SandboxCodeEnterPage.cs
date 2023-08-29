using CDOverhaul.CustomMultiplayer;
using Steamworks;

namespace CDOverhaul.HUD.Gamemodes
{
    public class SandboxCodeEnterPage : LBSCodeEnterPage
    {
        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            OverhaulEvents.AddEventListener(OverhaulMultiplayerManager.LOBBY_JOIN_FAIL_EVENT, onFailedToJoinLobby);
        }

        protected override void OnDisposed()
        {
            OverhaulEvents.RemoveEventListener(OverhaulMultiplayerManager.LOBBY_JOIN_FAIL_EVENT, onFailedToJoinLobby);
        }

        protected override async void TryJoinLobby()
        {
            FullscreenWindow.GamemodesUI.Hide();

            if (!ulong.TryParse(GetInputFieldValue(), out ulong lobbyId))
            {
                SetInputFieldInteractable(true);
                return;
            }
            OverhaulMultiplayerManager.Instance.JoinLobby((CSteamID)lobbyId);
        }

        private void onFailedToJoinLobby()
        {
            SetInputFieldInteractable(true);
        }
    }
}
