using ICSharpCode.SharpZipLib.Zip;
using InternalModBot;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.Content
{
    public class UpdateManager : Singleton<UpdateManager>
    {
        public const string DATA_REFRESH_TIME_PLAYER_PREF_KEY = "UpdateInfoRefreshDate";

        public const string NEW_VERSION_PLAYER_PREF_KEY = "UpdateInfoVersion";

        public const string REPOSITORY_FILE = "UpdateInfo.json";

        public const string PLAYER_PREF_KEY = "Overhaul.PreviousVersionPath";

        [ModSetting(ModSettingConstants.CHECK_FOR_UPDATES_ON_STARTUP, true)]
        public static bool CheckForUpdatesOnStartup;

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

        public static System.Version downloadedVersion
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
                PlayerPrefs.SetString(NEW_VERSION_PLAYER_PREF_KEY, ModBuildInfo.version.ToString());
            }
            _ = ModActionUtils.RunCoroutine(retrieveDataOnStartCoroutine());
        }

        private IEnumerator retrieveDataOnStartCoroutine()
        {
            yield return null;

            if (!CheckForUpdatesOnStartup)
            {
                downloadedVersion = ModBuildInfo.version;
                yield break;
            }

            if (!System.Version.TryParse(PlayerPrefs.GetString(NEW_VERSION_PLAYER_PREF_KEY, "default"), out System.Version newVersion))
                newVersion = ModBuildInfo.version;
            downloadedVersion = newVersion;

            if (DateTime.TryParse(PlayerPrefs.GetString(DATA_REFRESH_TIME_PLAYER_PREF_KEY, "default"), out DateTime timeToRefreshData))
                if (DateTime.Now < timeToRefreshData)
                    yield break;

            yield return new WaitUntil(() => MultiplayerLoginManager.Instance.IsLoggedIntoPlayfab());
            yield return new WaitForSecondsRealtime(2f);

            DownloadUpdateInfoFile(delegate
            {
                PlayerPrefs.SetString(DATA_REFRESH_TIME_PLAYER_PREF_KEY, DateTime.Now.AddDays(1).ToString());
            }, null, false);
            yield break;
        }

        public bool ShouldHighlightUpdatesButton()
        {
            return downloadedVersion > ModBuildInfo.version;
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

            RepositoryManager.Instance.GetTextFile(REPOSITORY_FILE, delegate (string content)
            {
                UpdateInfoList updateInfoList = null;
                try
                {
                    updateInfoList = ModJsonUtils.Deserialize<UpdateInfoList>(content);

                    if (ExclusiveContentManager.Instance.IsLocalUserTheTester())
                        if (updateInfoList.InternalRelease != null && updateInfoList.InternalRelease.ModVersion != null)
                            PlayerPrefs.SetString(NEW_VERSION_PLAYER_PREF_KEY, updateInfoList.InternalRelease.ModVersion.ToString());
                        else
                        if (updateInfoList.ModBotRelease != null && updateInfoList.ModBotRelease.ModVersion != null)
                            PlayerPrefs.SetString(NEW_VERSION_PLAYER_PREF_KEY, updateInfoList.ModBotRelease.ModVersion.ToString());
                }
                catch (Exception exc)
                {
                    errorCallback?.Invoke(exc.ToString());
                    return;
                }
                callback?.Invoke(updateInfoList);
                m_downloadedUpdateInfoList = updateInfoList;
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

            RepositoryManager.Instance.GetCustomFile(source, delegate (byte[] bytes)
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
            return ExclusiveContentManager.Instance.IsLocalUserTheTester() ? BranchesForTestersDropdownOptions : BranchesDropdownOptions;
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
