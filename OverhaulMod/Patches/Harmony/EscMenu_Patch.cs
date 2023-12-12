using HarmonyLib;
using OverhaulMod.UI;
using OverhaulMod.Utils;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(EscMenu))]
    internal static class EscMenu_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Show")]
        private static bool Show_Prefix()
        {
            if (UIPauseMenu.disableOverhauledVersion)
                return true;

            ModUIConstants.ShowPauseMenuRework();
            return false;
        }
    }
}
