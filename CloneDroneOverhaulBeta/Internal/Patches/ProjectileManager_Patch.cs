using HarmonyLib;
namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(ProjectileManager))]
    internal static class ProjectileManager_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("GetArrowVelocity")]
        private static bool GetArrowVelocity_Postfix(ref float __result)
        {
            __result = GameModeManager.IsMultiplayer() ? 40f : 75f;
            return false;
        }
    }
}