using HarmonyLib;
using OverhaulMod.Engine;
using System.Collections.Generic;
using UnityEngine;

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

        [HarmonyPrefix]
        [HarmonyPatch("addAudiencePositions")]
        private static bool addAudiencePositions_Prefix(AudienceManager __instance, List<Vector3> potentialAudiencePositions, AudiencePlacementLine audiencePlacementLine)
        {
            return audiencePlacementLine && audiencePlacementLine.StartPos && audiencePlacementLine.EndPos;
        }
    }
}
