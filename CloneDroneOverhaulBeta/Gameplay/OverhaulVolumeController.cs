using PicaVoxel;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulVolumeController : OverhaulController
    {
        private static float m_OgFireBurnColorMultiplier;

        [OverhaulSetting("Gameplay.Voxels.Make laser burn voxels", true, false, "Cutting robots with normal sword would leave nearby voxels burnt")]
        public static bool MakeLaserBurnVoxels;

        public override void Initialize()
        {
            // Change hit colors
            m_OgFireBurnColorMultiplier = AttackManager.Instance.FireBurnColorMultiplier;
            AttackManager.Instance.HitColor = new Color(4f, 0.65f, 0.35f, 0.2f);
            AttackManager.Instance.BodyOnFireColor = new Color(1f, 0.42f, 0.22f, 0.1f);
        }

        public static void OnVoxelDestroy(MechBodyPart bodyPart, PicaVoxelPoint picaVoxelPoint, Voxel? voxelAtPosition, Vector3 impactDirectionWorld, FireSpreadDefinition fireSpreadDefinition, Frame currentFrame)
        {
            if (!MakeLaserBurnVoxels)
            {
                return;
            }

            foreach (PicaVoxelPoint p in GetSurroundingPoints(picaVoxelPoint))
            {
                if (!bodyPart.IsVoxelWaitingToBeDestroyed(p))
                {
                    Voxel? vox = currentFrame.GetVoxelAtArrayPosition(p);
                    if (vox != null)
                    {
                        Voxel newVox = vox.Value;
                        Color32 color = new Color32((byte)Mathf.RoundToInt(vox.Value.Color.r * m_OgFireBurnColorMultiplier), (byte)Mathf.RoundToInt(vox.Value.Color.g * m_OgFireBurnColorMultiplier), (byte)Mathf.RoundToInt(vox.Value.Color.b * m_OgFireBurnColorMultiplier), vox.Value.Color.a);
                        newVox.Color = color;
                        currentFrame.SetVoxelAtArrayPosition(p, newVox);
                    }
                }
            }
        }

        public static PicaVoxelPoint GetOffsetPoint(in PicaVoxelPoint picaVoxelPoint, in int OffX, in int OffY, in int OffZ)
        {
            return new PicaVoxelPoint(picaVoxelPoint.X + OffX, picaVoxelPoint.Y + OffY, picaVoxelPoint.Z + OffZ);
        }

        public static PicaVoxelPoint[] GetSurroundingPoints(in PicaVoxelPoint picaVoxelPoint)
        {
            PicaVoxelPoint x1 = GetOffsetPoint(picaVoxelPoint, 1, 0, 0);
            PicaVoxelPoint x2 = GetOffsetPoint(picaVoxelPoint, -1, 0, 0);
            PicaVoxelPoint y1 = GetOffsetPoint(picaVoxelPoint, 0, 1, 0);
            PicaVoxelPoint y2 = GetOffsetPoint(picaVoxelPoint, 0, -1, 0);
            PicaVoxelPoint z1 = GetOffsetPoint(picaVoxelPoint, 0, 0, 1);
            PicaVoxelPoint z2 = GetOffsetPoint(picaVoxelPoint, 0, 0, -1);
            return new PicaVoxelPoint[] { x1, x2, y1, y2, z1, z2 };
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}