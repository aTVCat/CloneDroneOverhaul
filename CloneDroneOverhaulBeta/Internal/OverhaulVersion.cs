using System;
using System.Linq;
using System.Reflection;

namespace CDOverhaul
{
    public static class OverhaulVersion
    {
        private static readonly Version s_AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        private static readonly Version s_ModVersionUpdate2 = new Version("0.2.12.0");
        private static readonly Version s_ModVersionUpdate4 = new Version("0.4.0.1");

        private static readonly Updates s_CurrentUpdate = Updates.VER_3;

        public static bool IsUpdate(Updates update) => s_CurrentUpdate >= update;
        public static bool IsUpdate2 => !IsUpdate(Updates.VER_3);
        public static bool IsUpdate4 => IsUpdate(Updates.VER_4);

        public static Version ModVersion
        {
            get
            {
                switch (s_CurrentUpdate)
                {
                    case Updates.VER_2:
                        return s_ModVersionUpdate2;
                    case Updates.VER_4:
                        return s_ModVersionUpdate4;
                }
                return s_AssemblyVersion;
            }
        }
        public static string GetBuildString() => 'v' + ModVersion.ToString() + DebugString;

        /// <summary>
        /// The version of the game the mod will definitely work fine
        /// </summary>
        public const string TargetGameVersion = "1.5.0.18";

        public static readonly string Watermark = "Clone Drone Overhaul " + GetBuildString();
        public static readonly string ShortenedWatermark = "Overhaul " + GetBuildString();

#if DEBUG
        public const bool IsDebugBuild = true;
        public const string DebugString = " [Debug]";
#else
        public const bool IsDebugBuild = false;
        public const string DebugString = "";
#endif

        private static readonly string[] s_BlacklistedVersions = new string[]
        {
            "0.3.0.199" // A debug build was released to public by mistake xd
        };
        public static bool IsBlacklistedVersion(string versionString) => s_BlacklistedVersions.Contains(versionString);

        public enum Updates
        {
            VER_2 = 0,

            VER_3 = 1,

            VER_4 = 2,
        }
    }
}
