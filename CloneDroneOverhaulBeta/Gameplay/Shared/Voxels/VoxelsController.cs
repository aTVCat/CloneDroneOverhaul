using PicaVoxel;
using UnityEngine;

namespace CDOverhaul.Shared
{
    public class VoxelsController : ModController
    {
        public static float ColorBurnMultipler;

        [OverhaulSetting("TestCat.TestSec.TestSet", true, true)]
        public static bool NonFireCutBurnEnabled;

        public override void Initialize()
        {
            ColorBurnMultipler = AttackManager.Instance.FireBurnColorMultiplier;

            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        public static void OnVoxelDestroy(MechBodyPart bodyPart, PicaVoxelPoint picaVoxelPoint, Voxel? voxelAtPosition, Vector3 impactDirectionWorld, FireSpreadDefinition fireSpreadDefinition, Frame currentFrame)
        {
            PicaVoxelPoint x1 = new PicaVoxelPoint(picaVoxelPoint.X + 1, picaVoxelPoint.Y + 0, picaVoxelPoint.Z + 0);
            PicaVoxelPoint x2 = new PicaVoxelPoint(picaVoxelPoint.X - 1, picaVoxelPoint.Y - 0, picaVoxelPoint.Z - 0);
            PicaVoxelPoint y1 = new PicaVoxelPoint(picaVoxelPoint.X + 0, picaVoxelPoint.Y + 1, picaVoxelPoint.Z + 0);
            PicaVoxelPoint y2 = new PicaVoxelPoint(picaVoxelPoint.X - 0, picaVoxelPoint.Y - 1, picaVoxelPoint.Z - 0);
            PicaVoxelPoint z1 = new PicaVoxelPoint(picaVoxelPoint.X + 0, picaVoxelPoint.Y + 0, picaVoxelPoint.Z + 1);
            PicaVoxelPoint z2 = new PicaVoxelPoint(picaVoxelPoint.X - 0, picaVoxelPoint.Y - 0, picaVoxelPoint.Z - 1);

            foreach (PicaVoxelPoint p in new PicaVoxelPoint[] { x1, x2, y1, y2, z1, z2 })
            {
                if (!bodyPart.IsVoxelWaitingToBeDestroyed(p))
                {
                    Voxel? vox = currentFrame.GetVoxelAtArrayPosition(p);
                    if (vox != null)
                    {
                        Voxel newVox = vox.Value;
                        Color32 color = new Color32((byte)Mathf.RoundToInt(vox.Value.Color.r * ColorBurnMultipler), (byte)Mathf.RoundToInt(vox.Value.Color.g * ColorBurnMultipler), (byte)Mathf.RoundToInt(vox.Value.Color.b * ColorBurnMultipler), vox.Value.Color.a);
                        newVox.Color = color;
                        currentFrame.SetVoxelAtArrayPosition(p, newVox);
                    }
                }
            }
        }
    }
}