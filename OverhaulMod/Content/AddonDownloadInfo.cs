using OverhaulMod.Utils;
using System;
using System.Collections.Generic;

namespace OverhaulMod.Content
{
    public class AddonDownloadInfo
    {
        public string DisplayName, Description, Images;
        public long Size;
        public int Version;
        public Version MinModVersion;

        public string File;

        public List<string> Files;

        public string[] GetImages()
        {
            if (Images.IsNullOrEmpty())
                return Array.Empty<string>();

            return Images.Split(',');
        }

        public void FixValues()
        {
            if (Files == null)
                Files = new List<string>();
        }

        public bool IsSupported()
        {
            return MinModVersion != null && ModBuildInfo.version >= MinModVersion;
        }
    }
}
