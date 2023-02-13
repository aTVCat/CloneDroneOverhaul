using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulAPI
{
    public static class AudioAPI
    {
        public static AudioClipDefinition CreateDefinitionUsingClip(in AudioClip clip)
        {
            AudioClipDefinition def = new AudioClipDefinition();
            def.Clip = clip;
            return def;
        }
    }
}
