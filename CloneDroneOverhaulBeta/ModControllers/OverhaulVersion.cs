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

        /// <summary>
        /// The full name of the mod
        /// </summary>
        public static readonly string ModFullName = "Clone Drone Overhaul Beta a" + ModVersion.ToString() + DebugString;
        /// <summary>
        /// The shortened name of the mod
        /// </summary>
        public static readonly string ModShortName = "Overhaul a" + ModVersion.ToString() + DebugString;

#if DEBUG
        public const bool IsDebugBuild = true;
        private const string DebugString = " [DEBUG]";
#else
        public const bool IsDebugBuild = false;
        private const string DebugString = "";
#endif

    }
}
