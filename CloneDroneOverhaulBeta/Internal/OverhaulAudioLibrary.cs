using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulAudioLibrary
    {
        public static AudioClipDefinition[] HeavyRobotFootsteps { get; private set; }

        internal static void Initialize()
        {
            if (OverhaulSessionController.GetKey<bool>("HasInitialized"))
            {
                return;
            }
            OverhaulSessionController.SetKey("HasInitialized", true);

            HeavyRobotFootsteps = new AudioClipDefinition[2];
            HeavyRobotFootsteps[0] = AudioAPI.CreateDefinitionUsingClip(AssetsController.GetAsset<AudioClip>("HeavyFootStep1", OverhaulAssetsPart.Sounds));
            HeavyRobotFootsteps[1] = AudioAPI.CreateDefinitionUsingClip(AssetsController.GetAsset<AudioClip>("HeavyFootStep2", OverhaulAssetsPart.Sounds));
        }
    }
}
