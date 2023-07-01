using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulAudioLibrary
    {
        public static bool HasLoadedSounds;

        public static AudioClipDefinition[] HeavyRobotFootsteps { get; private set; }

        internal static void Initialize()
        {
            if (HasLoadedSounds)
                return;

            HasLoadedSounds = true;
            HeavyRobotFootsteps = new AudioClipDefinition[2];
            HeavyRobotFootsteps[0] = AudioAPI.CreateDefinitionUsingClip(OverhaulAssetsController.GetAsset<AudioClip>("HeavyFootStep1", OverhaulAssetPart.Sounds));
            HeavyRobotFootsteps[1] = AudioAPI.CreateDefinitionUsingClip(OverhaulAssetsController.GetAsset<AudioClip>("HeavyFootStep2", OverhaulAssetPart.Sounds));
        }
    }
}
