using ICSharpCode.SharpZipLib.Zip;
using InternalModBot;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.Content
{
    public class UpdateManager : Singleton<UpdateManager>
    {
        public const string REPOSITORY_FILE = "UpdateInfo.json";

        [ModSetting(ModSettingsConstants.CHECK_FOR_UPDATES_ON_STARTUP, true)]
        public static bool CheckForUpdatesOnStartup;

        [ModSetting(ModSettingsConstants.NOTIFY_ABOUT_NEW_TEST_BUILDS, false)]
        public static bool NotifyAboutNewTestBuilds;

        [ModSetting(ModSettingsConstants.SAVED_NEW_VERSION, null, ModSetting.Tag.IgnoreExport)]
        public static string SavedNewVersion;

        [ModSetting(ModSettingsConstants.UPDATES_LAST_CHECKED_DATE, null, ModSetting.Tag.IgnoreExport)]
        public static string UpdatesLastCheckedDate;

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

        public static System.Version downloadedVersion
        {
            get;
            set;
        }

        public bool GetUpdatesFromTestFolder = true;

        private UnityWebRequest m_webRequest;

        private float m_buildDownloadProgress;

        private UpdateInfoList m_updatesList;

        private void Start()
        {
            if (!CheckForUpdatesOnStartup)
            {
                downloadedVersion = ModBuildInfo.version;
                return;
            }

            ScheduledActionsManager scheduledActionsManager = ScheduledActionsManager.Instance;
            if (!scheduledActionsManager.ShouldExecuteAction(ScheduledActionType.RefreshModUpdates))
                return;

            _ = ModActionUtils.RunCoroutine(retrieveDataOnStartCoroutine());
        }

        private void Update()
        {
            if (m_webRequest != null)
            {
                try
                {
                    m_buildDownloadProgress = m_webRequest.downloadProgress;
                }
                catch
                {
                    m_webRequest = null;
                }
            }
        }

        public void LoadDataFromDisk()
        {
            UpdateInfoList infoList;
            try
            {
                infoList = ModJsonUtils.DeserializeStream<UpdateInfoList>(Path.Combine(ModCore.developerFolder, REPOSITORY_FILE));
            }
            catch
            {
                infoList = new UpdateInfoList();
            }
            infoList.FixValues();

            m_updatesList = infoList;
        }

        public UpdateInfoList GetUpdatesList()
        {
            return m_updatesList;
        }

        public float GetBuildDownloadProgress()
        {
            return m_buildDownloadProgress;
        }

        private IEnumerator retrieveDataOnStartCoroutine()
        {
            DownloadUpdateInfoFile(delegate
            {
                ScheduledActionsManager.Instance.SetActionExecuted(ScheduledActionType.RefreshModUpdates);
            }, null);
            yield break;
        }

        public bool ShouldHighlightUpdatesButton()
        {
            return downloadedVersion > ModBuildInfo.version;
        }

        public void DownloadUpdatesList(Action<GetUpdatesResult> callback)
        {
            string prefix = string.Empty;
            if (GetUpdatesFromTestFolder)
            {
                prefix = "test/";
            }

            RepositoryManager.Instance.GetTextFile(prefix + REPOSITORY_FILE, delegate (string content)
            {
                UpdateInfoList updateInfoList;
                try
                {
                    updateInfoList = ModJsonUtils.Deserialize<UpdateInfoList>(content);
                    updateInfoList.FixValues();
                }
                catch (Exception exc)
                {
                    callback?.Invoke(new GetUpdatesResult(exc.ToString()));
                    return;
                }

                System.Version maxVersion = null;
                foreach (KeyValuePair<string, UpdateInfo> build in updateInfoList.Builds)
                {
                    if (build.Value.IsOlderBuild() || !build.Value.CanBeInstalledByLocalUser())
                        continue;

                    if (build.Key == UpdateInfoList.RELEASE_BRANCH || NotifyAboutNewTestBuilds)
                    {
                        if (maxVersion == null || build.Value.ModVersion > maxVersion)
                        {
                            maxVersion = build.Value.ModVersion;
                        }
                    }
                }

                if(maxVersion != null)
                {
                    ModSettingsManager.SetStringValue(ModSettingsConstants.SAVED_NEW_VERSION, maxVersion.ToString());
                }
                else
                {
                    ModSettingsManager.SetStringValue(ModSettingsConstants.SAVED_NEW_VERSION, "0.0.0.0");
                }
                ModSettingsDataManager.Instance.Save();

                m_updatesList = updateInfoList;
                callback?.Invoke(new GetUpdatesResult(updateInfoList));
            }, delegate (string error)
            {
                callback?.Invoke(new GetUpdatesResult(error));
            }, out _);
        }

        public void DownloadUpdateInfoFile(Action<UpdateInfoList> callback, Action<string> errorCallback)
        {
            RepositoryManager.Instance.GetTextFile(REPOSITORY_FILE, delegate (string content)
            {
                UpdateInfoList updateInfoList = null;
                try
                {
                    updateInfoList = ModJsonUtils.Deserialize<UpdateInfoList>(content);
                    updateInfoList.FixValues();
                }
                catch (Exception exc)
                {
                    errorCallback?.Invoke(exc.ToString());
                    return;
                }
                callback?.Invoke(updateInfoList);
            }, errorCallback, out _);
        }

        public void DownloadBuild(string url, bool isGoogleDriveLink, string directoryName, Action<InstallUpdateResult> callback)
        {
            m_buildDownloadProgress = 0f;

            string directoryPath = Path.Combine(ModsManager.Instance.ModFolderPath, directoryName);
            if (Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.Delete(directoryPath, true);
                }
                catch (Exception exc)
                {
                    callback?.Invoke(new InstallUpdateResult(exc.ToString()));
                    return;
                }
            }

            if (isGoogleDriveLink)
            {
                string tempPath = Path.GetTempFileName();
                GoogleDriveManager.Instance.DownloadFile(url, tempPath, delegate (float progress)
                {
                    m_buildDownloadProgress = progress;
                }, delegate (string result)
                {
                    if (result != null)
                    {
                        callback?.Invoke(new InstallUpdateResult(result));
                        return;
                    }

                    callback?.Invoke(new InstallUpdateResult(installBuild(tempPath, directoryPath)));
                });
                return;
            }

            RepositoryManager.Instance.GetCustomFile(url, delegate (byte[] bytes)
            {
                m_webRequest = null;

                string tempFile = Path.GetTempFileName();
                ModFileUtils.WriteBytes(bytes, tempFile);

                callback?.Invoke(new InstallUpdateResult(installBuild(tempFile, directoryPath)));
            }, delegate (string error)
            {
                m_webRequest = null;
                callback?.Invoke(new InstallUpdateResult(error));
            }, out m_webRequest, -1);
        }

        private string installBuild(string archivePath, string targetDirectory)
        {
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(archivePath, targetDirectory, null);
            if (File.Exists(archivePath))
                File.Delete(archivePath);

            try
            {
                prepareCurrentBuildForAnUpdate();
            }
            catch (Exception exc)
            {
                return "Prepare build error: " + exc;
            }
            return null;
        }

        public void DownloadBuildFromSource(string source, string directoryName, Action callback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = null;
            string directoryPath = Path.Combine(ModsManager.Instance.ModFolderPath, directoryName);
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
                ModFileUtils.WriteBytes(bytes, tempFile);

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
            return ExclusivePerkManager.Instance.IsLocalUserTheTester() ? BranchesForTestersDropdownOptions : BranchesDropdownOptions;
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
        }

        public class GetUpdatesResult : DownloadResult
        {
            public UpdateInfoList Updates;

            public GetUpdatesResult()
            {

            }

            public GetUpdatesResult(string error)
            {
                Error = error;
            }

            public GetUpdatesResult(UpdateInfoList updates)
            {
                Updates = updates;
            }
        }

        public class InstallUpdateResult : DownloadResult
        {
            public InstallUpdateResult()
            {

            }

            public InstallUpdateResult(string error)
            {
                Error = error;
            }
        }
    }
}
