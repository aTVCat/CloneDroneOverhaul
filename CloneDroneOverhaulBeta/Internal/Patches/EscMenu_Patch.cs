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
            if (!OverhaulMod.IsCoreCreated)
            {
                return true;
            }

            if (OverhaulVersion.TechDemo2Enabled && OverhaulPauseMenu.UseThisMenu && !OverhaulPauseMenu.ForceUseOldMenu)
            {
                OverhaulPauseMenu.ToggleMenu();
                return false;
            }
            return true;
        }
    }
}
