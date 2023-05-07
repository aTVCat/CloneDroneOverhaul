using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace CDOverhaul.NetworkAssets.AdditionalContent
{
    public class OverhaulAdditionalContentController : OverhaulController
    {
        public const string ContentReloadedEventString = "OverhaulContentReloaded";

        public const string AdditionalContentDirectoryName = "AdditionalContent";
        public static string AdditionalContentDirectory { get; private set; }

        public static readonly string[] ExcludedDirectories = new string[]
        {
            "Archives"
        };

        public static bool IsReloadingContent
        {
            get;
            private set;
        }

        public static bool HasToReloadContent = true;
        public static OverhaulAdditionalContentUserData UserData;
        public static readonly List<OverhaulAdditionalContentPackInfo> LocalContent = new List<OverhaulAdditionalContentPackInfo>();
        public static readonly List<string> LoadedContent = new List<string>();
        public static List<OverhaulAdditionalContentPackInfo> GetLoadedContent()
        {
            List<OverhaulAdditionalContentPackInfo> result = new List<OverhaulAdditionalContentPackInfo>();
            if (LoadedContent.IsNullOrEmpty())
            {
                return result;
            }

            foreach (OverhaulAdditionalContentPackInfo pack in LocalContent)
            {
                if (LoadedContent.Contains(pack.PackID))
                {
                    result.Add(pack);
                }
            }
            return result;
        }

        public override void Initialize()
        {
            if (UserData == null)
            {
                UserData = OverhaulDataBase.GetData<OverhaulAdditionalContentUserData>("OverhaulUserAdditContentData");
            }

            if (string.IsNullOrEmpty(AdditionalContentDirectory))
            {
                AdditionalContentDirectory = OverhaulMod.Core.ModDirectory + AdditionalContentDirectoryName + "/";
            }

            if (HasToReloadContent)
            {
                DelegateScheduler.Instance.Schedule(delegate
                {
                    if (ErrorManager.Instance != null && ErrorManager.Instance.HasCrashed())
                    {
                        return;
                    }
                    ReloadContent();
                }, 0.2f);
            }
            else
            {
                OverhaulEventsController.DispatchEvent(ContentReloadedEventString);
            }
        }

        // todo: expand functionality, add coroutines
        public void ReloadContent()
        {
            StaticCoroutineRunner.StartStaticCoroutine(reloadContentCoroutine());
        }
        private IEnumerator reloadContentCoroutine()
        {
            IsReloadingContent = true;
            HasToReloadContent = false;

            yield return null;
            if (!LoadedContent.IsNullOrEmpty())
            {
                foreach (OverhaulAdditionalContentPackInfo pack in GetLoadedContent())
                {
                    pack.Unload();
                    yield return null;
                }
            }

            LocalContent.Clear();
            LocalContent.AddRange(GetAllInstalledContent());
            yield return null;

            if (LocalContent.IsNullOrEmpty())
            {
                IsReloadingContent = false;
                OverhaulEventsController.DispatchEvent(ContentReloadedEventString);
                yield break;
            }

            foreach (OverhaulAdditionalContentPackInfo packToLoad in LocalContent)
            {
                if (packToLoad != null && packToLoad.IsEnabled())
                {
                    packToLoad.Load();
                    while (packToLoad.LoadingProgress != 1f)
                    {
                        yield return null;
                    }
                }
            }
            yield return null;

            IsReloadingContent = false;
            OverhaulEventsController.DispatchEvent(ContentReloadedEventString);
            yield break;
        }

        #region Content detection

        private string[] getContentFolders()
        {
            string path = AdditionalContentDirectory;
            bool folderExists = Directory.Exists(path);
            return !folderExists ? null : Directory.GetDirectories(path);
        }

        private List<string> getRawContentInfos(string[] directories)
        {
            if (directories.IsNullOrEmpty() || directories.Length < 2)
            {
                return null;
            }

            List<string> rawInfoList = new List<string>();
            foreach (string directory in directories)
            {
                // Skip if excluded
                foreach (string excluded in ExcludedDirectories)
                {
                    if (directory.EndsWith(excluded))
                    {
                        continue;
                    }
                }

                string infoFilePath = directory + "/ContentInfo.json";
                bool infoFileExists = File.Exists(infoFilePath);
                if (!infoFileExists)
                {
                    continue;
                }

                string content = OverhaulCore.ReadTextFile(infoFilePath);
                rawInfoList.Add(content);
            }
            return rawInfoList;
        }

        private List<OverhaulAdditionalContentPackInfo> getPackInfos(List<string> rawContent)
        {
            List<OverhaulAdditionalContentPackInfo> result = new List<OverhaulAdditionalContentPackInfo>();
            int index = 0;
            do
            {
                try
                {
                    OverhaulAdditionalContentPackInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<OverhaulAdditionalContentPackInfo>(rawContent[index]);
                    info.PreviousFolderName = info.GetPackFolderName();
                    result.Add(info);
                }
                catch
                {
                }
                index++;
            } while (index < rawContent.Count);

            return result;
        }

        public List<OverhaulAdditionalContentPackInfo> GetAllInstalledContent()
        {
            return getPackInfos(getRawContentInfos(getContentFolders()));
        }

        public static bool IsInstalled(OverhaulAdditionalContentPackInfo info)
        {
            return info != null && IsInstalled(info.PackID);
        }

        public static bool IsInstalled(string guid)
        {
            if (LocalContent.IsNullOrEmpty())
            {
                return false;
            }

            foreach (OverhaulAdditionalContentPackInfo info1 in LocalContent)
            {
                if (info1 != null && info1.PackID == guid)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsLoaded(string guid)
        {
            return LoadedContent.Contains(guid);
        }

        #endregion

        public override string[] Commands()
        {
            throw new NotImplementedException();
        }
        public override string OnCommandRan(string[] command)
        {
            throw new NotImplementedException();
        }
    }
}
