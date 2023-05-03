using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CDOverhaul.NetworkAssets.AdditionalContent
{
    [Serializable]
    public class OverhaulAdditionalContentPackInfo
    {
        public OverhaulAdditionalContentPackInfo() { }

        public void SetEnabled(bool value, bool refresh)
        {
            if (!IsCompatibleWithMod() || OverhaulAdditionalContentController.UserData == null)
            {
                return;
            }

            if (value)
            {
                OverhaulAdditionalContentController.UserData.EnableContentPack(this);
            }
            else
            {
                OverhaulAdditionalContentController.UserData.DisableContentPack(this);
            }

            if (refresh)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            if(!IsCompatibleWithMod() || !IsInstalled())
            {
                return;
            }

            if (!IsLoaded() && IsEnabled())
            {
                Load();
            }
            else if (IsLoaded() && !IsEnabled())
            {
                Unload();
            }
        }

        #region Information

        [NonSerialized]
        public string PreviousFolderName;

        // The name and description won't be translated
        public string PackName;
        public string PackDescription;
        public string PackID;
        public ContentPackType PackType;
        public Version PackVersion;

        public Version MinModVersionRequired;

        public List<string> AssetBundles;
        public void AddAssetBundle(string assetBundle)
        {
            if (AssetBundles == null)
            {
                AssetBundles = new List<string>();
            }
            if (!AssetBundles.IsNullOrEmpty() && AssetBundles.Contains(assetBundle))
            {
                return;
            }
            AssetBundles.Add(assetBundle);
        }

        #endregion

        #region State info

        public bool IsInstalled()
        {
            return OverhaulAdditionalContentController.IsInstalled(this);
        }
        public bool IsEnabled()
        {
            return IsCompatibleWithMod() && OverhaulAdditionalContentController.UserData != null && OverhaulAdditionalContentController.UserData.IsContentPackEnabled(this);
        }
        public bool IsLoaded()
        {
            return !OverhaulAdditionalContentController.LoadedContent.IsNullOrEmpty() && OverhaulAdditionalContentController.LoadedContent.Contains(PackID);
        }
        public bool IsInstalledAndEnabled()
        {
            return IsInstalled() && IsEnabled();
        }
        public bool IsCompatibleWithMod()
        {
            return OverhaulVersion.ModVersion >= MinModVersionRequired;
        }
        public bool CanBeEdited()
        {
            return !IsLoaded() && !IsEnabled();
        }

        #endregion

        #region Loading & Unloading

        [NonSerialized]
        public float LoadingProgress;
        [NonSerialized]
        public string LoadingAssetBundle;
        public void Load()
        {
            if (IsLoaded() || LoadingProgress != 0f)
            {
                return;
            }
            StaticCoroutineRunner.StartStaticCoroutine(loadCoroutine());
        }
        private IEnumerator loadCoroutine()
        {
            int assetBundlesLoaded = 0;
            float multiplier = 1f / AssetBundles.Count;
            LoadingProgress = 0f;

            foreach (string assetBundleFile in AssetBundles)
            {
                LoadingAssetBundle = assetBundleFile;
                AssetsController.LoadAssetBundleAsync(GetAssetBundlePath(assetBundleFile), null);
                while (AssetsController.LoadingAssetBundle)
                {
                    LoadingProgress = (assetBundlesLoaded * multiplier) + (AssetsController.LoadingProgress * multiplier);
                    yield return null;
                }
                assetBundlesLoaded++;
            }
            LoadingProgress = 1f;
            if (OverhaulAdditionalContentController.LoadedContent != null) OverhaulAdditionalContentController.LoadedContent.Add(PackID);
            yield break;
        }

        public void Unload()
        {
            LoadingProgress = 0f;
            LoadingAssetBundle = string.Empty;
            foreach(string assetBundleFile in AssetBundles)
            {
                AssetsController.TryUnloadAssetBundle(GetAssetBundlePath(assetBundleFile), true);
            }
            OverhaulAdditionalContentController.LoadedContent.Remove(PackID);
        }

        #endregion

        #region Creating & saving

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

        #endregion

        #region IO

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

        public string GetAssetBundlePath(string assetBundle)
        {
            return OverhaulMod.Core.ModDirectory + OverhaulAdditionalContentController.AdditionalContentDirectoryName + "/" + GetPackFolderName() + "/" + assetBundle;
        }

        #endregion

        public enum ContentPackType
        {
            PreInstalled,
            Network
        }
    }
}
