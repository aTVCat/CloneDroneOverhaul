using System;

namespace OverhaulMod.Content
{
    public class ContentInfo
    {
        public string DisplayName;

        public int Version;

        [NonSerialized]
        public string FolderPath;
    }
}
