using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Shared
{
    public class CustomRobotAnimation : OverhaulDataBase
    {
        public string AnimationName;
        public List<CustomRobotAnimationTrack> Tracks;

        public override void RepairFields()
        {
            if(Tracks == null)
            {
                Tracks = new List<CustomRobotAnimationTrack>();
            }
            if(AnimationName == null)
            {
                AnimationName = "DefaultAnimation";
            }
        }

        /// <summary>
        /// Set animation frame and update robot model
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="modelToUpdate"></param>
        public void SetFrame(in int frame, in CharacterModel modelToUpdate)
        {
            if (modelToUpdate.UpperAnimator == null)
            {
                return;
            }

            foreach (CustomRobotAnimationTrack track in Tracks)
            {
                Transform t = TransformUtils.FindChildRecursive(modelToUpdate.UpperAnimator.transform, track.BodyPartName);
                if (t != null)
                {
                    CustomRobotAnimationKeyframe keyframe = track.GetKeyframeAt(frame);
                    if (keyframe != null)
                    {
                        t.localEulerAngles = keyframe.Rotation;
                    }
                }
            }
        }

        /// <summary>
        /// Called when new animation is created
        /// </summary>
        public void InitializeAnimationAsNew()
        {
            foreach (string str in CustomAnimationsController.GetAllBodyParts())
            {
                CustomRobotAnimationTrack track = new CustomRobotAnimationTrack();
                track.InitializeAsNewTrack();
                track.BodyPartName = str;
                Tracks.Add(track);
            }
        }

        #region Static

        /// <summary>
        /// Create and save animation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CustomRobotAnimation CreateNewAnimation(in string name)
        {
            CustomRobotAnimation anim = new CustomRobotAnimation
            {
                AnimationName = name,
                FileName = name
            };
            anim.RepairFields();
            anim.InitializeAnimationAsNew();
            SaveAnimation(anim);
            return anim;
        }

        /// <summary>
        /// Save animation in Assets/Animations/ folder
        /// </summary>
        /// <param name="animation"></param>
        public static void SaveAnimation(in CustomRobotAnimation animation)
        {
            animation.SaveData(true, "Animations");
        }

        #endregion
    }
}
