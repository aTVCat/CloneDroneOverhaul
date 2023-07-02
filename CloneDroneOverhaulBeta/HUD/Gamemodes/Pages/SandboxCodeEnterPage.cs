﻿using CDOverhaul.MultiplayerSandbox;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class SandboxCodeEnterPage : CodeEnterPage
    {
        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            OverhaulEventsController.AddEventListener(MultiplayerSandboxController.LobbyJoinFailEvent, onFailedToJoinLobby);
        }

        protected override void OnDisposed()
        {
            OverhaulEventsController.RemoveEventListener(MultiplayerSandboxController.LobbyJoinFailEvent, onFailedToJoinLobby);
        }

        protected override async void TryJoinLobby()
        {
            FullscreenWindow.GamemodesUI.Hide();

            if(!ulong.TryParse(GetInputFieldValue(), out ulong lobbyId))
            {
                SetInputFieldInteractable(true);
                return;
            }
            MultiplayerSandboxController.Instance.JoinLobby((CSteamID)lobbyId);
        }

        private void onFailedToJoinLobby()
        {
            SetInputFieldInteractable(true);
        }
    }
}
