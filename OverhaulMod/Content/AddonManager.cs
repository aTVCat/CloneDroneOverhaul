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
        public const string ADDONS_LIST_REPOSITORY_FILE = "AddonsList.json";

        public const string ADDON_INFO_FILE = "AddonInfo.json";

        public const string EXTRAS_ADDON_FOLDER_NAME = "Extras";

        public const string REALISTIC_SKYBOXES_ADDON_FOLDER_NAME = "Realistic skyboxes";

        public const string GALLERY_ADDON_FOLDER_NAME = "Behind the scenes";

        public const string ADDON_DOWNLOADED_EVENT = "OverhaulAddonDownloaded";

        private Dictionary<string, float> m_downloadingFiles;

        private List<object> m_loadingAddons;

        private List<AddonInfo> m_installedAddons;

        private bool m_hasInitialized;

        private void Start()
        {
            m_downloadingFiles = new Dictionary<string, float>();
            m_loadingAddons = new List<object>();
            RefreshInstalledAddons();
            m_hasInitialized = true;
        }

        public bool IsInitialized()
        {
            return m_hasInitialized;
        }

        public bool DownloadAddon(string addonName, string downloadUrl, Action<string> callback)
        {
            if (m_downloadingFiles.ContainsKey(downloadUrl))
            {
                _ = base.StartCoroutine(waitUntilAddonIsDownloaded(downloadUrl, callback));
                return false;
            }

            m_downloadingFiles.Add(downloadUrl, 0f);

            string tempPath = Path.GetTempFileName();
            GoogleDriveManager.Instance.DownloadFile(downloadUrl, tempPath, delegate (float progress)
            {
                if (!m_downloadingFiles.ContainsKey(downloadUrl))
                    m_downloadingFiles.Add(downloadUrl, progress);
                else
                    m_downloadingFiles[downloadUrl] = progress;
            },
            delegate (string result)
            {
                _ = m_downloadingFiles.Remove(downloadUrl);

                if (result != null)
                {
                    ModManagers.Instance.TriggerModContentLoadedEvent(result);
                    callback?.Invoke(result);
                    return;
                }

                DeleteAddon(addonName);

                try
                {
                    string dest = Path.Combine(ModCore.addonsFolder, addonName);
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

                RefreshInstalledAddons(true);
            });
            return true;
        }

        public void DownloadAddonsList(out UnityWebRequest unityWebRequest, Action<AddonListInfo> callback, Action<string> errorCallback)
        {
            RepositoryManager.Instance.GetTextFile(ADDONS_LIST_REPOSITORY_FILE, delegate (string rawData)
            {
                AddonListInfo addonsList = null;
                try
                {
                    addonsList = ModJsonUtils.Deserialize<AddonListInfo>(rawData);
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

        private IEnumerator waitUntilAddonIsDownloaded(string name, Action<string> callback)
        {
            GlobalEventManager.Instance.AddEventListenerOnce(ADDON_DOWNLOADED_EVENT, delegate (string error)
            {
                callback?.Invoke(error);
            });
            yield break;
        }

        public bool IsDownloadingAddon(string name)
        {
            return m_downloadingFiles.ContainsKey(name);
        }

        public float GetDownloadProgressOfAddon(string name)
        {
            if (m_downloadingFiles.TryGetValue(name, out float progress))
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
        /// Deletes addon from disk
        /// </summary>
        /// <param name="name"></param>
        public void DeleteAddon(string name)
        {
            if (HasInstalledAddon(name))
                Directory.Delete(Path.Combine(ModCore.addonsFolder, name), true);
        }

        /// <summary>
        /// Check if specified addon is installed
        /// </summary>
        /// <param name="addonName"></param>
        /// <param name="quick">If true, it check if addon is cached</param>
        /// <returns></returns>
        public bool HasInstalledAddon(string addonName, bool quick = false)
        {
            if (quick)
            {
                string lower = addonName.ToLower();
                if (m_installedAddons.IsNullOrEmpty())
                    return false;

                foreach (AddonInfo c in m_installedAddons)
                    if (c.DisplayName.ToLower() == lower)
                        return true;
            }
            return File.Exists(Path.Combine(GetAddonPath(addonName), ADDON_INFO_FILE));
        }

        /// <summary>
        /// Check if specified addon version is installed
        /// </summary>
        /// <param name="addonName"></param>
        /// <param name="quick">If true, it check if addon is cached</param>
        /// <returns></returns>
        public bool HasInstalledAddonVersion(string addonName, int minVersion, bool quick = false)
        {
            // todo: implement addon version system
            return HasInstalledAddon(addonName, quick);
        }

        /// <summary>
        /// Get content directory path
        /// </summary>
        /// <param name="addonName"></param>
        /// <returns></returns>
        public string GetAddonPath(string addonName)
        {
            return Path.Combine(ModCore.addonsFolder, addonName);
        }

        /// <summary>
        /// Returns installed addons
        /// </summary>
        /// <param name="returnCached">Will return cached value if possible</param>
        /// <returns></returns>
        public List<AddonInfo> GetInstalledAddons(bool returnCached = true)
        {
            if (returnCached && m_installedAddons != null)
                return m_installedAddons;

            List<AddonInfo> list = m_installedAddons;
            if (list == null)
            {
                list = new List<AddonInfo>();
                m_installedAddons = list;
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

        public void RefreshInstalledAddons(bool force = false)
        {
            _ = GetInstalledAddons(force);
        }
    }
}
