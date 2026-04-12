#define OVERRIDE_VER
//#define DISABLE_EXCLUSIVE_PERKS

using OverhaulMod.Utils;
using System;
using System.IO;

namespace OverhaulMod
{
    public static class ModBuild
    {
        public const bool VERSION_4_3 = false;

        public const string BUILD_INFO_FILE_PATH = "buildInfo.json";

#if OVERRIDE_VER
        public const string OVERRIDE_VERSION = "4.2.1094";
#endif

        private static bool s_loaded;

        public static Info s_buildInfo;
        public static Info BuildInfo
        {
            get => s_buildInfo;
        }

        public static int VersionMajor
        {
            get;
            private set;
        }

        public static int VersionMinor
        {
            get;
            private set;
        }

        public static int VersionBuild
        {
            get;
            private set;
        }

        private static Version s_version;
        public static Version Version
        {
            get
            {
                if (s_version == null)
                {
                    s_version = new Version(VersionMajor, VersionMinor, VersionBuild);
                }
                return s_version;
            }
        }

        private static string s_versionString;
        public static string VersionString
        {
            get
            {
                if (s_versionString == null)
                {
                    s_versionString = $"{VersionMajor}.{VersionMinor}.{VersionBuild}";
                }
                return s_versionString;
            }
        }

        private static string s_fullVersionString;
        public static string FullVersionString
        {
            get
            {
                if (s_fullVersionString == null)
                {
                    if (s_buildInfo != null)
                    {
                        s_fullVersionString = $"{VersionString} ({s_buildInfo.CompilationTime.ToShortDateString()})";
                    }
                    else
                    {
                        s_fullVersionString = $"{VersionString} (unknown)";
                    }
                }
                return s_fullVersionString;
            }
        }

        public static bool IsDebugBuild
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

        public static bool DisableExclusivePerks
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

        internal static void Load()
        {
            if (s_loaded) return;

            loadVersion();
            loadBuildInfo();
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
                version = ModCache.ModAssemblyName.Version;
#else
            Version version = ModCache.modAssemblyName.Version;
#endif
            int major = version.Major;
            int minor = version.Minor;
            int build = version.Build;

            VersionMajor = major;
            VersionMinor = minor;
            VersionBuild = build;
        }

        private static void loadBuildInfo()
        {
            try
            {
                s_buildInfo = ModJsonUtils.DeserializeStream<Info>(Path.Combine(ModCore.DataFolder, BUILD_INFO_FILE_PATH));
            }
            catch
            {
                s_buildInfo = null;
            }
        }

        public static void GenerateBuildInfo()
        {
            s_buildInfo = new Info()
            {
                CompilationTime = DateTime.UtcNow
            };
            ModJsonUtils.WriteStream(Path.Combine(ModCore.DataFolder, BUILD_INFO_FILE_PATH), s_buildInfo);
        }

        public static bool ShouldShowHypocrisis3Special()
        {
            return ModFeatures.IsEnabled(ModFeatures.FeatureType.Hypocrisis3Special) && ModSpecialUtils.IsModEnabled("hypocrisis-mod");
        }

        public class Info
        {
            public DateTime CompilationTime;
        }
    }
}