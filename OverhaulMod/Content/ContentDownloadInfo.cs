using System;
using System.Collections.Generic;

namespace OverhaulMod.Content
{
    public class ContentDownloadInfo
    {
        public string DisplayName;
        public long Size;
        public Version MinModVersion;

        public string File;

        public List<string> Files;

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
