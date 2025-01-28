using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Content
{
    public class AddonDownloadInfo
    {
        public AddonInfo Addon;

        public string UniqueID;

        public string PackageFileURL;

        /// <summary>
        /// The size in bytes
        /// </summary>
        public long PackageFileSize;

        public List<string> Images;

        public bool HasAddonInfo()
        {
            return Addon != null;
        }

        public void SetAddon()
        {
            SetAddon(AddonManager.Instance.GetAddonInfo(UniqueID));
        }

        public void SetAddon(AddonInfo addonInfo)
        {
            Addon = addonInfo;
        }

        public string GetDisplayName()
        {
            return Addon?.GetDisplayName(LocalizationManager.Instance.GetCurrentLanguageCode());
        }

        public string GetDescription()
        {
            return Addon?.GetDescription(LocalizationManager.Instance.GetCurrentLanguageCode());
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
