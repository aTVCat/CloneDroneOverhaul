using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Utils;
using System;
using System.IO;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class ContentManager : Singleton<ContentManager>
    {
        public void DownloadContent(string name, out UnityWebRequest unityWebRequest, Action callback, Action<string> errorCallback)
        {
            ContentRepositoryManager.Instance.GetFile($"content/{name}.zip", delegate (byte[] bytes)
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
        /// Get installed content directories
        /// </summary>
        /// <returns></returns>
        public string[] GetLocalContent()
        {
            return Directory.GetDirectories(ModCore.contentFolder);
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
    }
}
