#if DEBUG
using HarmonyLib;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(AlwaysHideFromLevelEditor))]
    internal static class AlwaysHideFromLevelEditor_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("onLevelEditorStarted")]
        private static void onLevelEditorStarted_Postfix(AlwaysHideFromLevelEditor __instance)
        {
            __instance.gameObject.SetActive(true);
        }
    }
}
#endif