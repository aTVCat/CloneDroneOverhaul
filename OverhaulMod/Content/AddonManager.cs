using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class AddonManager : Singleton<AddonManager>
    {
        public const string ADDONS_LIST_REPOSITORY_FILE = "AddonDownloads.json";

        public const string ADDON_INFO_FILE = "AddonInfo.json";

        public const string ADDON_INFO_FILE_OLD = "contentInfo.json";

        public const string ADDON_DOWNLOADED_EVENT = "OverhaulAddonDownloaded";

        public const string EXTRAS_ADDON_ID = "d1ca04f0";

        public const string REALISTIC_SKYBOXES_ADDON_ID = "51a9fc49";

        public const string GALLERY_ADDON_ID = "b63ebd97";

        public const string ADDON_UPDATES_REFRESHED = "AddonUpdatesRefreshed";

        [ModSetting(ModSettingsConstants.ADDONS_TO_UPDATE, "", ModSetting.Tag.IgnoreExport)]
        public static string AddonsToUpdate;

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

        private void Start()
        {
            RefreshAddonUpdates();
        }

        public void RefreshAddonUpdates()
        {
            if (!ScheduledActionsManager.Instance.ShouldExecuteAction(ScheduledActionType.RefreshAddonUpdates))
                return;

            GetDownloads(delegate (GetDownloadsResult getDownloadsResult)
            {
                if (!getDownloadsResult.IsError())
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (AddonDownloadInfo addonInfo in getDownloadsResult.Downloads.Addons)
                    {
                        if (HasInstalledAddon(addonInfo.UniqueID) && !HasInstalledAddon(addonInfo.UniqueID, addonInfo.Addon.Version))
                        {
                            stringBuilder.Append(addonInfo.UniqueID);
                            stringBuilder.Append(',');
                        }
                    }
                    ModSettingsManager.SetStringValue(ModSettingsConstants.ADDONS_TO_UPDATE, stringBuilder.ToString(), false);
                    ScheduledActionsManager.Instance.SetActionExecuted(ScheduledActionType.RefreshAddonUpdates);
                    GlobalEventManager.Instance.Dispatch(ADDON_UPDATES_REFRESHED);
                }
            });
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
                        if (!download.Addon.IsSupported())
                        {
                            callback?.Invoke($"This addon requires new Overhaul mod version: {download.Addon.MinModVersion}");
                            return;
                        }

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
            string displayName = addonDownloadInfo.GetDisplayName();

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
                    ModManagers.Instance.TriggerModContentLoadedEvent($"\"{displayName}\" download error:\n{exc}");
                    return;
                }

                if (DoesAddonNeedUpdate(uniqueId))
                {
                    ModSettingsManager.SetStringValue(ModSettingsConstants.ADDONS_TO_UPDATE, AddonsToUpdate.Replace($"{uniqueId},", string.Empty), false);
                }

                RefreshInstalledAddons();

                ModManagers.Instance.TriggerModContentLoadedEvent(null);
                callback?.Invoke(null);
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
            }, errorCallback, out unityWebRequest, 20);
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
        public bool HasInstalledAddon(string addonId, int minVersion)
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

        /// <summary>
        /// Get the version of installed addon with specified id
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <returns>-1 if addon is not installed, otherwise the installed addon version</returns>
        public int GetAddonVersion(string uniqueId)
        {
            var info = GetAddonInfo(uniqueId);
            if (info == null)
                return -1;

            return info.Version;
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
                {
                    addonInfoFilePath = Path.Combine(folder, ADDON_INFO_FILE_OLD);
                    if (!File.Exists(addonInfoFilePath))
                        continue;

                    try
                    {
                        string newId = null;

                        ContentInfo contentInfo = ModJsonUtils.DeserializeStream<ContentInfo>(addonInfoFilePath);
                        if (contentInfo.DisplayName == "Extras")
                        {
                            newId = EXTRAS_ADDON_ID;
                        }
                        else if (contentInfo.DisplayName == "Behind the scenes")
                        {
                            newId = GALLERY_ADDON_ID;
                        }
                        else if (contentInfo.DisplayName == "Realistic skyboxes")
                        {
                            newId = REALISTIC_SKYBOXES_ADDON_ID;
                        }

                        if (newId.IsNullOrEmpty())
                            continue;

                        AddonInfo addonInfo = new AddonInfo();
                        addonInfo.DisplayName = new Dictionary<string, string>();
                        addonInfo.DisplayName.Add("en", contentInfo.DisplayName);
                        addonInfo.Description = new Dictionary<string, string>();
                        addonInfo.Version = -1;
                        addonInfo.UniqueID = newId;
                        addonInfo.FolderPath = folder;
                        addonInfo.MinModVersion = ModBuildInfo.version;
                        list.Add(addonInfo);
                    }
                    catch
                    {
                        continue;
                    }
                }

                try
                {
                    AddonInfo contentInfo = ModJsonUtils.DeserializeStream<AddonInfo>(addonInfoFilePath);
                    if (!contentInfo.IsSupported())
                        continue;

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

        public bool DoesAddonNeedUpdate(string addonId)
        {
            return AddonsToUpdate.Contains(addonId);
        }

        public bool DoesAddonNeedUpdate(AddonInfo addonInfo)
        {
            return DoesAddonNeedUpdate(addonInfo.UniqueID);
        }

        public class GetDownloadsResult : DownloadResult
        {
            public AddonDownloadListInfo Downloads;

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
        }
    }
}
