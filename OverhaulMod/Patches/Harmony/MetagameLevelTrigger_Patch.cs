using HarmonyLib;
using OverhaulMod.Utils;
using System.Collections.Generic;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(MetagameLevelTrigger))]
    internal static class MetagameLevelTrigger_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnTriggerEnter")]
        private static bool OnTriggerEnter_Prefix()
        {
            if (GameModeManager.IsStoryChapter3())
                return false;

            return true;
        }
    }
}
