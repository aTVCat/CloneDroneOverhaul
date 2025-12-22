#define OVERRIDE_VER
//#define DISABLE_EXCLUSIVE_PERKS
//#define DEVELOPER_BUILD
//#define PRE_RELEASE_BUILD

using OverhaulMod.Utils;
using System;
using System.IO;

namespace OverhaulMod
{
    public static class ModBuildInfo
    {
        public const bool VERSION_4_2_PATCH = true;

        public const bool VERSION_4_3 = false;

        public const bool VERSION_4_4 = false;

        public const bool VERSION_5_0 = false;

        public const string EXTRA_INFO_FILE_PATH = "buildInfo.json";

#if OVERRIDE_VER
        public const string OVERRIDE_VERSION = "4.2.1050";
#endif

        private static bool s_loaded;

        public static ExtraInfo extraInfo { get; private set; }

        public static bool extraInfoError
        {
            get
            {
                return extraInfo == null;
            }
        }

        public static int versionMajor
        {
            get;
            private set;
        }

        public static int versionMinor
        {
            get;
            private set;
        }

        public static int versionBuild
        {
            get;
            private set;
        }

        private static Version s_version;
        public static Version version
        {
            get
            {
                if (s_version == null)
                {
                    s_version = new Version(versionMajor, versionMinor, versionBuild);
                }
                return s_version;
            }
        }

        private static string s_versionStringNoBranch;
        public static string versionStringNoBranch
        {
            get
            {
                if (s_versionStringNoBranch == null)
                {
                    s_versionStringNoBranch = $"{versionMajor}.{versionMinor}.{versionBuild}";
                }
                return s_versionStringNoBranch;
            }
        }

        private static string s_versionString;
        public static string versionString
        {
            get
            {
                if (s_versionString == null)
                {
                    if (isPrereleaseBuild)
                        s_versionString = $"{versionMajor}.{versionMinor}.{versionBuild} {branchName}";
                    else
                        s_versionString = $"{versionMajor}.{versionMinor}.{versionBuild}";
                }
                return s_versionString;
            }
        }

        private static string s_fullVersionString;
        public static string fullVersionString
        {
            get
            {
                if (s_fullVersionString == null)
                {
                    string buildRelease = debug ? "internal" : "public";
                    if (extraInfo != null && !extraInfoError)
                    {
                        string date = extraInfo.CompileTime.ToShortDateString();
                        s_fullVersionString = $"{versionString} ({date}, {buildRelease})";
                    }
                    else
                    {
                        s_fullVersionString = $"{versionString} (na, {buildRelease})";
                    }
                }
                return s_fullVersionString;
            }
        }

        public static bool debug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static bool isPrereleaseBuild
        {
            get
            {
#if PRE_RELEASE_BUILD
                return true;
#else
                return false;
#endif
            }
        }

        public static bool isDeveloperBuild
        {
            get
            {
#if DEVELOPER_BUILD
                return true;
#else
                return false;
#endif
            }
        }

        public static bool disableExclusivePerks
        {
            get
            {
#if DISABLE_EXCLUSIVE_PERKS
                return true;
#else
                return false;
#endif
            }
        }

        public static string branchName
        {
            get
            {
                return "0.4-dev";
            }
        }

        internal static void Load()
        {
            if (s_loaded)
                return;

            loadVersion();
            loadExtraInfo();
            s_loaded = true;
        }

        private static void loadVersion()
        {
#if OVERRIDE_VER
            Version version;
            string verString = OVERRIDE_VERSION;
            if (!verString.IsNullOrEmpty() && !verString.IsNullOrWhiteSpace() && Version.TryParse(verString, out Version result))
                version = result;
            else
                version = ModCache.modAssemblyName.Version;
#else
            Version version = ModCache.modAssemblyName.Version;
#endif
            int major = version.Major;
            int minor = version.Minor;
            int build = version.Build;

            versionMajor = major;
            versionMinor = minor;
            versionBuild = build;
        }

        private static void loadExtraInfo()
        {
            extraInfo = null;
            try
            {
                string filePath = Path.Combine(ModCore.dataFolder, EXTRA_INFO_FILE_PATH);

                ExtraInfo loadedExtraInfo = ModJsonUtils.DeserializeStream<ExtraInfo>(filePath);
                extraInfo = loadedExtraInfo;
            }
            catch { }
        }

        public static bool ShouldShowHypocrisis3Special()
        {
            return ModFeatures.IsEnabled(ModFeatures.FeatureType.Hypocrisis3Special) && ModSpecialUtils.IsModEnabled("hypocrisis-mod");
        }

        public static void GenerateExtraInfo()
        {
            ExtraInfo ei = new ExtraInfo()
            {
                CompileTime = DateTime.UtcNow
            };
            extraInfo = ei;

            string filePath = Path.Combine(ModCore.dataFolder, EXTRA_INFO_FILE_PATH);
            string content = ModJsonUtils.Serialize(ei);
            ModFileUtils.WriteText(content, filePath);
        }

        public class ExtraInfo
        {
            public DateTime CompileTime;
        }
    }
}