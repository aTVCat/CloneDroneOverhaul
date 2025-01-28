using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class AddonManager : Singleton<AddonManager>
    {
        public const string ADDONS_LIST_REPOSITORY_FILE = "AddonDownloads.json";

        public const string ADDON_INFO_FILE = "AddonInfo.json";

        public const string ADDON_DOWNLOADED_EVENT = "OverhaulAddonDownloaded";

        public const string EXTRAS_ADDON_ID = "d1ca04f0";

        public const string REALISTIC_SKYBOXES_ADDON_ID = "51a9fc49";

        public const string GALLERY_ADDON_ID = "084c7e7a";

        private Dictionary<string, float> m_downloadingAddons;

        private List<object> m_loadingAddons;

        private List<AddonInfo> m_loadedAddons;

        private AddonDownloadListInfo m_addonDownloadListInfo;

        public override void Awake()
        {
            base.Awake();
            m_downloadingAddons = new Dictionary<string, float>();
            m_loadingAddons = new List<object>();
            RefreshInstalledAddons();
        }

        public AddonDownloadListInfo GetDownloadsFromDisk()
        {
            if (m_addonDownloadListInfo != null)
                return m_addonDownloadListInfo;

            string path = Path.Combine(ModCore.developerFolder, ADDONS_LIST_REPOSITORY_FILE);
            if (!File.Exists(path))
            {
                m_addonDownloadListInfo = new AddonDownloadListInfo();
            }
            else
            {
                m_addonDownloadListInfo = ModJsonUtils.DeserializeStream<AddonDownloadListInfo>(path);
            }
            m_addonDownloadListInfo.FixValues();
            return m_addonDownloadListInfo;
        }

        public void SaveDownloadsToDisk()
        {
            if (m_addonDownloadListInfo == null)
                return;

            string path = Path.Combine(ModCore.developerFolder, ADDONS_LIST_REPOSITORY_FILE);
            ModJsonUtils.WriteStream(path, m_addonDownloadListInfo);
        }

        public void GetDownloads(Action<GetDownloadsResult> callback)
        {
            if (m_addonDownloadListInfo != null)
            {
                callback?.Invoke(new GetDownloadsResult(m_addonDownloadListInfo));
                return;
            }

            RepositoryManager.Instance.GetTextFile(ADDONS_LIST_REPOSITORY_FILE, delegate (string content)
            {
                AddonDownloadListInfo addonDownloadListInfo;
                try
                {
                    addonDownloadListInfo = ModJsonUtils.Deserialize<AddonDownloadListInfo>(content);
                    addonDownloadListInfo.FixValues();
                    m_addonDownloadListInfo = addonDownloadListInfo;
                }
                catch (Exception ex)
                {
                    callback?.Invoke(new GetDownloadsResult(ex.ToString()));
                    return;
                }

                callback?.Invoke(new GetDownloadsResult(addonDownloadListInfo));
            }, delegate (string error)
            {
                callback?.Invoke(new GetDownloadsResult(error));
            }, out _, 20);
        }

        public void DownloadAddon(string addonId, Action<string> callback)
        {
            if (m_downloadingAddons.ContainsKey(addonId))
            {
                _ = base.StartCoroutine(waitUntilAddonIsDownloaded(callback));
                return;
            }

            m_downloadingAddons.Add(addonId, 0f);
            GetDownloads(delegate (GetDownloadsResult getDownloadsResult)
            {
                m_downloadingAddons.Remove(addonId);
                if (getDownloadsResult.IsError())
                {
                    callback?.Invoke(getDownloadsResult.Error);
                    return;
                }

                AddonDownloadListInfo downloads = getDownloadsResult.Downloads;
                foreach (AddonDownloadInfo download in downloads.Addons)
                {
                    if (download.UniqueID == addonId)
                    {
                        DownloadAddon(download, callback);
                        return;
                    }
                }

                callback?.Invoke("Could not find requested addon");
            });
        }

        public void DownloadAddon(AddonDownloadInfo addonDownloadInfo, Action<string> callback)
        {
            string folderName = addonDownloadInfo.Addon?.GetDisplayName("en", true);
            if (folderName.IsNullOrEmpty())
                folderName = $"addon_{addonDownloadInfo.UniqueID}";
            else
                folderName = folderName.Replace(" ", string.Empty);

            string uniqueId = addonDownloadInfo.UniqueID;
            string downloadUrl = addonDownloadInfo.PackageFileURL;

            if (m_downloadingAddons.ContainsKey(uniqueId))
            {
                _ = base.StartCoroutine(waitUntilAddonIsDownloaded(callback));
                return;
            }
            m_downloadingAddons.Add(uniqueId, 0f);

            string tempPath = Path.GetTempFileName();
            GoogleDriveManager.Instance.DownloadFile(downloadUrl, tempPath, delegate (float progress)
            {
                if (!m_downloadingAddons.ContainsKey(uniqueId))
                    m_downloadingAddons.Add(uniqueId, progress);
                else
                    m_downloadingAddons[uniqueId] = progress;
            },
            delegate (string result)
            {
                _ = m_downloadingAddons.Remove(uniqueId);

                if (result != null)
                {
                    ModManagers.Instance.TriggerModContentLoadedEvent(result);
                    callback?.Invoke(result);
                    return;
                }

                DeleteAddon(uniqueId);

                try
                {
                    string dest = Path.Combine(ModCore.addonsFolder, folderName);
                    if (!Directory.Exists(dest))
                        _ = Directory.CreateDirectory(dest);

                    FastZip fastZip = new FastZip();
                    fastZip.ExtractZip(tempPath, dest, null);
                }
                catch (Exception exc)
                {
                    ModManagers.Instance.TriggerModContentLoadedEvent(exc.ToString());
                    return;
                }

                ModManagers.Instance.TriggerModContentLoadedEvent(null);
                callback?.Invoke(null);

                RefreshInstalledAddons();
            });
        }

        public void DownloadAddonsList(out UnityWebRequest unityWebRequest, Action<AddonDownloadListInfo> callback, Action<string> errorCallback)
        {
            RepositoryManager.Instance.GetTextFile(ADDONS_LIST_REPOSITORY_FILE, delegate (string rawData)
            {
                AddonDownloadListInfo addonsList = null;
                try
                {
                    addonsList = ModJsonUtils.Deserialize<AddonDownloadListInfo>(rawData);
                    addonsList.FixValues();
                }
                catch (Exception exc)
                {
                    errorCallback?.Invoke(exc.ToString());
                    return;
                }

                callback?.Invoke(addonsList);
            }, errorCallback, out unityWebRequest, 15);
        }

        private IEnumerator waitUntilAddonIsDownloaded(Action<string> callback)
        {
            GlobalEventManager.Instance.AddEventListenerOnce(ADDON_DOWNLOADED_EVENT, delegate (string error)
            {
                callback?.Invoke(error);
            });
            yield break;
        }

        public bool IsDownloadingAddon(string uniqueId)
        {
            return m_downloadingAddons.ContainsKey(uniqueId);
        }

        public float GetAddonDownloadProgress(string uniqueId)
        {
            if (m_downloadingAddons.TryGetValue(uniqueId, out float progress))
                return progress;

            return -1f;
        }

        public void SetAddonIsLoading(object obj, bool value)
        {
            if (obj == null)
                return;

            if (value && !m_loadingAddons.Contains(obj))
                m_loadingAddons.Add(obj);
            else if (!value)
                _ = m_loadingAddons.Remove(obj);
        }

        public bool IsLoadingAddons()
        {
            return m_loadingAddons != null && m_loadingAddons.Count != 0;
        }

        /// <summary>
        /// Check if specified addon is installed
        /// </summary>
        /// <param name="addonId"></param>
        /// <param name="quick">If true, it check if addon is cached</param>
        /// <returns></returns>
        public bool HasInstalledAddon(string addonId)
        {
            return HasInstalledAddon(addonId, out _);
        }

        /// <summary>
        /// Check if specified addon is installed
        /// </summary>
        /// <param name="addonId"></param>
        /// <param name="quick">If true, it check if addon is cached</param>
        /// <returns></returns>
        public bool HasInstalledAddon(string addonId, out string path)
        {
            path = null;
            if (m_loadedAddons.IsNullOrEmpty())
                return false;

            foreach (AddonInfo c in m_loadedAddons)
                if (c.UniqueID == addonId)
                {
                    path = c.FolderPath;
                    return true;
                }

            return false;
        }

        /// <summary>
        /// Check if specified addon version is installed
        /// </summary>
        /// <param name="addonId"></param>
        /// <param name="quick">If true, it check if addon is cached</param>
        /// <returns></returns>
        public bool HasInstalledAddonVersion(string addonId, int minVersion)
        {
            if (m_loadedAddons.IsNullOrEmpty())
                return false;

            foreach (AddonInfo c in m_loadedAddons)
                if (c.UniqueID == addonId && c.Version >= minVersion)
                    return true;

            return false;
        }

        public AddonInfo GetAddonInfo(string uniqueId)
        {
            foreach (AddonInfo addon in m_loadedAddons)
                if (addon.UniqueID == uniqueId)
                    return addon;

            return null;
        }

        public string GetAddonPath(string uniqueId)
        {
            HasInstalledAddon(uniqueId, out string result);
            return result;
        }

        /// <summary>
        /// Deletes addon from disk
        /// </summary>
        /// <param name="uniqueId"></param>
        public void DeleteAddon(string uniqueId)
        {
            AddonInfo addonInfo = GetAddonInfo(uniqueId);
            if (addonInfo != null)
            {
                m_loadedAddons.Remove(addonInfo);
                Directory.Delete(addonInfo.FolderPath, true);
            }
        }

        public void RefreshInstalledAddons()
        {
            _ = GetLoadedAddons(false);
        }

        /// <summary>
        /// Returns installed addons
        /// </summary>
        /// <param name="returnCached">Will return cached value if possible</param>
        /// <returns></returns>
        public List<AddonInfo> GetLoadedAddons(bool returnCached = true)
        {
            if (returnCached && m_loadedAddons != null)
                return m_loadedAddons;

            List<AddonInfo> list = m_loadedAddons;
            if (list == null)
            {
                list = new List<AddonInfo>();
                m_loadedAddons = list;
            }
            else
            {
                list.Clear();
            }

            string[] folders = Directory.GetDirectories(ModCore.addonsFolder);
            if (folders.IsNullOrEmpty())
                return list;

            foreach (string folder in folders)
            {
                string addonInfoFilePath = Path.Combine(folder, ADDON_INFO_FILE);
                if (!File.Exists(addonInfoFilePath))
                    continue;

                try
                {
                    AddonInfo contentInfo = ModJsonUtils.DeserializeStream<AddonInfo>(addonInfoFilePath);
                    contentInfo.FolderPath = folder;
                    list.Add(contentInfo);
                }
                catch
                {
                    continue;
                }
            }

            return list;
        }

        public void AddLoadedAddon(AddonInfo addonInfo)
        {
            m_loadedAddons.Add(addonInfo);
        }

        public class GetDownloadsResult
        {
            public AddonDownloadListInfo Downloads;

            public string Error;

            public GetDownloadsResult()
            {

            }

            public GetDownloadsResult(string error)
            {
                Error = error;
            }

            public GetDownloadsResult(AddonDownloadListInfo downloads)
            {
                Downloads = downloads;
            }

            public bool IsError()
            {
                return !Error.IsNullOrEmpty();
            }
        }
    }
}
