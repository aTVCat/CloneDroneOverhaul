using HarmonyLib;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            UIConstants.ShowPauseMenuRework();
            return false;
        }
    }
}
