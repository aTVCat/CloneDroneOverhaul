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
            if (!OverhaulMod.IsModInitialized || !UIPauseMenu.Instance)
                return true;

            if (UIPauseMenu.UseThisMenu && !UIPauseMenu.ForceUseOldMenu)
            {
                UIPauseMenu.ToggleMenu();
                return false;
            }
            return true;
        }
    }
}
