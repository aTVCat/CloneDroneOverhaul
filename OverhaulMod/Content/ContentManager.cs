using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class ContentManager : Singleton<ContentManager>
    {
        public const string CONTENT_LIST_REPOSITORY_FILE = "ContentList.json";

        public const string CONTENT_INFO_FILE = "contentInfo.json";

        public const string EXTRAS_CONTENT_FOLDER_NAME = "Extras";

        public const string REALISTIC_SKYBOXES_CONTENT_FOLDER_NAME = "RealisticSkyboxes";

        public const string GALLERY_CONTENT_FOLDER_NAME = "Gallery";

        public const string CONTENT_DOWNLOAD_DONE_EVENT = "OverhaulContentDownloadDone";

        private Dictionary<string, UnityWebRequest> m_downloadingFiles;

        private List<ContentInfo> m_installedContent;

        private List<object> m_loadingAddons;

        public bool isLoadingAnyContent
        {
            get
            {
                return m_loadingAddons != null && m_loadingAddons.Count != 0;
            }
        }

        private void Start()
        {
            m_downloadingFiles = new Dictionary<string, UnityWebRequest>();
            m_loadingAddons = new List<object>();
            RefreshContent();
        }

        public void RefreshContent(bool force = false)
        {
            if (force)
                m_installedContent = null;

            _ = GetContent();
        }

        public bool DownloadContent(string name, Action callback, Action<string> errorCallback)
        {
            if (m_downloadingFiles.ContainsKey(name))
            {
                _ = base.StartCoroutine(waitUntilContentIsDownloaded(name, callback, errorCallback));
                return false;
            }

            RepositoryManager.Instance.GetCustomFile($"https://github.com/aTVCat/Overhaul-Mod-Content/raw/main/content/{name}.zip", delegate (byte[] bytes)
            {
                RemoveContent(name);
                _ = m_downloadingFiles.Remove(name);

                try
                {
                    string tempFile = Path.GetTempFileName();
                    ModIOUtils.WriteBytes(bytes, tempFile);

                    string directory = ModCore.addonsFolder + name.Replace(" ", string.Empty) + "/";
                    if (!Directory.Exists(directory))
                        _ = Directory.CreateDirectory(directory);

                    FastZip fastZip = new FastZip();
                    fastZip.ExtractZip(tempFile, directory, null);
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch (Exception exc)
                {
                    ModManagers.Instance.TriggerModContentLoadedEvent(exc.ToString());
                    return;
                }

                ModManagers.Instance.TriggerModContentLoadedEvent(null);
                callback?.Invoke();

                RefreshContent(true);
            }, delegate (string error)
            {
                _ = m_downloadingFiles.Remove(name);
                ModManagers.Instance.TriggerModContentLoadedEvent(error);
            }, out UnityWebRequest unityWebRequest, -1);

            m_downloadingFiles.Add(name, unityWebRequest);

            return true;
        }

        public void DownloadContentList(out UnityWebRequest unityWebRequest, Action<ContentListInfo> callback, Action<string> errorCallback)
        {
            RepositoryManager.Instance.GetTextFile(CONTENT_LIST_REPOSITORY_FILE, delegate (string rawData)
            {
                ContentListInfo contentInfo = null;
                try
                {
                    contentInfo = ModJsonUtils.Deserialize<ContentListInfo>(rawData);
                    contentInfo.FixValues();
                }
                catch (Exception exc)
                {
                    errorCallback?.Invoke(exc.ToString());
                    return;
                }

                callback?.Invoke(contentInfo);
            }, errorCallback, out unityWebRequest, 15);
        }

        private IEnumerator waitUntilContentIsDownloaded(string name, Action callback, Action<string> errorCallback)
        {
            GlobalEventManager.Instance.AddEventListenerOnce(CONTENT_DOWNLOAD_DONE_EVENT, delegate (string error)
            {
                if (string.IsNullOrEmpty(error))
                {
                    callback?.Invoke();
                }
                else
                {
                    errorCallback.Invoke(error);
                }
            });
            yield break;
        }

        public bool IsDownloading(string name)
        {
            return m_downloadingFiles.ContainsKey(name);
        }

        public float GetDownloadProgress(string name)
        {
            if (m_downloadingFiles.TryGetValue(name, out UnityWebRequest unityWebRequest))
            {
                try
                {
                    return unityWebRequest.isDone ? 1f : unityWebRequest.downloadProgress;
                }
                catch
                {
                    return -1f;
                }
            }
            return -1f;
        }

        public void RemoveContent(string name)
        {
            if (HasContent(name))
                Directory.Delete($"{ModCore.addonsFolder}{name}/", true);
        }

        public bool HasContent(string contentName, bool quick = false)
        {
            if (quick)
            {
                if (m_installedContent.IsNullOrEmpty())
                    return false;

                foreach (ContentInfo c in m_installedContent)
                    if (c.DisplayName == contentName)
                        return true;
            }
            return Directory.Exists($"{ModCore.addonsFolder}{contentName}/");
        }

        public void SetContentIsLoading(object obj, bool value)
        {
            if (obj == null)
                return;

            if (value && !m_loadingAddons.Contains(obj))
                m_loadingAddons.Add(obj);
            else if (!value)
                _ = m_loadingAddons.Remove(obj);
        }

        /// <summary>
        /// Get content directory path
        /// </summary>
        /// <param name="contentName"></param>
        /// <returns></returns>
        public string GetContentPath(string contentName)
        {
            string path = $"{ModCore.addonsFolder}{contentName}/";
            return !Directory.Exists(path) ? null : path;
        }

        public List<ContentInfo> GetContent()
        {
            if (m_installedContent != null)
                return m_installedContent;

            string[] folders = Directory.GetDirectories(ModCore.addonsFolder);
            if (folders.IsNullOrEmpty())
                return null;

            List<ContentInfo> list = new List<ContentInfo>();
            foreach (string folder in folders)
            {
                string addonInfoFilePath = folder + "/" + CONTENT_INFO_FILE;
                if (!File.Exists(addonInfoFilePath))
                    continue;

                try
                {
                    ContentInfo contentInfo = ModJsonUtils.DeserializeStream<ContentInfo>(addonInfoFilePath);
                    contentInfo.FolderPath = folder + "/";
                    list.Add(contentInfo);
                }
                catch
                {
                    continue;
                }
            }
            m_installedContent = list;
            return list;
        }
    }
}
