using System;
using System.Reflection;

namespace CDOverhaul
{
    /// <summary>
    /// Mod version manager to be expanded in future builds...
    /// </summary>
    public static class OverhaulVersion
    {
        /// <summary>
        /// The version of the mod
        /// </summary>
        public static readonly Version ModVersion = Assembly.GetExecutingAssembly().GetName().Version;

        public static readonly string CurrentBranch = Branch.V03Dev.ToString();

        public static readonly string ModFullName = "Clone drone Overhaul Beta a" + ModVersion.ToString() + " [" + CurrentBranch + "]" + DebugString;
        public static readonly string ModShortName = "CDO a" + ModVersion.ToString() + " [" + CurrentBranch + "]" + DebugString;


#if DEBUG
        public const bool IsDebugBuild = true;
        private const string DebugString = " [DEBUG]";
#else
        public const bool IsDebugBuild = false;
        private const string DebugString = "";
#endif

        public enum Branch
        {
            V03Dev,
            V03Dev_Base,
            Optimization
        }
    }
}
