using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace OverhaulMod.Content
{
    public class AddonInfo
    {
        public Dictionary<string, string> DisplayName;

        public Dictionary<string, string> Description;

        public string Images;

        public string UniqueID;

        public int Version;

        public Version MinModVersion;

        public string PackageFileURL;

        public long PackageFileSize;

        [NonSerialized]
        public string FolderPath;

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

        public void GenerateUniqueID()
        {
            UniqueID = Guid.NewGuid().ToString();
        }

        public string GetDisplayName()
        {
            return GetDisplayName(LocalizationManager.Instance.GetCurrentLanguageCode());
        }

        public string GetDisplayName(string language)
        {
            if (DisplayName.IsNullOrEmpty())
                return null;

            if (!DisplayName.ContainsKey(language))
            {
                if (DisplayName.ContainsKey("en"))
                {
                    return DisplayName["en"];
                }
                return null;
            }
            return DisplayName[language];
        }

        public string GetDescription()
        {
            return GetDescription(LocalizationManager.Instance.GetCurrentLanguageCode());
        }

        public string GetDescription(string language)
        {
            if (Description.IsNullOrEmpty())
                return null;

            if (!Description.ContainsKey(language))
            {
                if (Description.ContainsKey("en"))
                {
                    return Description["en"];
                }
                return null;
            }
            return Description[language];
        }

        public string[] GetImages()
        {
            if (Images.IsNullOrEmpty())
                return Array.Empty<string>();

            return StringUtils.GetNonEmptySplitOfCommaSeparatedString(Images);
        }
    }
}
