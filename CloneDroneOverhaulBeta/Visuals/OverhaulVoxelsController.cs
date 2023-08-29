using OverhaulAPI;
using PicaVoxel;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulVoxelsController : OverhaulBehaviour
    {
        public const float COLOR_MULTIPLIER = 0.8f;

        [OverhaulSetting("Gameplay.Voxels.Make laser burn voxels", true, false, "Cutting robots with normal sword would leave nearby voxels burnt")]
        public static bool MakeLaserBurnVoxels;

        public static void OnVoxelDestroy(MechBodyPart bodyPart, PicaVoxelPoint picaVoxelPoint, Voxel? voxelAtPosition, Vector3 impactDirectionWorld, FireSpreadDefinition fireSpreadDefinition, Frame currentFrame)
        {
            bool hasFire = fireSpreadDefinition != null;
            if (!hasFire)
            {
                if (Random.Range(0, 13) < 2)
                {
                    Vector3 position = currentFrame.GetVoxelWorldPosition(picaVoxelPoint);
                    _ = PooledPrefabController.SpawnEntry<PooledPrefabInstanceBase>(Visuals.OverhaulEffectsManager.LASER_CUT_VFX, position, Vector3.zero);
                }

                if (MakeLaserBurnVoxels)
                {
                    foreach (PicaVoxelPoint p in GetClosestPoints(picaVoxelPoint))
                    {
                        if (bodyPart.IsVoxelWaitingToBeDestroyed(p))
                            continue;

                        Voxel? vox = currentFrame.GetVoxelAtArrayPosition(p);
                        if (vox == null)
                            continue;

                        Color32 oldColor = vox.Value.Color;
                        Voxel theVox = vox.Value;
                        theVox.Color = new Color32(getColor(oldColor.r),
                            getColor(oldColor.g),
                            getColor(oldColor.b),
                            oldColor.a);
                        currentFrame.SetVoxelAtArrayPosition(p, theVox);
                    }
                }
                return;
            }

            if (Random.Range(0, 13) < 2)
            {
                Vector3 position = currentFrame.GetVoxelWorldPosition(picaVoxelPoint);
                _ = PooledPrefabController.SpawnEntry<PooledPrefabInstanceBase>(Visuals.OverhaulEffectsManager.FIRE_CUT_VFX, position, Vector3.zero);
            }
        }

        private static byte getColor(byte color) => (byte)Mathf.RoundToInt(color * COLOR_MULTIPLIER);

        public static PicaVoxelPoint GetOffsetPoint(in PicaVoxelPoint picaVoxelPoint, in int OffX, in int OffY, in int OffZ) => new PicaVoxelPoint(picaVoxelPoint.X + OffX, picaVoxelPoint.Y + OffY, picaVoxelPoint.Z + OffZ);
        public static PicaVoxelPoint[] GetClosestPoints(in PicaVoxelPoint picaVoxelPoint)
        {
            PicaVoxelPoint x1 = GetOffsetPoint(picaVoxelPoint, 1, 0, 0);
            PicaVoxelPoint x2 = GetOffsetPoint(picaVoxelPoint, -1, 0, 0);
            PicaVoxelPoint y1 = GetOffsetPoint(picaVoxelPoint, 0, 1, 0);
            PicaVoxelPoint y2 = GetOffsetPoint(picaVoxelPoint, 0, -1, 0);
            PicaVoxelPoint z1 = GetOffsetPoint(picaVoxelPoint, 0, 0, 1);
            PicaVoxelPoint z2 = GetOffsetPoint(picaVoxelPoint, 0, 0, -1);
            return new PicaVoxelPoint[6] { x1, x2, y1, y2, z1, z2 };
        }
    }
}