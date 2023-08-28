using CDOverhaul.Visuals;
using HarmonyLib;
namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(PlayerCameraMover))]
    internal static class PlayerCameraMover_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("LateUpdate")]
        private static bool LateUpdate_Prefix()
        {
            return !OverhaulMod.IsModInitialized || !ViewModesSystem.IsFirstPersonModeEnabled;
        }
    }
}