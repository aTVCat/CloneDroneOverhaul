using PicaVoxel;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public static class VRVoxelBurnEffectController
    {
        public static float RGBMultiplier = 0.6f;
        public static float AlphaMultiplier = 1.4f;

        private static readonly Dictionary<int, List<PicaVoxelPoint>> s_BurningVoxels = new Dictionary<int, List<PicaVoxelPoint>>();
        private static Voxel s_Voxel = new Voxel() { State = VoxelState.Active };

        public static void SetPointOnFire(Frame frame, PicaVoxelPoint point)
        {
            if (!frame || point == null)
                return;

            /*
            Voxel? voxelAtArrayPosition = frame.GetVoxelAtArrayPosition(point);
            if (voxelAtArrayPosition == null || voxelAtArrayPosition.Value.State != VoxelState.Active)
                return;*/

            //s_Voxel.Color = AttackManager.Instance.BodyOnFireColor;
            //s_Voxel.State = VoxelState.Active;
            //frame.SetVoxelAtArrayPosition(point, s_Voxel);
            registerFrameAndPoint(frame, point);
        }

        private static void registerFrameAndPoint(Frame frame, PicaVoxelPoint point)
        {
            int instanceId = frame.GetInstanceID();
            if (s_BurningVoxels.ContainsKey(instanceId))
            {
                s_BurningVoxels[instanceId].Add(point);
                return;
            }

            List<PicaVoxelPoint> list = new List<PicaVoxelPoint>
            {
                point
            };
            s_BurningVoxels.Add(instanceId, list);
        }

        public static void UpdateFrame(Frame frame)
        {
            if (!frame)
                return;

            int instanceId = frame.GetInstanceID();
            if (!s_BurningVoxels.ContainsKey(instanceId))
                return;

            List<PicaVoxelPoint> list = s_BurningVoxels[instanceId];
            if (list.IsNullOrEmpty())
                return;

            int index = 0;
            do
            {
                updatePoint(frame, list[index], list);
                index++;
            } while (index < list.Count);
        }

        private static void updatePoint(Frame frame, PicaVoxelPoint point, List<PicaVoxelPoint> list)
        {
            if (point == null)
                return;

            Voxel? voxelAtArrayPosition = frame.GetVoxelAtArrayPosition(point);
            if (voxelAtArrayPosition == null)
                return;

            Color32 color = voxelAtArrayPosition.Value.Color;
            color.r = getColorValue(color.r, RGBMultiplier);
            color.g = getColorValue(color.g, RGBMultiplier);
            color.b = getColorValue(color.b, RGBMultiplier);
            color.a = getColorValue(color.a, AlphaMultiplier);
            if (getColorGeneralNumber(color) < 4)
            {
                _ = list.Remove(point);
                if (list.IsNullOrEmpty())
                {
                    _ = s_BurningVoxels.Remove(frame.GetInstanceID());
                }
            }

            s_Voxel.Color = color;
            s_Voxel.State = voxelAtArrayPosition.Value.State;
            frame.SetVoxelAtArrayPosition(point, s_Voxel);
        }

        public static void UpdatePointColor(Frame frame, PicaVoxelPoint point)
        {
            if (point == null)
                return;

            Voxel? voxelAtArrayPosition = frame.GetVoxelAtArrayPosition(point);
            if (voxelAtArrayPosition == null)
                return;

            Color32 color = voxelAtArrayPosition.Value.Color;
            if (getColorGeneralNumber(color) < 4)
                return;

            color.r = getColorValue(color.r, RGBMultiplier);
            color.g = getColorValue(color.g, RGBMultiplier);
            color.b = getColorValue(color.b, RGBMultiplier);
            color.a = getColorValue(color.a, AlphaMultiplier);

            s_Voxel.Color = color;
            s_Voxel.State = voxelAtArrayPosition.Value.State;
            frame.SetVoxelAtArrayPosition(point, s_Voxel);
        }

        private static byte getColorValue(byte color, float multiplier)
        {
            return (byte)Mathf.RoundToInt(color * multiplier);
        }

        private static int getColorGeneralNumber(Color32 color32)
        {
            return color32.r + color32.g + color32.b;
        }
    }
}
