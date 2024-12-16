using System;
using UnityEngine.Serialization;

namespace OverhaulMod.Content
{
    public class AddonInfo
    {
        public string DisplayName;

        public int Version;

        [NonSerialized]
        public string FolderPath;
    }
}
