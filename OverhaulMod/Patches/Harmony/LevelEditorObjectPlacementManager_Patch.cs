using HarmonyLib;

namespace OverhaulMod.Patches.Harmony
{
    /*
    [HarmonyPatch(typeof(LevelEditorObjectPlacementManager))]
    internal static class LevelEditorObjectPlacementManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("PlaceObjectInLevelRoot")]
        private static void PlaceObjectInLevelRoot_Postfix(ref ObjectPlacedInLevel __result)
        {
            if (!__result)
                return;

            __result.gameObject.SetActive(true);
        }
    }*/
}
