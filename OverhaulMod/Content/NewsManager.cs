using OverhaulMod.Utils;
using System;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class NewsManager : Singleton<NewsManager>
    {
        public const string REPOSITORY_FILE = "NewsInfo.json";

        public static bool DebugGetNewList = false;

        private NewsInfoList m_downloadedNewsInfoList;

        public void DownloadNewsInfoFile(Action<NewsInfoList> callback, Action<string> errorCallback, bool clearCache = false)
        {
            if (DebugGetNewList)
            {
                callback?.Invoke(new NewsInfoList() { News = new System.Collections.Generic.List<NewsInfo>() });
                return;
            }

            if (clearCache)
            {
                m_downloadedNewsInfoList = null;
                UnityWebRequest.ClearCookieCache();
            }

            if (m_downloadedNewsInfoList != null)
            {
                callback?.Invoke(m_downloadedNewsInfoList);
                return;
            }

            ContentRepositoryManager.Instance.GetTextFile(REPOSITORY_FILE, delegate (string content)
            {
                NewsInfoList newsInfoList = null;
                try
                {
                    newsInfoList = ModJsonUtils.Deserialize<NewsInfoList>(content);
                    if (newsInfoList.News == null)
                        newsInfoList.News = new System.Collections.Generic.List<NewsInfo>();
                }
                catch (Exception exc)
                {
                    errorCallback?.Invoke(exc.ToString());
                    return;
                }
                callback?.Invoke(newsInfoList);
            }, errorCallback, out _);
        }
    }
}
