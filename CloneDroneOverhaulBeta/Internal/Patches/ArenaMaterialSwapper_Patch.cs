using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Combat;
using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ArenaMaterialSwapper))]
    internal static class ArenaMaterialSwapper_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("onArenaSettingsRefreshed")]
        private static bool onArenaSettingsRefreshed_Prefix(AmplifyColorSwapper __instance)
        {
            if (!OverhaulMod.IsModInitialized)
                return true;

            return !OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsArenaOverhaulEnabled;
        }
    }
}
