using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(EnergyUI))]
    internal static class EnergyUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Show")]
        private static void Show_Postfix()
        {
            if (!OverhaulMod.IsCoreCreated)
            {
                return;
            }

            EnergyUIReplacement.DirectRefreshPatch();
        }
    }
}
