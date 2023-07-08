using CDOverhaul.Gameplay;
using HarmonyLib;
using ModLibrary;
using PicaVoxel;
using System.Collections.Generic;
using UnityEngine;
using static RootMotion.FinalIK.IKSolver;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(MechBodyPart))]
    internal static class MechBodyPart_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("destroyVoxelAtPositionFromCut")]
        private static void destroyVoxelAtPositionFromCut(MechBodyPart __instance, PicaVoxelPoint picaVoxelPoint, Voxel? voxelAtPosition, Vector3 localPosition, Vector3 volumeWorldCenter, Vector3 impactDirectionWorld, FireSpreadDefinition fireSpreadDefinition, Frame currentFrame)
        {
            if (!OverhaulMod.IsModInitialized)
                return;

            OverhaulVolumeController.OnVoxelDestroy(__instance, picaVoxelPoint, voxelAtPosition, impactDirectionWorld, fireSpreadDefinition, currentFrame);
        }

        // A fix of a crash
        [HarmonyPrefix]
        [HarmonyPatch("CanApplyArmorPiece")]
        private static bool CanApplyArmorPiece_Prefix(MechBodyPart __instance, ArmorPiece armorPiecePrefab, ref bool __result)
        {
            if (!OverhaulMod.IsModInitialized)
                return true;

            try
            {
                __result = __instance.transform.parent && armorPiecePrefab;
            }
            catch
            {
                __result = false;
            }
            return __result;
        }

        /*
        [HarmonyPrefix]
        [HarmonyPatch("tryBurnColorAt")]
        private static bool tryBurnColorAt_Prefix(MechBodyPart __instance, Frame currentFrame, PicaVoxelPoint voxelPosition, int offsetX, int offsetY, int offsetZ, float colorMultiplier = -1f)
        {
            if (!OverhaulMod.IsModInitialized)
                return true;

            
            PicaVoxelPoint point = new PicaVoxelPoint(voxelPosition.X + offsetX, voxelPosition.Y + offsetY, voxelPosition.Z + offsetZ);
            if (__instance.IsVoxelWaitingToBeDestroyed(point))
                return false;

            VRVoxelBurnEffectController.SetPointOnFire(currentFrame, point);
            return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MechBodyPart), "tryAddingVoxelToFireSpreadList", new System.Type[] { typeof(Frame), typeof(PicaVoxelPoint), typeof(List<VoxelBeingDestroyed>), typeof(int), typeof(int), typeof(int), typeof(FireSpreadDefinition) })]
        private static void tryAddingVoxelToFireSpreadList_Postfix(MechBodyPart __instance, Frame currentFrame, PicaVoxelPoint voxelPosition, List<VoxelBeingDestroyed> pointsSetOnFire, int offsetX, int offsetY, int offsetZ, FireSpreadDefinition fireSpreadDefinition)
        {
            PicaVoxelPoint point = new PicaVoxelPoint(voxelPosition.X + offsetX, voxelPosition.Y + offsetY, voxelPosition.Z + offsetZ);
            VRVoxelBurnEffectController.SetPointOnFire(currentFrame, point);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechBodyPart), "tryAddingVoxelToFireSpreadList", new System.Type[] { typeof(Frame), typeof(PicaVoxelPoint), typeof(List<VoxelBeingDestroyed>), typeof(int), typeof(int), typeof(int), typeof(VoxelBeingDestroyed) })]
        private static void tryAddingVoxelToFireSpreadList2_Postfix(MechBodyPart __instance, Frame currentFrame, PicaVoxelPoint voxelPosition, List<VoxelBeingDestroyed> pointsSetOnFire, int offsetX, int offsetY, int offsetZ, VoxelBeingDestroyed voxelBeingDestroyed)
        {
            PicaVoxelPoint point = new PicaVoxelPoint(voxelPosition.X + offsetX, voxelPosition.Y + offsetY, voxelPosition.Z + offsetZ);
            VRVoxelBurnEffectController.SetPointOnFire(currentFrame, point);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MechBodyPart), "TrackVoxelToBeDestroyed")]
        public static void TrackVoxelToBeDestroyed_Postfix(MechBodyPart __instance, ref VoxelBeingDestroyed voxelBeingDestroyed)
        {
            if (voxelBeingDestroyed.FireSpreadDefinition != null)
            {
                voxelBeingDestroyed.TimeToDestroy = Time.time + 0.85f;
                //VRVoxelBurnEffectController.SetPointOnFire(__instance.GetPrivateField<Frame>("_currentFrame"), voxelBeingDestroyed.VoxelPoint);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateMe")]
        private static void UpdateMe_Prefix(MechBodyPart __instance)
        {
            if (OverhaulMod.IsModInitialized)
            {
                //VRVoxelBurnEffectController.UpdateFrame(__instance.GetPrivateField<Frame>("_currentFrame"));
            }
        }*/
    }
}
