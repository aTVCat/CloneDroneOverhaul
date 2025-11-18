using HarmonyLib;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(CustomizationUIButtonRow))]
    internal static class CustomizationUIButtonRow_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CustomizationUIButtonRow.UpdateSelected))]
        private static bool UpdateSelected_Prefix(ref CustomizationUIButtonRow __instance)
        {
            return __instance._animator;
        }
    }
}
