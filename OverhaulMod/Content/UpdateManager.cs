using ICSharpCode.SharpZipLib.Zip;
using InternalModBot;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.Content
{
    public class UpdateManager : Singleton<UpdateManager>
    {
        public const string REPOSITORY_FILE = "UpdateInfo.json";
        public const string PLAYER_PREF_KEY = "Overhaul.PreviousVersionPath";

        public static readonly List<Dropdown.OptionData> BranchesForTestersDropdownOptions = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData() { text = "Release" },
            new Dropdown.OptionData() { text = "Testing" },
            new Dropdown.OptionData() { text = "Canary" },
        };

        public static readonly List<Dropdown.OptionData> BranchesDropdownOptions = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData() { text = "Release" },
            new Dropdown.OptionData() { text = "Testing" },
        };

        public static float timeToToClearCache
        {
            get;
            set;
        }

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
                m_downloadedUpdateInfoList = null;

            if (m_downloadedUpdateInfoList != null)
            {
                DelegateScheduler.Instance.Schedule(delegate
                {
                    callback?.Invoke(m_downloadedUpdateInfoList);
                }, 1f);
                return;
            }

            ContentRepositoryManager.Instance.GetTextFile(REPOSITORY_FILE, delegate (string content)
            {
                UpdateInfoList updateInfoList = null;
                try
                {
                    updateInfoList = ModJsonUtils.Deserialize<UpdateInfoList>(content);
                    m_downloadedUpdateInfoList = updateInfoList;
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

            ContentRepositoryManager.Instance.GetCustomFile(source, delegate (byte[] bytes)
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

        public string GetBranchDescription(int index)
        {
            switch (index)
            {
                case 0:
                    return "Recommended. Get stable builds.";
                case 1:
                    return "Get latest test builds.";
                case 2:
                    return "Only available to testers. Some features may not work.";
            }
            return "This branch doesn't have description.";
        }

        public List<Dropdown.OptionData> GetAvailableBranches()
        {
            if (ExclusiveContentManager.Instance.IsLocalUserTheTester())
            {
                return BranchesForTestersDropdownOptions;
            }
            return BranchesDropdownOptions;
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
