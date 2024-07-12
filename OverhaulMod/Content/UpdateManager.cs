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

        public const string NEW_RELEASE_VERSION_PLAYER_PREF_KEY = "UpdateInfoReleaseVersion";

        public const string NEW_TESTING_VERSION_PLAYER_PREF_KEY = "UpdateInfoTestingVersion";

        public const string REPOSITORY_FILE = "UpdateInfo.json";

        public const string PLAYER_PREF_KEY = "Overhaul.PreviousVersionPath";

        [ModSetting(ModSettingsConstants.CHECK_FOR_UPDATES_ON_STARTUP, true)]
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
            /*
            if (PlayerPrefs.HasKey(PLAYER_PREF_KEY))
            {
                string oldBuildPath = PlayerPrefs.GetString(PLAYER_PREF_KEY);
                if (Directory.Exists(oldBuildPath))
                {
                    try
                    {
                        Directory.Delete(oldBuildPath, true);
                    }
                    catch (Exception exc)
                    {
                        ModUIUtils.MessagePopupOK("Old build cleanup error", exc.ToString(), 200f, true);
                    }
                }
                PlayerPrefs.DeleteKey(PLAYER_PREF_KEY);
                PlayerPrefs.SetString(NEW_VERSION_PLAYER_PREF_KEY, ModBuildInfo.version.ToString());
            }*/
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

            bool isTester = ModBuildInfo.isPrereleaseBuild || ExclusiveContentManager.Instance.IsLocalUserTheTester();
            if (!System.Version.TryParse(PlayerPrefs.GetString(isTester ? NEW_TESTING_VERSION_PLAYER_PREF_KEY : NEW_RELEASE_VERSION_PLAYER_PREF_KEY, "default"), out System.Version newVersion))
                newVersion = ModBuildInfo.version;
            downloadedVersion = newVersion;

            if (newVersion > ModBuildInfo.version && GameModeManager.IsOnTitleScreen())
            {
                ModUIUtils.MessagePopup(true, "Overhaul Mod update available!", $"Version {newVersion} is available to install via updates menu.\nWould you like to install it now?", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "ok", "Yes", "No", null, delegate
                {
                    UI.UIUpdatesWindow window = ModUIConstants.ShowUpdatesWindow();
                    window.SelectBranchAndSearchForUpdates(isTester ? 1 : 0);
                });
            }

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

                    if (ModBuildInfo.isPrereleaseBuild || ExclusiveContentManager.Instance.IsLocalUserTheTester())
                    {
                        if (updateInfoList.InternalRelease != null && updateInfoList.InternalRelease.ModVersion != null)
                            PlayerPrefs.SetString(NEW_TESTING_VERSION_PLAYER_PREF_KEY, updateInfoList.InternalRelease.ModVersion.ToString());
                    }
                    else
                    {
                        if (updateInfoList.ModBotRelease != null && updateInfoList.ModBotRelease.ModVersion != null)
                            PlayerPrefs.SetString(NEW_RELEASE_VERSION_PLAYER_PREF_KEY, updateInfoList.ModBotRelease.ModVersion.ToString());
                    }
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
                    prepareCurrentBuildForAnUpdate();
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
                    return LocalizationManager.Instance.GetTranslatedString("update_branch_release_description");
                case 1:
                    return LocalizationManager.Instance.GetTranslatedString("update_branch_testing_description");
                case 2:
                    return LocalizationManager.Instance.GetTranslatedString("update_branch_canary_description");
            }
            return LocalizationManager.Instance.GetTranslatedString("update_branch_unknown_description");
        }

        public List<Dropdown.OptionData> GetAvailableBranches()
        {
            return ExclusiveContentManager.Instance.IsLocalUserTheTester() ? BranchesForTestersDropdownOptions : BranchesDropdownOptions;
        }

        private void prepareCurrentBuildForAnUpdate()
        {
            string modFolder = ModCore.instance.ModInfo.FolderPath;
            string filePath = Path.Combine(modFolder, "ModInfo.json");
            string backupFilePath = Path.Combine(modFolder, "ModInfo.json.bak");
            if (File.Exists(filePath))
            {
                if (File.Exists(backupFilePath))
                    File.Delete(backupFilePath);

                File.Move(filePath, backupFilePath);
            }

            /*
            PlayerPrefs.SetString(PLAYER_PREF_KEY, modFolder);
            PlayerPrefs.Save();*/
        }
    }
}
