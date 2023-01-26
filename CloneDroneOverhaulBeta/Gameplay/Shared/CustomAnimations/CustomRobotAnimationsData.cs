using System;
using System.Collections.Generic;
using System.IO;

namespace CDOverhaul.Shared
{
    public class CustomRobotAnimationsData : ModDataContainerBase
    {
        public string[] AllAnimationsUnderModAssetsFolder => Directory.GetFiles(OverhaulBase.Core.ModFolder + "Assets/Animations/");

        [NonSerialized]
        public Dictionary<string, CustomRobotAnimation> LoadedAnimations;

        public override void RepairMissingFields()
        {
            if (LoadedAnimations == null)
            {
                LoadedAnimations = new Dictionary<string, CustomRobotAnimation>();
            }
        }

        public bool IsAnimationLoaded(in string animName)
        {
            return LoadedAnimations.ContainsKey(animName);
        }

        public CustomRobotAnimation GetAnimation(in string animName)
        {
            CustomRobotAnimation anim = null;
            if (!IsAnimationLoaded(animName))
            {
                anim = ModDataContainerBase.GetData<CustomRobotAnimation>(animName, true, "Animations/");
                LoadedAnimations.Add(animName, anim);
                return anim;
            }
            anim = LoadedAnimations[animName];
            return anim;
        }

        public CustomRobotAnimation NewAnimation(in string animName)
        {
            CustomRobotAnimation anim = new CustomRobotAnimation
            {
                AnimationName = animName
            };
            anim.RepairMissingFields();
            SaveAnimation(anim);
            return anim;
        }

        public void SaveAnimation(in CustomRobotAnimation anim)
        {
            anim.FileName = anim.AnimationName;
            anim.SaveData<CustomRobotAnimation>(true, "Animations/");
        }
    }
}
