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

        /// <summary>
        /// Called when new track is created
        /// </summary>
        public void InitializeAsNewTrack()
        {
            Keyframes = new List<CustomRobotAnimationKeyframe>();
            CreateKeyframeAt(0);
        }

        /// <summary>
        /// Get keyframe at <paramref name="frame"/> and if it is null, try get smooth keyframe between two ones if <paramref name="getSmoothKeyframes"/> set to true
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="getSmoothKeyframes"></param>
        /// <returns></returns>
        public CustomRobotAnimationKeyframe GetKeyframeAt(in int frame, in bool getSmoothKeyframes = true)
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
            if (getSmoothKeyframes && result == null)
            {
                result = GetSmoothKeyframeAt(frame);
            }
            return result;
        }

        /// <summary>
        /// Get a keyframe between 2 ones
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
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

            CustomRobotAnimationKeyframe next = GetKeyframeAt(nextKeyframe, false);
            CustomRobotAnimationKeyframe prev = GetKeyframeAt(prevKeyframe, false);

            if (prev == null)
            {
                if (next == null)
                {
                    return null;
                }
                return next;
            }
            if (next == null)
            {
                if (prev == null)
                {
                    return null;
                }
                return prev;
            }

            Vector3 rotation = next.Rotation - prev.Rotation;
            rotation = rotation / (nextKeyframe - prevKeyframe);

            k = CustomRobotAnimationKeyframe.NewKeyframe(frame);
            k.IsSmooth = true;
            k.Rotation = GetKeyframeAt(prevKeyframe).Rotation + (rotation * (frame - prevKeyframe));
            return k;
        }

        /// <summary>
        /// Check if user has placed keyframe at frame
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool HasKeyFrameAt(in int frame)
        {
            return GetKeyFrameListPositionAt(frame) != -1;
        }

        /// <summary>
        /// Get index of keyframe
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create new keyframe
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Remove keyframe
        /// </summary>
        /// <param name="frame"></param>
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
