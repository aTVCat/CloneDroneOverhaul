using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Shared
{
    public class CustomRobotAnimationTrack
    {
        public string BodyPartName;

        public List<CustomRobotAnimationKeyframe> Keyframes;

        [NonSerialized]
        private readonly Dictionary<int, int> _cachedPositions = new Dictionary<int, int>();

        public void InitializeAsNewTrack()
        {
            Keyframes = new List<CustomRobotAnimationKeyframe>();
        }

        public CustomRobotAnimationKeyframe GetKeyframeAt(in int frame)
        {
            CustomRobotAnimationKeyframe result = null;
            if (_cachedPositions.ContainsKey(frame))
            {
                result = Keyframes[_cachedPositions[frame]];
                return result;
            }

            int index = 0;
            foreach (CustomRobotAnimationKeyframe kFrame in Keyframes)
            {
                if (kFrame.Frame == frame)
                {
                    result = kFrame;
                    _cachedPositions.Add(frame, index);
                }
                index++;
            }
            if (result == null)
            {
                result = GetSmoothKeyframeAt(frame);
            }
            return result;
        }

        public CustomRobotAnimationKeyframe GetSmoothKeyframeAt(in int frame)
        {
            CustomRobotAnimationKeyframe k = null;

            int index = 0;
            int prevKeyframe = 0;
            int nextKeyframe = 720; // Animation length
            foreach (CustomRobotAnimationKeyframe keyframe in Keyframes)
            {
                if (keyframe.Frame == frame)
                {
                    return keyframe;
                }
                if (keyframe.Frame < frame && keyframe.Frame > prevKeyframe)
                {
                    prevKeyframe = keyframe.Frame;
                }
                if (keyframe.Frame > frame && keyframe.Frame < nextKeyframe)
                {
                    nextKeyframe = keyframe.Frame;
                }
                index++;
            }
            OverhaulDebugController.Print("Prev keyframe: " + prevKeyframe, Color.white);
            OverhaulDebugController.Print("Next keyframe: " + nextKeyframe, Color.white);

            Vector3 rotation = GetKeyframeAt(nextKeyframe).Rotation - GetKeyframeAt(prevKeyframe).Rotation;
            rotation = rotation / (nextKeyframe - prevKeyframe);

            k = CustomRobotAnimationKeyframe.NewKeyframe(frame);
            k.IsSmooth = true;
            k.Rotation = GetKeyframeAt(prevKeyframe).Rotation + (rotation * (frame - prevKeyframe));
            return k;
        }

        public bool HasKeyFrameAt(in int frame)
        {
            return GetKeyFrameListPositionAt(frame) != -1;
        }

        public int GetKeyFrameListPositionAt(in int frame) // Optimize this
        {
            int index = 0;
            foreach (CustomRobotAnimationKeyframe kFrame in Keyframes)
            {
                if (kFrame.Frame == frame)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public CustomRobotAnimationKeyframe CreateKeyframeAt(in int frame)
        {
            int alreadyExistingKFrame = GetKeyFrameListPositionAt(frame);
            if (alreadyExistingKFrame != -1)
            {
                CustomRobotAnimationKeyframe aKFrame = Keyframes[alreadyExistingKFrame];
                return aKFrame;
            }
            CustomRobotAnimationKeyframe newKF = CustomRobotAnimationKeyframe.NewKeyframe(frame);
            Keyframes.Add(newKF);
            return newKF;
        }

        public void RemoveKeyframeAt(in int frame)
        {
            int alreadyExistingKFrame = GetKeyFrameListPositionAt(frame);
            if (alreadyExistingKFrame != -1)
            {
                Keyframes.RemoveAt(alreadyExistingKFrame);
            }
        }
    }
}
