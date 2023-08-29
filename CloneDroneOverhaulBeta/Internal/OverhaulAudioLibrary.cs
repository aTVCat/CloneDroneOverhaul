using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul
{
    public class OverhaulAudioLibrary : OverhaulManager<OverhaulAudioLibrary>
    {
        public static bool HasLoadedSounds;

        public static AudioClipDefinition[] HeavyRobotFootsteps;

        protected override void OnAssetsLoaded()
        {
            if (HasLoadedSounds)
                return;

            HasLoadedSounds = true;
            HeavyRobotFootsteps = new AudioClipDefinition[2];
            HeavyRobotFootsteps[0] = OverhaulAssetsController.GetAsset<AudioClip>("HeavyFootStep1", OverhaulAssetPart.Sounds).ToAudioClipDefinition();
            HeavyRobotFootsteps[1] = OverhaulAssetsController.GetAsset<AudioClip>("HeavyFootStep2", OverhaulAssetPart.Sounds).ToAudioClipDefinition();
        }
    }
}
