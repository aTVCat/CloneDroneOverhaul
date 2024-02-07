using OverhaulMod.Utils;
using System;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class NewsManager : Singleton<NewsManager>
    {
        public const string REPOSITORY_FILE = "NewsInfo.json";

        public const string DATA_FILE = "NewsUserData.json";

        public static bool DebugGetNewList = false;

        private NewsInfoList m_downloadedNewsInfoList;

        private NewsUserData m_userData;

        private void Start()
        {
            LoadUserData();
        }

        public void LoadUserData()
        {
            if (m_userData != null)
                return;

            NewsUserData newsUserData;
            try
            {
                newsUserData = ModDataManager.Instance.DeserializeFile<NewsUserData>(DATA_FILE, false);
                newsUserData.FixValues();
            }
            catch
            {
                newsUserData = new NewsUserData();
                newsUserData.FixValues();
            }
            m_userData = newsUserData;
        }

        public void SaveUserData()
        {
            NewsUserData newsUserData = m_userData;
            if (newsUserData == null)
            {
                newsUserData = new NewsUserData();
                m_userData = newsUserData;
            }

            newsUserData.FixValues();

            try
            {
                ModDataManager.Instance.SerializeToFile(DATA_FILE, newsUserData, false);
            }
            catch { }
        }

        public bool HasAnsweredSurvey(string title)
        {
            return m_userData != null && m_userData.HasAnswered(title);
        }

        public void SetHasAnsweredSurvey(string title)
        {
            NewsUserData newsUserData = m_userData;
            if (newsUserData == null)
            {
                newsUserData = new NewsUserData();
                m_userData = newsUserData;
            }

            newsUserData.FixValues();

            if (!newsUserData.AnsweredSurveys.Contains(title))
                newsUserData.AnsweredSurveys.Add(title);

            SaveUserData();
        }

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

            RepositoryManager.Instance.GetTextFile(REPOSITORY_FILE, delegate (string content)
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
