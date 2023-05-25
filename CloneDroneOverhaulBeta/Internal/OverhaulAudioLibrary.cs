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
            HeavyRobotFootsteps[0] = AudioAPI.CreateDefinitionUsingClip(OverhaulAssetsController.GetAsset<AudioClip>("HeavyFootStep1", OverhaulAssetPart.Sounds));
            HeavyRobotFootsteps[1] = AudioAPI.CreateDefinitionUsingClip(OverhaulAssetsController.GetAsset<AudioClip>("HeavyFootStep2", OverhaulAssetPart.Sounds));
        }
    }
}
