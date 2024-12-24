using OverhaulMod.Utils;
using System;
using System.IO;

namespace OverhaulMod.Content
{
    public class AddonDownloadInfo
    {
        public AddonInfo Info;

        public string Images;

        public Version MinModVersion;

        public string PackageFileURL;

        public long PackageFileSize;

        public AddonDownloadInfo()
        {

        }

        public AddonDownloadInfo(AddonInfo info)
        {
            Info = info;
        }

        public string GetDisplayName()
        {
            return Info?.GetDisplayName();
        }

        public string GetDisplayName(string language)
        {
            return Info?.GetDisplayName(language);
        }

        public string GetDescription()
        {
            return Info?.GetDescription();
        }

        public string GetDescription(string language)
        {
            return Info?.GetDescription(language);
        }

        public string GetUniqueID()
        {
            return Info?.UniqueID;
        }

        public int GetVersion()
        {
            if (Info == null)
                return -1;

            return Info.Version;
        }

        public string[] GetImages()
        {
            if (Images.IsNullOrEmpty())
                return Array.Empty<string>();

            return StringUtils.GetNonEmptySplitOfCommaSeparatedString(Images);
        }

        public bool IsSupported()
        {
            return MinModVersion != null && ModBuildInfo.version >= MinModVersion;
        }

        public void CalculatePackageFileSize(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!fileInfo.Exists)
            {
                PackageFileSize = -1L;
                return;
            }
            PackageFileSize = fileInfo.Length;
        }
    }
}
