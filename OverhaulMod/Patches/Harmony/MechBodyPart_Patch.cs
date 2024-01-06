using HarmonyLib;
using OverhaulMod.Engine;
using PicaVoxel;
using System;

namespace OverhaulMod.Patches.Harmony
{
    [HarmonyPatch(typeof(MechBodyPart))]
    internal static class MechBodyPart_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("createNewVoxelBeingDestroyed", new Type[] { typeof(PicaVoxelPoint), typeof(FireSpreadDefinition), typeof(float) })]
        private static void createNewVoxelBeingDestroyed_Postfix(MechBodyPart __instance, ref VoxelBeingDestroyed __result, PicaVoxelPoint picaVoxelPoint, FireSpreadDefinition fireSpreadDefinition, float probabilityOfFireSpread)
        {
            if (fireSpreadDefinition != null && !__instance.IgnoreColorBurnForGlowingVoxels)
            {
                FadingVoxelManager manager = FadingVoxelManager.Instance;
                if (manager)
                {
                    __result.TimeToDestroy += manager.timeToDestroyOffset;
                    manager.AddFadingVoxel(picaVoxelPoint, __instance, __result.TimeToDestroy);
                }
            }
        }

        /*
        [HarmonyPrefix]
        [HarmonyPatch("destroyVoxelAtPositionFromCut")]
        private static bool destroyVoxelAtPositionFromCut_Prefix(MechBodyPart __instance, PicaVoxelPoint picaVoxelPoint, Voxel? voxelAtPosition, Vector3 localPosition, Vector3 volumeWorldCenter, Vector3 impactDirectionWorld, FireSpreadDefinition fireSpreadDefinition, Frame currentFrame)
        {
            if (voxelAtPosition != null)
            {
                Vector3 vector = __instance.transform.TransformPoint(localPosition);
                Vector3 vector2 = (vector - volumeWorldCenter).normalized + impactDirectionWorld;
                if (fireSpreadDefinition != null)
                    GlobalFireParticleSystem.Instance.SpawnSingleBig(vector, (3f * impactDirectionWorld) + (1f * vector2), 1f);
                else
                    VoxelParticleSystem.Instance.SpawnSingle(vector, voxelAtPosition.Value.Color, __instance.getVoxelParticleSize() * 0.75f, (3f * impactDirectionWorld) + (1f * vector2));
            }
            Color c = fireSpreadDefinition != null ? AttackManager.Instance.BodyOnFireColor * UnityEngine.Random.Range(0.9f, 1f) : AttackManager.Instance.HitColor;
            currentFrame.SetVoxelAtArrayPosition(picaVoxelPoint, new Voxel
            {
                Color = c,
                State = VoxelState.Active,
                Value = 1
            });
            __instance.DestroyVoxelAfterWait(picaVoxelPoint, fireSpreadDefinition);
            return false;
        }*/
    }
}
