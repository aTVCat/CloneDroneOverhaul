using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Utils;
using System;
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

        public void DownloadContent(string name, out UnityWebRequest unityWebRequest, Action callback, Action<string> errorCallback)
        {
            RepositoryManager.Instance.GetFile($"content/{name}.zip", delegate (byte[] bytes)
            {
                RemoveContent(name);

                string tempFile = Path.GetTempFileName();
                ModIOUtils.WriteBytes(bytes, tempFile);

                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(tempFile, ModCore.contentFolder, null);
                if (File.Exists(tempFile))
                    File.Delete(tempFile);

                ModManagers.Instance.TriggerModContentLoadedEvent();
                callback?.Invoke();
            }, errorCallback, out unityWebRequest, 200);
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
            }, errorCallback, out unityWebRequest, 200);
        }

        public void RemoveContent(string name)
        {
            if (HasContent(name))
                Directory.Delete($"{ModCore.contentFolder}{name}/", true);
        }

        public bool HasContent(string contentName)
        {
            return Directory.Exists($"{ModCore.contentFolder}{contentName}/");
        }

        /// <summary>
        /// Get content directory path
        /// </summary>
        /// <param name="contentName"></param>
        /// <returns></returns>
        public string GetContentPath(string contentName)
        {
            string path = $"{ModCore.contentFolder}{contentName}/";
            return !Directory.Exists(path) ? null : path;
        }

        public List<ContentInfo> GetContent()
        {
            string[] folders = Directory.GetDirectories(ModCore.contentFolder);
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
            return list;
        }
    }
}
