using System;
using System.Reflection;

namespace CDOverhaul
{
    /// <summary>
    /// Mod version controller
    /// </summary>
    public static class OverhaulVersion
    {
        private static readonly Version m_ModVersionUpd2 = new Version("0.2.10.0");
        /// <summary>
        /// The version of the mod
        /// </summary>
        private static readonly Version m_ModVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public static Version ModVersion
        {
            get
            {
                if (Upd2Hotfix)
                {
                    return m_ModVersionUpd2;
                }
                return m_ModVersion;
            }
        }

        public const string GameTargetVersion = "1.4.0.17";

        /// <summary>
        /// The full name of the mod
        /// </summary>
        public static readonly string ModFullName = "Clone Drone Overhaul Update 2-HF " + getVersionPrefixChar() + ModVersion.ToString() + DebugString;
        /// <summary>
        /// The shortened name of the mod
        /// </summary>
        public static readonly string ModShortName = "Overhaul " + getVersionPrefixChar() + ModVersion.ToString() + DebugString;

        /// <summary>
        /// Enable 0.3 June demo Update things
        /// </summary>
        public const bool Upd3JunePreview = false;

        public const bool Upd2Hotfix = false;

#if DEBUG
        public const bool IsDebugBuild = true;
        public const string DebugString = " (Debug)";
#else
        public const bool IsDebugBuild = false;
        public const string DebugString = "";
#endif

        private static char getVersionPrefixChar() => 'v';
    }
}
