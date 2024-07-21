using HarmonyLib;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(MetagameLevelTrigger))]
    internal static class MetagameLevelTrigger_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnTriggerEnter")]
        private static bool OnTriggerEnter_Prefix()
        {
            return !GameModeManager.IsStoryChapter3();
        }
    }
}
