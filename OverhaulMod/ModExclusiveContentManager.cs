using OverhaulMod.Content;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod
{
    public class ModExclusiveContentManager : Singleton<ModExclusiveContentManager>
    {
        public const string REPOSITORY_FILE = "ContentInfoList.json";

        public ExclusiveContentInfoList contentInfoList
        {
            get;
            private set;
        }

        public string error
        {
            get;
            private set;
        }

        public bool hasRetrievedDataOnStart
        {
            get;
            private set;
        }

        public ulong localSteamId
        {
            get
            {
                return contentInfoList == null ? 0 : contentInfoList.LocalSteamID;
            }
        }

        public override void Awake()
        {
            base.Awake();
            _ = ModActionUtils.RunCoroutine(retrieveDataOnStartCoroutine());
        }

        public List<ExclusiveContentInfo> GetUnlockedContent()
        {
            List<ExclusiveContentInfo> list = new List<ExclusiveContentInfo>();
            if (contentInfoList != null && contentInfoList.List != null && contentInfoList.List.Count != 0)
                foreach (ExclusiveContentInfo info in contentInfoList.List)
                    if (info.IsAvailableToLocalUser())
                        list.Add(info);

            return list;
        }

        public List<ExclusiveContentInfo> GetContentOfType<T>() where T : ExclusiveContentBase
        {
            List<ExclusiveContentInfo> list = new List<ExclusiveContentInfo>();
            if (contentInfoList != null && contentInfoList.List != null && contentInfoList.List.Count != 0)
                foreach (ExclusiveContentInfo info in contentInfoList.List)
                    if (info.Content != null && info.Content.GetType() == typeof(T))
                        list.Add(info);

            return list;
        }

        private IEnumerator retrieveDataOnStartCoroutine()
        {
            yield return new WaitForEndOfFrame();
            RetrieveDataFromRepository(null, null, true);
            yield return new WaitUntil(() => MultiplayerLoginManager.Instance.IsLoggedIntoPlayfab());
            yield return new WaitForSecondsRealtime(2f);
            RetrieveDataFromRepository(delegate
            {
                hasRetrievedDataOnStart = true;
            }, delegate (string error)
            {
                hasRetrievedDataOnStart = true;
                this.error = error;
            }, false);
            yield break;
        }

        public void RetrieveDataFromRepository(Action doneCallback, Action<string> errorCallback, bool forceFileCheck = false)
        {
            contentInfoList = null;
            error = null;

            ModUserDataManager dataManager = ModUserDataManager.Instance;
            if (forceFileCheck && dataManager.HasFile(REPOSITORY_FILE, false))
            {
                ExclusiveContentInfoList contentInfoList = null;
                try
                {
                    contentInfoList = ModJsonUtils.DeserializeStream<ExclusiveContentInfoList>(dataManager.folder + REPOSITORY_FILE);
                }
                catch (Exception exc)
                {
                    string s = exc.ToString();
                    error = s;
                    errorCallback?.Invoke(s);
                    return;
                }
                this.contentInfoList = contentInfoList;
                doneCallback?.Invoke();
                return;
            }

            ModContentRepositoryManager repositoryManager = ModContentRepositoryManager.Instance;
            repositoryManager.GetTextFileContent(REPOSITORY_FILE, delegate (string contents)
            {
                if (string.IsNullOrEmpty(contents))
                {
                    this.contentInfoList = new ExclusiveContentInfoList() { List = new List<ExclusiveContentInfo>() };
                    doneCallback?.Invoke();
                    return;
                }

                ExclusiveContentInfoList contentInfoList = null;
                try
                {
                    contentInfoList = ModJsonUtils.Deserialize<ExclusiveContentInfoList>(contents);
                    contentInfoList.LocalSteamID = (ulong)SteamUser.GetSteamID();
                    ExclusiveContentEditor.Save(contents);
                }
                catch (Exception exc)
                {
                    string s = exc.ToString();
                    error = s;
                    errorCallback?.Invoke(s);
                    return;
                }
                this.contentInfoList = contentInfoList;
                doneCallback?.Invoke();
            }, delegate (string error)
            {
                this.error = error;
                errorCallback?.Invoke(error);
            }, 10);
        }

        public void GetOverrideRobotColor(FirstPersonMover firstPersonMover, Color currentColor, out Color newColor)
        {
            newColor = currentColor;
            foreach (ExclusiveContentInfo exclusiveContentInfo in GetContentOfType<ExclusiveContentColorOverride>())
            {
                exclusiveContentInfo.VerifyFields();
                if (exclusiveContentInfo.Content is ExclusiveContentColorOverride colorOv && colorOv.GetOverrideRobotColor(firstPersonMover, currentColor, out newColor))
                    return;
            }
        }
    }
}
