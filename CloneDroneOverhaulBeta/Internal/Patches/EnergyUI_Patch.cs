﻿using CDOverhaul.HUD.Vanilla;
using HarmonyLib;
using ModLibrary;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(EnergyUI))]
    internal static class EnergyUI_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Show")]
        private static void Show_Postfix()
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            EnergyUIReplacement.RefreshPatchStatic();
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetErrorLabelVisible")]
        private static bool SetErrorLabelVisible_Prefix(string text)
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.AreEnergyUIImprovementsEnabled || !EnergyUIReplacement.PatchHUD || !OverhaulMod.IsModInitialized || VanillaUIImprovements.InstanceIsNull)
                return true;

            VanillaUIImprovements.Instance.EnergyUI.ShowText(text);
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch("onInsufficientEnergyAttempt")]
        private static bool onInsufficientEnergyAttempt_Prefix(EnergyUI __instance, float requestedAmount)
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.AreEnergyUIImprovementsEnabled || !EnergyUIReplacement.PatchHUD || !OverhaulMod.IsModInitialized || VanillaUIImprovements.InstanceIsNull)
                return true;

            object[] args = new object[] { 0f, requestedAmount, "InsufficientAmount" };
            __instance.CallPrivateMethod("showGlow", args);
            return false;
        }
    }
}
