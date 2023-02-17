using UnityEngine;

namespace OverhaulAPI
{
    public static class AudioAPI
    {
        public static AudioClipDefinition CreateDefinitionUsingClip(in AudioClip clip)
        {
            AudioClipDefinition def = new AudioClipDefinition
            {
                Clip = clip
            };
            return def;
        }
    }
}
