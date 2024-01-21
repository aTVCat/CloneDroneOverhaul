using UnityEngine;

namespace OverhaulAPI
{
    public static class AudioUtils
    {
        public static AudioClipDefinition ToAudioClipDefinition(this AudioClip clip)
        {
            AudioClipDefinition def = new AudioClipDefinition
            {
                Clip = clip
            };
            return def;
        }
    }
}
