using System;
using UnityEngine;

namespace CDOverhaul.Shared
{
    public class CustomRobotAnimationKeyframe
    {
        public int Frame;

        public Vector3 Rotation;

        [NonSerialized]
        public bool IsSmooth;

        public static CustomRobotAnimationKeyframe NewKeyframe(in int frame)
        {
            CustomRobotAnimationKeyframe k = new CustomRobotAnimationKeyframe
            {
                Frame = frame,
                Rotation = Vector3.zero
            };
            return k;
        }
    }
}
