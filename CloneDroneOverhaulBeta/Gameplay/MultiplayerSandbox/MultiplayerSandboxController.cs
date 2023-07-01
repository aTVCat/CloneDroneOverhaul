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
        public MultiplayerSandboxLobby Lobby;

        public override void Initialize()
        {
        }

        public void CreateLobby()
        {
            SteamAPICall_t apiCall = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 10);

            CallResult<LobbyCreated_t> apiCallResult = CallResult<LobbyCreated_t>.Create(delegate (LobbyCreated_t t, bool a)
            {
                if(t.m_eResult != EResult.k_EResultOK)
                {
                    Debug.LogWarning("[CDO_MS] Cannot create lobby!");
                    return;
                }

                Lobby = new MultiplayerSandboxLobby
                {
                    LobbyID = (CSteamID)t.m_ulSteamIDLobby
                };
                SteamMatchmaking.SetLobbyData(Lobby.LobbyID, "modVer", OverhaulVersion.ModVersion.ToString());
                Debug.Log("[CDO_MS] Created new lobby!");
            });
            apiCallResult.Set(apiCall);
        }

        public void JoinLobby(CSteamID lobbyId)
        {
            SteamAPICall_t apiCall = SteamMatchmaking.JoinLobby(lobbyId);

            CallResult<LobbyEnter_t> apiCallResult = CallResult<LobbyEnter_t>.Create(delegate (LobbyEnter_t t, bool a)
            {
                if (t.m_bLocked)
                {
                    Debug.LogWarning("[CDO_MS] User is blocked!");
                    return;
                }

                Lobby = new MultiplayerSandboxLobby
                {
                    LobbyID = (CSteamID)t.m_ulSteamIDLobby
                };
                Debug.Log("[CDO_MS] Joined the lobby!");
            });
            apiCallResult.Set(apiCall);
        }

        public void JoinLobby(ulong lobbyId) => JoinLobby((CSteamID)lobbyId);
    }
}
