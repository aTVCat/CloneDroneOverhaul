using HarmonyLib;
using OverhaulMod.Utils;
using System.Collections.Generic;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(GameDataManager))]
    internal static class GameDataManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetSectionsVisible")]
        private static void GetSectionsVisible_Postfix(ref List<string> __result)
        {
            if (GameModeManager.Is(GameMode.Story))
            {
                var list = ModGameUtils.overrideActiveSections;
                if(list != null)
                    __result = list;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("GetCurrentLevelID")]
        private static void GetCurrentLevelID_Postfix(ref string __result)
        {
            if (GameModeManager.Is(GameMode.Story))
            {
                var list = ModGameUtils.overrideCurrentLevelId;
                if (list != null)
                    __result = list;
            }
        }
    }
}
