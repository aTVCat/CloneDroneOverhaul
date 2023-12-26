using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Utils;
using System;
using System.IO;
using UnityEngine.Networking;

namespace OverhaulMod
{
    public class ContentManager : Singleton<ContentManager>
    {
        public void DownloadDefaultContent(out UnityWebRequest unityWebRequest, Action callback, Action<string> errorCallback)
        {
            if (HasFolder("default"))
            {
                Directory.Delete(ModCore.contentFolder + "default/", true);
            }

            ContentRepositoryManager.Instance.GetFileContent("content/default.zip", delegate (byte[] bytes)
            {
                string tempFile = Path.GetTempFileName();
                ModIOUtils.WriteBytes(bytes, tempFile);

                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(tempFile, ModCore.contentFolder, null);
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
                ModManagers.Instance.DispatchModContentLoadedEvent();
                callback?.Invoke();
            }, errorCallback, out unityWebRequest, 200);
        }

        public string[] GetInstalledContentPaths()
        {
            return Directory.GetDirectories(ModCore.contentFolder);
        }

        public string GetContentPath(string contentName)
        {
            string path = ModCore.contentFolder + contentName + "/";
            return !Directory.Exists(path) ? null : path;
        }

        public bool HasFolder(string contentName)
        {
            return Directory.Exists(ModCore.contentFolder + contentName + "/");
        }

        public bool IsFullInstallation()
        {
            return HasFolder("default");
        }
    }
}
