using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Steamworks;
using UnityEngine;

namespace CDOverhaul.MultiplayerSandbox
{
    public class MultiplayerSandboxController : OverhaulController
    {
        public const string LobbyJoinFailEvent = "OverhaulMultiplayer.LobbyJoinFail";

        public static MultiplayerSandboxController Instance;

        public MultiplayerSandboxLobby Lobby;
        public MultiplayerSandboxWorld World;

        public static bool FullInitialization => Instance && Instance.Lobby != null && Instance.World != null;

        public override void Initialize()
        {
            Instance = this;
        }

        private void FixedUpdate()
        {
            MultiplayerSandboxNetworking.FixedFrames++;
        }

        private void LateUpdate()
        {
            if (!FullInitialization)
                return;

            MultiplayerSandboxNetworking.HandleIncomingPackets();
            if (Input.GetKeyDown(KeyCode.I))
            {
                Lobby.LobbyID.ToString().CopyToClipboard();
            }
        }

        /// <summary>
        /// Create a lobby
        /// </summary>
        public void CreateLobby(bool isPublic = false, int maxPlayers = 10)
        {
            SteamAPICall_t apiCall = SteamMatchmaking.CreateLobby(isPublic ? ELobbyType.k_ELobbyTypePublic : ELobbyType.k_ELobbyTypeFriendsOnly, maxPlayers);

            CallResult<LobbyCreated_t> apiCallResult = CallResult<LobbyCreated_t>.Create(delegate (LobbyCreated_t t, bool a)
            {
                if(t.m_eResult != EResult.k_EResultOK)
                {
                    Debug.LogWarning("[CDO_MS] Cannot create lobby!");
                    return;
                }
                SteamMatchmaking.SetLobbyData(Lobby.LobbyID, "modVer", OverhaulVersion.ModVersion.ToString());

                InitializeLobbyData((CSteamID)t.m_ulSteamIDLobby);
                InitializeWorld();
                Debug.Log("[CDO_MS] Created new lobby!");
            });
            apiCallResult.Set(apiCall);
        }

        /// <summary>
        /// Join specific lobby
        /// </summary>
        /// <param name="lobbyId"></param>
        public void JoinLobby(CSteamID lobbyId)
        {
            SteamAPICall_t apiCall = SteamMatchmaking.JoinLobby(lobbyId);

            CallResult<LobbyEnter_t> apiCallResult = CallResult<LobbyEnter_t>.Create(delegate (LobbyEnter_t t, bool a)
            {
                if(t.m_ulSteamIDLobby == (ulong)CSteamID.Nil || (EChatRoomEnterResponse)t.m_EChatRoomEnterResponse != EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                {
                    Debug.LogWarning("[CDO_MS] Cannot join the lobby!");
                    OverhaulEventsController.DispatchEvent(LobbyJoinFailEvent);
                    return;
                }

                string version = SteamMatchmaking.GetLobbyData((CSteamID)t.m_ulSteamIDLobby, "modVer");
                if(version != OverhaulVersion.ModVersion.ToString())
                {
                    LeaveLobby((CSteamID)t.m_ulSteamIDLobby);
                    return;
                }

                InitializeLobbyData((CSteamID)t.m_ulSteamIDLobby);
                InitializeWorld();
                Debug.Log("[CDO_MS] Joined the lobby!");
            });
            apiCallResult.Set(apiCall);
        }

        public void LeaveLobby(CSteamID lobbyId)
        {
            SteamMatchmaking.LeaveLobby(lobbyId);

            Lobby = null;
            World = null;
        }

        public void InitializeLobbyData(CSteamID lobbyId)
        {
            if (Lobby != null)
                return;

            Lobby = new MultiplayerSandboxLobby
            {
                LobbyID = lobbyId
            };
        }

        public void InitializeWorld()
        {
            World = new MultiplayerSandboxWorld();
            World.StartGameInUsualWorld();
        }
    }
}
