using OverhaulMod.Utils;
using System;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class UpdateManager : Singleton<UpdateManager>
    {
        public const string REPOSITORY_FILE = "UpdateInfo.json";

        private UpdateInfoList m_downloadedUpdateInfoList;

        public void DownloadUpdateInfoFile(Action<UpdateInfoList> callback, Action<string> errorCallback, bool clearCache = false)
        {
            if (clearCache)
            {
                m_downloadedUpdateInfoList = null;
                UnityWebRequest.ClearCookieCache();
            }

            if(m_downloadedUpdateInfoList != null)
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
                catch(Exception exc)
                {
                    errorCallback?.Invoke(exc.ToString());
                    return;
                }
                callback?.Invoke(updateInfoList);
            }, errorCallback, out _);
        }
    }
}
