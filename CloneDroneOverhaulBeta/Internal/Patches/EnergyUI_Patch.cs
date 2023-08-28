using CDOverhaul.HUD.Vanilla;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(EnergyUI))]
    internal static class EnergyUI_Patch
    {
        /*
        [HarmonyPostfix]
        [HarmonyPatch("Show")]
        private static void Show_Postfix()
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            EnergyUIReplacement.RefreshPatchStatic();
        }*/

        [HarmonyPrefix]
        [HarmonyPatch("SetErrorLabelVisible")]
        private static bool SetErrorLabelVisible_Prefix(string text)
        {
            if (!EnergyUIReplacement.PatchHUD || !OverhaulMod.IsModInitialized || VanillaUIImprovements.InstanceIsNull)
                return true;

            VanillaUIImprovements.Instance.EnergyUI.ShowText(text);
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch("onInsufficientEnergyAttempt")]
        private static bool onInsufficientEnergyAttempt_Prefix(EnergyUI __instance, float requestedAmount)
        {
            if (!EnergyUIReplacement.PatchHUD || !OverhaulMod.IsModInitialized || VanillaUIImprovements.InstanceIsNull)
                return true;

            __instance.showGlow(0f, requestedAmount, "InsufficientAmount");
            return false;
        }
    }
}
