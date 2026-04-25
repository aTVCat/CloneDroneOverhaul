using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(CacheManager))]
    internal static class CacheManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(CacheManager.CreateOrClearInstance))]
        private static void CreateOrClearInstance_Postfix()
        {
            ComponentCacheManager manager = ComponentCacheManager.Instance;
            if (manager) manager.ClearCache();
        }
    }
}