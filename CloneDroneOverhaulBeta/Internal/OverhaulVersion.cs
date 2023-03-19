using System;
using System.Reflection;

namespace CDOverhaul
{
    /// <summary>
    /// Mod version controller
    /// </summary>
    public static class OverhaulVersion
    {
        /// <summary>
        /// The version of the mod
        /// </summary>
        public static readonly Version ModVersion = Assembly.GetExecutingAssembly().GetName().Version;

        public const string GameTargetVersion = "1.4.0.17";

        /// <summary>
        /// The full name of the mod
        /// </summary>
        public static readonly string ModFullName = "Clone Drone Overhaul Tech-Demo 2 " + getVersionPrefixChar() + ModVersion.ToString() + DebugString;
        /// <summary>
        /// The shortened name of the mod
        /// </summary>
        public static readonly string ModShortName = "Overhaul " + getVersionPrefixChar() + ModVersion.ToString() + DebugString;

        public const bool UseArenaRemaster = true;
        public const bool AllowWindowNameChanging = false;

        /// <summary>
        /// Enable 0.3 Tech-Demo 2 Update things
        /// </summary>
        public const bool TechDemo2Enabled = true;

        /// <summary>
        /// Enable 0.3 June demo Update things
        /// </summary>
        public const bool JuneDemoEnabled = true;

        /// <summary>
        /// Enable 0.4 Update things
        /// </summary>
        public const bool LevelEditorUpdateEnabled = false;

        /// <summary>
        /// Enable 0.5 Update things
        /// </summary>
        public const bool RandomPatchesUpdateEnabled = false;

        /// <summary>
        /// Enable 0.6 Update things
        /// </summary>
        public const bool CommunityUpdateEnabled = false;

        /// <summary>
        /// Enable 0.7 Update things
        /// </summary>
        public const bool BalancingUpdateEnabled = false;

#if DEBUG
        public const bool IsDebugBuild = true;
        private const string DebugString = " [DEBUG]";
#else
        public const bool IsDebugBuild = false;
        private const string DebugString = "";
#endif

        private static char getVersionPrefixChar()
        {
            return 'v';
        }
    }
}
