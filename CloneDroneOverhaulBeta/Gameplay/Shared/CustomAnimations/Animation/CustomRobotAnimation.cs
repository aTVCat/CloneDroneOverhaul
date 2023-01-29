using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Shared
{
    public class CustomRobotAnimation : ModDataContainerBase
    {
        public string AnimationName;

        public List<CustomRobotAnimationTrack> Tracks;

        public void SetFrame(in int frame, in CharacterModel modelToUpdate)
        {
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

        public void InitializeAnimationAsNew()
        {
            if (Tracks == null)
            {
                Tracks = new List<CustomRobotAnimationTrack>();
            }
            foreach (string str in CustomAnimationsController.GetAllBodyParts())
            {
                CustomRobotAnimationTrack track = new CustomRobotAnimationTrack();
                track.InitializeAsNewTrack();
                Tracks.Add(track);
            }
        }

        public static CustomRobotAnimation CreateNewAnimation(in string name)
        {
            CustomRobotAnimation anim = new CustomRobotAnimation
            {
                AnimationName = name
            };
            anim.RepairMissingFields();
            anim.InitializeAnimationAsNew();
            SaveAnimation(anim);
            return anim;
        }

        public static void SaveAnimation(in CustomRobotAnimation animation)
        {
            animation.SaveData<CustomRobotAnimation>(true, "Animations/");
        }
    }
}
