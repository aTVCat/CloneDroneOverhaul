//#define FORCE_DEVELOPER
//#define FORCE_NOT_DEVELOPER

using OverhaulMod.Utils;
using Steamworks;
using System.IO;

namespace OverhaulMod
{
    public static class ModUserInfo
    {
        public const string FILE_NAME = "SavedUserInfo.json";

        public static readonly CSteamID DeveloperSteamID = new CSteamID(76561199028311109);
        public static readonly string DeveloperPlayFabID = "883CC7F4CA3155A3";

        private static Info s_info;

        public static void Load()
        {
            Info info;
            try
            {
                info = ModJsonUtils.DeserializeStream<Info>(Path.Combine(ModCore.modUserDataFolder, FILE_NAME));
            }
            catch
            {
                info = null;
            }
        }

        public static void Save()
        {
            Info info = s_info;
            if (info == null)
            {
                info = new Info();
                s_info = info;
            }

            bool hasChanged = false;
            if ((info.PlayfabID.IsNullOrEmpty() || info.PlayfabID != localPlayerPlayFabID) && !localPlayerPlayFabID.IsNullOrEmpty())
            {
                info.PlayfabID = localPlayerPlayFabID;
                hasChanged = true;
            }

            if ((info.SteamID == default || info.SteamID != (ulong)localPlayerSteamID) && (ulong)localPlayerSteamID != default)
            {
                info.SteamID = (ulong)localPlayerSteamID;
                hasChanged = true;
            }

            if (hasChanged)
                ModJsonUtils.WriteStream(Path.Combine(ModCore.modUserDataFolder, FILE_NAME), s_info);
        }

        private static CSteamID s_localPlayerSteamID;
        public static CSteamID localPlayerSteamID
        {
            get
            {
                if (s_localPlayerSteamID == default)
                {
                    SteamManager steamManager = SteamManager.Instance;
                    if (!steamManager || !steamManager.Initialized)
                        return s_info != null ? (CSteamID)s_info.SteamID : default;

                    s_localPlayerSteamID = SteamUser.GetSteamID();
                    Save();
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
                        return s_info != null ? s_info.PlayfabID : null;

                    s_localPlayerPlayFabID = multiplayerLoginManager.GetLocalPlayFabID();
                    Save();
                }
                return s_localPlayerPlayFabID;
            }
        }

        public static bool isDeveloper
        {
            get
            {
#if FORCE_NOT_DEVELOPER
                return false;
#elif FORCE_DEVELOPER
                return true;
#else
                return localPlayerSteamID == DeveloperSteamID || localPlayerPlayFabID == DeveloperPlayFabID;
#endif
            }
        }

        public class Info
        {
            public string PlayfabID;

            public ulong SteamID;
        }
    }
}
