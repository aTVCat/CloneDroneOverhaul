using OverhaulAPI;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulAudioLibrary
    {
        private static AudioClipDefinition[] m_HeavyRobotFootsteps;
        public static AudioClipDefinition[] HeavyRobotFootsteps => m_HeavyRobotFootsteps;

        internal static void Initialize()
        {
            if (OverhaulSessionController.GetKey<bool>("HasInitialized"))
            {
                return;
            }
            OverhaulSessionController.SetKey("HasInitialized", true);

            m_HeavyRobotFootsteps = new AudioClipDefinition[2];
            m_HeavyRobotFootsteps[0] = AudioAPI.CreateDefinitionUsingClip(AssetsController.GetAsset<AudioClip>("HeavyFootStep1", OverhaulAssetsPart.Sounds));
            m_HeavyRobotFootsteps[1] = AudioAPI.CreateDefinitionUsingClip(AssetsController.GetAsset<AudioClip>("HeavyFootStep2", OverhaulAssetsPart.Sounds));
        }
    }
}
