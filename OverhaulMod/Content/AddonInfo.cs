using OverhaulMod.Utils;
using System;
using System.Collections.Generic;

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

        [NonSerialized]
        public string FolderPath;

        public bool IsSupported()
        {
            return MinModVersion != null && ModBuildInfo.version >= MinModVersion;
        }

        public void GenerateUniqueID()
        {
            UniqueID = Guid.NewGuid().ToString().Remove(8);
        }

        public string GetDisplayName()
        {
            return GetDisplayName(LocalizationManager.Instance.GetCurrentLanguageCode());
        }

        public string GetDisplayName(string language, bool returnNullIfNotPresent = false)
        {
            if (DisplayName.IsNullOrEmpty())
                return null;

            if (!DisplayName.ContainsKey(language))
            {
                if (returnNullIfNotPresent)
                    return null;

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

        public string GetDescription(string language, bool returnNullIfNotPresent = false)
        {
            if (Description.IsNullOrEmpty())
                return null;

            if (!Description.ContainsKey(language))
            {
                if (returnNullIfNotPresent)
                    return null;

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
