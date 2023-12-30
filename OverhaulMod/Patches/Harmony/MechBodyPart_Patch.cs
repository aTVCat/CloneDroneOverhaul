using HarmonyLib;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using PicaVoxel;
using System;
using UnityEngine;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(MechBodyPart))]
    internal static class MechBodyPart_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("createNewVoxelBeingDestroyed", new Type[] { typeof(PicaVoxelPoint), typeof(FireSpreadDefinition), typeof(float) })]
        private static void createNewVoxelBeingDestroyed_Postfix(MechBodyPart __instance, ref VoxelBeingDestroyed __result, PicaVoxelPoint picaVoxelPoint, FireSpreadDefinition fireSpreadDefinition, float probabilityOfFireSpread)
        {
            if(fireSpreadDefinition != null && !__instance.IgnoreColorBurnForGlowingVoxels)
            {
                var manager = FadingVoxelManager.Instance;
                if (manager)
                {
                    __result.TimeToDestroy += manager.timeToDestroyOffset;
                    manager.AddFadingVoxel(picaVoxelPoint, __instance, __result.TimeToDestroy);
                }
            }
        }
    }
}
