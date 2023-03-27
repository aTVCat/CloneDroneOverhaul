using HarmonyLib;
using PicaVoxel;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(MechBodyPart))]
    internal static class MechBodyPart_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("destroyVoxelAtPositionFromCut")]
        private static void destroyVoxelAtPositionFromCut(MechBodyPart __instance, PicaVoxelPoint picaVoxelPoint, Voxel? voxelAtPosition, Vector3 localPosition, Vector3 volumeWorldCenter, Vector3 impactDirectionWorld, FireSpreadDefinition fireSpreadDefinition, Frame currentFrame)
        {
            if (!OverhaulMod.IsCoreCreated)
            {
                return;
            }

            VoxelsController.OnVoxelDestroy(__instance, picaVoxelPoint, voxelAtPosition, impactDirectionWorld, fireSpreadDefinition, currentFrame);
        }
    }
}
