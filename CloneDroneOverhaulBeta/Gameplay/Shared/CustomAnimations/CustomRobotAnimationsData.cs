using System;
using System.Collections.Generic;
using System.IO;

namespace CDOverhaul.Shared
{
    public class CustomRobotAnimationsData : ModDataContainerBase
    {
        public static string[] AllAnimationsUnderModAssetsFolder => Directory.GetFiles(OverhaulMod.Core.ModDirectory + "Assets/Animations/");

        [NonSerialized]
        public static Dictionary<string, CustomRobotAnimation> LoadedAnimations;

        public override void RepairFields()
        {
            if (LoadedAnimations == null)
            {
                LoadedAnimations = new Dictionary<string, CustomRobotAnimation>();
            }
        }

        /// <summary>
        /// All animations are not usually loaded into memory 
        /// </summary>
        /// <param name="animName"></param>
        /// <returns></returns>
        public bool IsAnimationLoaded(in string animName)
        {
            return LoadedAnimations.ContainsKey(animName);
        }

        public CustomRobotAnimation GetAnimation(in string animName)
        {
            CustomRobotAnimation anim;
            if (!IsAnimationLoaded(animName))
            {
                anim = ModDataContainerBase.GetData<CustomRobotAnimation>(animName, true, "Animations");
                LoadedAnimations.Add(animName, anim);
            }
            else
            {
                anim = LoadedAnimations[animName];
            }
            return anim;
        }
    }
}
