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

        public static readonly string CurrentBranch = Branch.V03Dev_Base.ToString();

        public static readonly string ModFullName = "Clone drone Overhaul Beta a" + ModVersion.ToString() + " [" + CurrentBranch + "]";
        public static readonly string ModShortName = "CDO a" + ModVersion.ToString() + " [" + CurrentBranch + "]";

        public enum Branch
        {
            V03Dev,
            V03Dev_Base,
            Optimization
        }
    }
}
