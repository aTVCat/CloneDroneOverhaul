using OverhaulMod.Utils;
using PicaVoxel;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class FadingVoxelManager : Singleton<FadingVoxelManager>
    {
        [ModSetting(ModSettingsConstants.ENABLE_VOXEL_FIRE_FADING, true)]
        public static bool EnableFading;

        [ModSetting(ModSettingsConstants.ENABLE_VOXEL_BURNING, true)]
        public static bool EnableBurning;

        private List<FadingVoxel> m_voxelsToFade;

        public float multiplier
        {
            get;
            set;
        } = 0.94f;

        public float timeToDestroyOffset
        {
            get;
            set;
        } = 1.5f;

        public int fadingVoxelsLimit
        {
            get;
            set;
        } = 1250;

        public float randomFadeMultiplierMin
        {
            get;
            set;
        } = 0.98f;

        private void Start()
        {
            m_voxelsToFade = new List<FadingVoxel>();
        }

        private void Update()
        {
            if (ModTime.hasFixedUpdated && ModTime.fixedFrameCount % 10 == 0)
            {
                UpdateFading();
            }
        }

        public void AddFadingVoxel(PicaVoxelPoint picaVoxelPoint, MechBodyPart mechBodyPart, float timeToDestroy)
        {
            if (m_voxelsToFade.Count > fadingVoxelsLimit)
                return;

            FadingVoxel fadingVoxel = new FadingVoxel()
            {
                Point = picaVoxelPoint,
                BodyPart = mechBodyPart,
                TimeToDestroy = timeToDestroy
            };
            m_voxelsToFade.Add(fadingVoxel);
        }

        public byte BurnColor(byte color) => (byte)Mathf.RoundToInt(color * ModCache.attackManager.FireBurnColorMultiplier);

        public PicaVoxelPoint GetOffsetPoint(in PicaVoxelPoint picaVoxelPoint, in int OffX, in int OffY, in int OffZ) => new PicaVoxelPoint(picaVoxelPoint.X + OffX, picaVoxelPoint.Y + OffY, picaVoxelPoint.Z + OffZ);

        public PicaVoxelPoint[] GetSurroundingPoints(in PicaVoxelPoint picaVoxelPoint)
        {
            PicaVoxelPoint x1 = GetOffsetPoint(picaVoxelPoint, 1, 0, 0);
            PicaVoxelPoint x2 = GetOffsetPoint(picaVoxelPoint, -1, 0, 0);
            PicaVoxelPoint y1 = GetOffsetPoint(picaVoxelPoint, 0, 1, 0);
            PicaVoxelPoint y2 = GetOffsetPoint(picaVoxelPoint, 0, -1, 0);
            PicaVoxelPoint z1 = GetOffsetPoint(picaVoxelPoint, 0, 0, 1);
            PicaVoxelPoint z2 = GetOffsetPoint(picaVoxelPoint, 0, 0, -1);
            return new PicaVoxelPoint[6] { x1, x2, y1, y2, z1, z2 };
        }

        public void UpdateFading()
        {
            if (m_voxelsToFade == null || m_voxelsToFade.Count == 0)
                return;

            int index = 0;
            List<int> indicesToRemove = new List<int>();
            foreach (FadingVoxel fadingVoxel in m_voxelsToFade)
            {
                if (fadingVoxel.TimeToDestroy <= Time.time)
                {
                    indicesToRemove.Add(index);
                    index++;
                    continue;
                }

                MechBodyPart mechBodyPart = fadingVoxel.BodyPart;
                if (mechBodyPart)
                {
                    Frame frame = mechBodyPart._currentFrame;
                    if (frame)
                    {
                        Voxel? voxelAtArrayPosition = frame.GetVoxelAtArrayPosition(fadingVoxel.Point);
                        if (voxelAtArrayPosition != null)
                        {
                            Voxel voxel = voxelAtArrayPosition.Value;
                            if (voxel.Active)
                            {
                                Color color = voxel.Color;
                                color *= multiplier * Random.Range(randomFadeMultiplierMin, 1f);
                                voxel.Color = color;
                                frame.SetVoxelAtArrayPosition(fadingVoxel.Point, voxel);
                            }
                        }
                    }
                }
                index++;
            }

            if (indicesToRemove.Count != 0)
            {
                int i = indicesToRemove.Count - 1;
                do
                {
                    m_voxelsToFade.RemoveAt(indicesToRemove[i]);
                    i--;
                } while (i > -1);
            }
        }
    }
}
