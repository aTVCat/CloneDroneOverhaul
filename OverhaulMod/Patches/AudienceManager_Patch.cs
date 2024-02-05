using HarmonyLib;
using OverhaulMod.Engine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(AudienceManager))]
    internal static class AudienceManager_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("InitializeAudienceDataForLevel")]
        private static void InitializeAudienceDataForLevel_Postfix()
        {
            ArenaAudienceManager arenaAudienceManager = ArenaAudienceManager.Instance;
            if (arenaAudienceManager && ArenaRemodelManager.EnableRemodel)
            {
                arenaAudienceManager.PatchAudienceRotation();
            }
        }
    }
}
