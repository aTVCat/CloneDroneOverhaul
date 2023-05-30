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
            if (!OverhaulMod.IsModInitialized)
            {
                return;
            }

            OverhaulVolumeController.OnVoxelDestroy(__instance, picaVoxelPoint, voxelAtPosition, impactDirectionWorld, fireSpreadDefinition, currentFrame);
        }

        [HarmonyPrefix]
        [HarmonyPatch("CanApplyArmorPiece")]
        private static bool CanApplyArmorPiece_Prefix(MechBodyPart __instance, ArmorPiece armorPiecePrefab, ref bool __result)
        {
            if (!OverhaulMod.IsModInitialized)
            {
                return true;
            }

            __result = __instance.transform.parent != null && armorPiecePrefab != null;
            return __result;
        }
    }
}
