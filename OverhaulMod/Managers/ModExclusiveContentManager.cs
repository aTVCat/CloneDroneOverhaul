using OverhaulMod.Content;
using OverhaulMod.Utils;
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

        public override void Awake()
        {
            base.Awake();
            _ = ModActionUtils.RunCoroutine(retrieveDataOnStartCoroutine());
        }

        private IEnumerator retrieveDataOnStartCoroutine()
        {
            yield return new WaitForSecondsRealtime(2f);
            RetrieveDataFromRepository(null, null);
            yield break;
        }

        public void RetrieveDataFromRepository(Action doneCallback, Action<string> errorCallback)
        {
            contentInfoList = null;
            error = null;

            ModUserDataManager dataManager = ModUserDataManager.Instance;
            if (dataManager.HasFile(REPOSITORY_FILE, false))
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
            });
        }
    }
}
