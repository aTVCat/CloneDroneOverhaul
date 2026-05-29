//#define OVERRIDE_VER
//#define DISABLE_EXCLUSIVE_PERKS
#define REPOSITORY_TEST

using OverhaulMod.Utils;
using System;
using System.IO;
using UnityEngine.XR;

namespace OverhaulMod
{
    public static class ModBuild
    {
        public const bool VERSION_4_3 = true;

        public const string BUILD_INFO_FILE_PATH = "buildInfo.json";

#if OVERRIDE_VER
        public const string OVERRIDE_VERSION = "4.2.1126.0";
#endif

        public static readonly Version MinimumGameVersion = new Version(1, 11, 0, 20);

        public static readonly Version MinimumModBotVersion = new Version(2, 1, 0, 0);

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

        public static int VersionRevision
        {
            get;
            private set;
        }

        private static Version s_version;
        public static Version Version
        {
            get => s_version;
        }

        private static string s_versionString;
        public static string VersionString
        {
            get
            {
                if (s_versionString == null)
                {
                    if (VersionRevision > 0)
                    {
                        s_versionString = $"{VersionMajor}.{VersionMinor}.{VersionBuild}.{VersionRevision}";
                    }
                    else
                    {
                        s_versionString = $"{VersionMajor}.{VersionMinor}.{VersionBuild}";
                    }
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

        public static bool RepositoryTest
        {
            get
            {
#if REPOSITORY_TEST
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
            Version version = ModCache.ModAssemblyName.Version;
#endif

            s_version = version;
            VersionMajor = version.Major;
            VersionMinor = version.Minor;
            VersionBuild = version.Build;
            VersionRevision = version.Revision;
        }

        private static void loadBuildInfo()
        {
            try
            {
                s_buildInfo = ModJsonUtils.DeserializeStream<Info>(Path.Combine(ModDirectories.DataFolder, BUILD_INFO_FILE_PATH));
            }
            catch (Exception)
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
            ModJsonUtils.WriteStream(Path.Combine(ModDirectories.DataFolder, BUILD_INFO_FILE_PATH), s_buildInfo);
        }

        public static bool ShouldShowHypocrisis3Special()
        {
            return ModFeatures.IsEnabled(ModFeatures.FeatureType.Hypocrisis3Special) && ModSpecialUtils.IsModEnabled("hypocrisis-mod");
        }

        public static bool IsInVRMode() => XRSettings.enabled;

        public static bool IsGameVersionSupported(Version gameVersion) => gameVersion >= MinimumGameVersion;

        public static bool IsModBotVersionSupported(Version modBotVersion) => modBotVersion >= MinimumModBotVersion;

        public class Info
        {
            public DateTime CompilationTime;
        }
    }
}