using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CDOverhaul
{
    public static class OverhaulVersion
    {
        public const string ModID = "rAnDomPaTcHeS1";
        public const bool IsModBotBuild = true;

        private static readonly Version s_AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        private static readonly Version s_ModVersionUpdate2 = new Version("0.2.13.4");
        private static readonly Version s_ModVersionUpdate3 = new Version("0.3.0.345");

        private static readonly Updates s_CurrentUpdate = Updates.VER_4;

        public static bool IsUpdate(Updates update) => s_CurrentUpdate >= update;
        public static bool IsVersionUnder3 => !IsUpdate(Updates.VER_3);
        public static bool IsVersion3 => IsUpdate(Updates.VER_3);
        public static bool IsVersion3Update => IsUpdate(Updates.VER_3_Update);
        public static bool IsVersion4 => IsUpdate(Updates.VER_4);

        public static Version modVersion
        {
            get
            {
                switch (s_CurrentUpdate)
                {
                    case Updates.VER_2:
                        return s_ModVersionUpdate2;
                    case Updates.VER_3:
                        return s_ModVersionUpdate3;
                }
                return s_AssemblyVersion;
            }
        }

        private static string s_FullBuildTag;
        public static string fullBuildTag
        {
            get
            {
                if (string.IsNullOrEmpty(s_FullBuildTag))
                {
                    FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                    s_FullBuildTag = "alpha " + modVersion.ToString() + " " + fileInfo.LastWriteTime.ToShortDateString().Replace(".", string.Empty) + " [M1]";
                }
                return s_FullBuildTag;
            }
        }

        public static bool hasToShowFullBuildTag
        {
            get
            {
                return IsDevelopmentBuild;
            }
        }

        public const string TargetGameVersion = "1.5.0.18";

        public static readonly string Watermark = "Overhaul Mod Alpha Build v" + modVersion.ToString();
        public static readonly string ShortenedWatermark = "Overhaul v" + modVersion.ToString();

        public const bool IsDevelopmentBuild = true;
#if DEBUG
        public const bool IsDebugBuild = true;
        public const bool IsTestMode = true;
#else
        public const bool IsDebugBuild = false;
        public const bool IsTestMode = false;
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

            VER_3_Update = 2,

            VER_4 = 3,
        }
    }
}
