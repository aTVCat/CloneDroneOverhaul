using System;

namespace OverhaulMod.Content
{
    /// <summary>
    /// Old version of <see cref="AddonDownloadInfo"/>. It is used to store legacy addons data
    /// </summary>
    public class ContentInfo
    {
        public string DisplayName;

        public int Version;

        [NonSerialized]
        public string FolderPath;
    }
}
