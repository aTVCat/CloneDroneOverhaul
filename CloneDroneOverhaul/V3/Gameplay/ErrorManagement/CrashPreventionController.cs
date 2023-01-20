using System;

namespace CloneDroneOverhaul.V3.Gameplay
{
    /// <summary>
    /// Controller that prevents the game from crashing in some cases
    /// </summary>
    public static class CrashPreventionController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="latestLog"></param>
        /// <returns></returns>
        public static bool TryPreventCrash(in string latestLog)
        {
            throw new NotImplementedException("Version 0.3");
            return false;
        }
    }
}
