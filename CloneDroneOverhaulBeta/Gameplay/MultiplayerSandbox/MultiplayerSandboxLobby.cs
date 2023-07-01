using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.MultiplayerSandbox
{
    public class MultiplayerSandboxLobby
    {
        public CSteamID LobbyID;

        public string[] GetUsernamesInLobby()
        {
            int memberCount = SteamMatchmaking.GetNumLobbyMembers(LobbyID);
            string[] result = new string[memberCount];

            int index = 0;
            do
            {
                CSteamID userID = SteamMatchmaking.GetLobbyMemberByIndex(LobbyID, index);
                result[index] = SteamFriends.GetFriendPersonaName(userID);
                index++;
            } while (index < memberCount);

            return result;
        }
    }
}
