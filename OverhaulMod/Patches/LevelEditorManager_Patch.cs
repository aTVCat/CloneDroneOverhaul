using HarmonyLib;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(LevelEditorManager))]
    internal static class LevelEditorManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(LevelEditorManager.Initialize))]
        private static void Initialize_Prefix(LevelEditorManager __instance)
        {
            LevelManager.Instance._currentLevelHidesTheArena = false; // fix arena settings not applying for some reason
        }
    }
}
