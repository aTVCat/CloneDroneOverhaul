using CDOverhaul.HUD;
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(EscMenu))]
    internal static class EscMenu_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Show")]
        private static bool Show_Prefix()
        {
            if (!OverhaulMod.IsModInitialized || !OverhaulMod.IsHUDInitialized)
                return true;

            if (OverhaulPauseMenu.UseThisMenu && !OverhaulPauseMenu.ForceUseOldMenu)
            {
                OverhaulPauseMenu.ToggleMenu();
                return false;
            }
            return true;
        }
    }
}
