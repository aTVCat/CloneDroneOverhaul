using System;
using System.Collections.Generic;
using System.IO;

namespace CDOverhaul.NetworkAssets.AdditionalContent
{
    [Serializable]
    public class OverhaulAdditionalContentPackInfo
    {
        public OverhaulAdditionalContentPackInfo() { }
        public string GetPackFolderName()
        {
            return PackName.Replace(" ", "_");
        }

        public string GetThumbnailFileName()
        {
            return "Thumbnail.png";
        }

        public string GetPackFolder()
        {
            return OverhaulAdditionalContentController.AdditionalContentDirectory + PreviousFolderName + "/";
        }

        // The name and description won't be translated
        public string PackName;
        public string PackDescription;
        public string PackID;
        public ContentPackType PackType;
        public Version PackVersion;

        public Version MinModVersionRequired;

        public List<string> AssetBundles;

        [NonSerialized]
        public string PreviousFolderName;

        public bool IsCompatibleWithMod()
        {
            return OverhaulVersion.ModVersion >= MinModVersionRequired;
        }

        public void SaveThis()
        {
            if (string.IsNullOrEmpty(PreviousFolderName))
            {
                return;
            }

            string prevPath = OverhaulAdditionalContentController.AdditionalContentDirectory + PreviousFolderName + "/";
            string newPath = OverhaulAdditionalContentController.AdditionalContentDirectory + GetPackFolderName() + "/";
            if (!Directory.Exists(prevPath))
            {
                Directory.CreateDirectory(newPath);
                prevPath = newPath;
            }
            else
            {
                if (!Equals(PreviousFolderName, GetPackFolderName())) Directory.Move(prevPath, newPath);
            }
            PreviousFolderName = GetPackFolderName();

            File.WriteAllText(newPath + "ContentInfo.json", Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));
        }

        public static OverhaulAdditionalContentPackInfo CreateNew()
        {
            Version version = (Version)OverhaulVersion.ModVersion.Clone();
            OverhaulAdditionalContentPackInfo packInfo = new OverhaulAdditionalContentPackInfo()
            {
                PackName = "Sample content pack",
                PackDescription = "This pack is under construction.",
                PackType = ContentPackType.Network,
                PackVersion = version,
                PackID = Guid.NewGuid().ToString(),
                MinModVersionRequired = version,
                AssetBundles = new List<string>()
            };
            packInfo.PreviousFolderName = packInfo.GetPackFolderName();

            // saving content pack on disk
            try
            {
                string newDirectoryPath = OverhaulAdditionalContentController.AdditionalContentDirectory + packInfo.GetPackFolderName() + "/";
                Directory.CreateDirectory(newDirectoryPath);
                File.WriteAllText(newDirectoryPath + "ContentInfo.json", Newtonsoft.Json.JsonConvert.SerializeObject(packInfo, Newtonsoft.Json.Formatting.Indented));
            }
            catch
            {
                return null;
            }

            return packInfo;
        }

        public enum ContentPackType
        {
            PreInstalled,
            Network
        }
    }
}
