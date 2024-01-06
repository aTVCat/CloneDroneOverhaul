using ICSharpCode.SharpZipLib.Zip;
using InternalModBot;
using OverhaulMod.Utils;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class UpdateManager : Singleton<UpdateManager>
    {
        public const string REPOSITORY_FILE = "UpdateInfo.json";
        public const string PLAYER_PREF_KEY = "Overhaul.PreviousVersionPath";

        private UpdateInfoList m_downloadedUpdateInfoList;

        private void Start()
        {
            if (PlayerPrefs.HasKey(PLAYER_PREF_KEY))
            {
                string oldBuildPath = PlayerPrefs.GetString(PLAYER_PREF_KEY);
                if (Directory.Exists(oldBuildPath))
                {
                    Directory.Delete(oldBuildPath, true);
                }
                PlayerPrefs.DeleteKey(PLAYER_PREF_KEY);
            }
        }

        public void DownloadUpdateInfoFile(Action<UpdateInfoList> callback, Action<string> errorCallback, bool clearCache = false)
        {
            if (clearCache)
            {
                m_downloadedUpdateInfoList = null;
                UnityWebRequest.ClearCookieCache();
            }

            if (m_downloadedUpdateInfoList != null)
            {
                callback?.Invoke(m_downloadedUpdateInfoList);
                return;
            }

            ContentRepositoryManager.Instance.GetTextFileContent(REPOSITORY_FILE, delegate (string content)
            {
                UpdateInfoList updateInfoList = null;
                try
                {
                    updateInfoList = ModJsonUtils.Deserialize<UpdateInfoList>(content);
                }
                catch (Exception exc)
                {
                    errorCallback?.Invoke(exc.ToString());
                    return;
                }
                callback?.Invoke(updateInfoList);
            }, errorCallback, out _);
        }

        public void DownloadBuildFromSource(string source, string directoryName, Action callback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = null;
            string directoryPath = ModsManager.Instance.ModFolderPath + directoryName.Replace('.', '_');
            if (Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.Delete(directoryPath, true);
                }
                catch (Exception exc)
                {
                    errorCallback?.Invoke(exc.ToString());
                    return;
                }
            }

            ContentRepositoryManager.Instance.CustomGetFileContent(source, delegate (byte[] bytes)
            {
                string tempFile = Path.GetTempFileName();
                ModIOUtils.WriteBytes(bytes, tempFile);

                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(tempFile, directoryPath, null);
                if (File.Exists(tempFile))
                    File.Delete(tempFile);

                try
                {
                    prepareBuildForAnUpdate();
                }
                catch (Exception exc)
                {
                    errorCallback?.Invoke("Prepare build error: " + exc);
                    return;
                }

                callback?.Invoke();
            }, errorCallback, out unityWebRequest, -1);
        }

        private void prepareBuildForAnUpdate()
        {
            string modFolder = ModCore.instance.ModInfo.FolderPath;
            string filePath = modFolder + "ModInfo.json";
            File.Delete(filePath);

            PlayerPrefs.SetString(PLAYER_PREF_KEY, modFolder);
            PlayerPrefs.Save();
        }
    }
}
