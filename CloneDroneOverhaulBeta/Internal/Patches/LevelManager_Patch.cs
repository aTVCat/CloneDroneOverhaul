using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Overmodes;
using HarmonyLib;
using System.Collections.Generic;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(LevelManager))]
    internal static class LevelManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("getLevelDescriptions")]
        private static bool getLevelDescriptions_Postfix(ref List<LevelDescription> __result)
        {
            if(OvermodesController.Instance && OvermodesController.Instance.IsOvermode())
            {
                __result = OvermodesController.Instance.CurrentOvermode.GetLevelDescriptions();
                return false;
            }
            return true;
        }
    }
}
