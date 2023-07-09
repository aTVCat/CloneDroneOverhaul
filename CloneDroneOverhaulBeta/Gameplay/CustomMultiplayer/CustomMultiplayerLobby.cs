using Steamworks;

namespace CDOverhaul.CustomMultiplayer
{
    public class CustomMultiplayerLobby
    {
        public CSteamID LobbyID
        {
            get;
            set;
        }

        public CSteamID GetLobbyOwner() => SteamMatchmaking.GetLobbyOwner(LobbyID);
        public CSteamID GetLobbyMember(int index) => SteamMatchmaking.GetLobbyMemberByIndex(LobbyID, index);

        public int GetMemberCount() => SteamMatchmaking.GetNumLobbyMembers(LobbyID);

        public string[] GetLobbyMemberUsernames()
        {
            int memberCount = GetMemberCount();
            string[] result = new string[memberCount];

            int index = 0;
            do
            {
                CSteamID userID = GetLobbyMember(index);
                result[index] = SteamFriends.GetFriendPersonaName(userID);
                index++;
            } while (index < memberCount);

            return result;
        }

        public CSteamID[] GetLobbyMemberSteamIDs()
        {
            int memberCount = GetMemberCount();
            CSteamID[] result = new CSteamID[memberCount];

            int index = 0;
            do
            {
                result[index] = GetLobbyMember(index);
                index++;
            } while (index < memberCount);

            return result;
        }
    }
}
