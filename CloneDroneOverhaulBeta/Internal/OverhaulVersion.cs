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

        /// <summary>
        /// Enable arena remaster in the build
        /// </summary>
        public const bool UseArenaRemaster = true;
        /// <summary>
        /// Enable usage of Discord RPC
        /// </summary>
        public const bool AllowRPC = false;

        /// <summary>
        /// Enable 0.3 Tech-Demo 2 Update things
        /// </summary>
        public const bool Upd3TechDemo2 = true;

        /// <summary>
        /// Enable 0.3 June demo Update things
        /// </summary>
        public const bool Upd3JunePreview = false;

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
