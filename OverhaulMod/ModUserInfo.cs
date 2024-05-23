using Steamworks;

namespace OverhaulMod
{
    public static class ModUserInfo
    {
        public static readonly Steamworks.CSteamID DeveloperSteamID = new Steamworks.CSteamID(76561199028311109);
        public static readonly string DeveloperPlayFabID = "883CC7F4CA3155A3";

        private static CSteamID s_localPlayerSteamID;
        public static CSteamID localPlayerSteamID
        {
            get
            {
                if (s_localPlayerSteamID == default)
                {
                    SteamManager steamManager = SteamManager.Instance;
                    if (!steamManager || !steamManager.Initialized)
                        return default;

                    s_localPlayerSteamID = SteamUser.GetSteamID();
                }
                return s_localPlayerSteamID;
            }
        }

        private static string s_localPlayerPlayFabID;
        public static string localPlayerPlayFabID
        {
            get
            {
                if (s_localPlayerPlayFabID == null)
                {
                    MultiplayerLoginManager multiplayerLoginManager = MultiplayerLoginManager.Instance;
                    if (!multiplayerLoginManager || !multiplayerLoginManager.IsLoggedIntoPlayfab())
                        return null;

                    s_localPlayerPlayFabID = multiplayerLoginManager.GetLocalPlayFabID();
                }
                return s_localPlayerPlayFabID;
            }
        }

        public static bool isDeveloper
        {
            get
            {
                return localPlayerSteamID == DeveloperSteamID || localPlayerPlayFabID == DeveloperPlayFabID;
            }
        }
    }
}
