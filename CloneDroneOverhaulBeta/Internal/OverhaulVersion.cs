using System;
using System.Reflection;

namespace CDOverhaul
{
    public static class OverhaulVersion
    {
        /// <summary>
        /// The version of the mod
        /// </summary>
        private static readonly Version m_ModVersion = Assembly.GetExecutingAssembly().GetName().Version;
        private static readonly Version m_ModVersionUpd2 = new Version("0.2.10.56");

        public static Version ModVersion => IsUpdate2Hotfix ? m_ModVersionUpd2 : m_ModVersion;

        /// <summary>
        /// The version of game mod will definitely work 
        /// </summary>
        public const string GameTargetVersion = "1.5.0.16";

        /// <summary>
        /// The full name of the mod
        /// </summary>
        public static readonly string ModFullName = "Clone Drone Overhaul " + buildString;
        /// <summary>
        /// The shortened name of the mod
        /// </summary>
        public static readonly string ModShortName = "Overhaul " + buildString;
        private static string buildString => getVersionPrefixChar() + ModVersion.ToString() + DebugString;

        public const bool IsUpdate2Hotfix = false;

        public const bool IsUpdate4 = true;

#if DEBUG
        public const bool IsDebugBuild = true;
        public const string DebugString = " (Debug)";
#else
        public const bool IsDebugBuild = false;
        public const string DebugString = "";
#endif

        private static char getVersionPrefixChar()
        {
            return 'v';
        }
    }
}
