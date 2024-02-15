using OverhaulMod.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using static Mono.Security.X509.X520;

namespace OverhaulMod.Content
{
    public class NewsManager : Singleton<NewsManager>
    {
        public const string DATA_REFRESH_TIME_PLAYER_PREF_KEY = "NewsInfoRefreshDate";

        public const string PREV_NEWS_COUNT_PREF_KEY = "NewsInfoCount";

        public const string REPOSITORY_FILE = "NewsInfo.json";

        public const string DATA_FILE = "NewsUserData.json";

        public static bool DebugGetNewList = false;

        private NewsInfoList m_downloadedNewsInfoList;

        private NewsUserData m_userData;

        public static float timeToToClearCache
        {
            get;
            set;
        }

        public static int downloadedNewsCount
        {
            get;
            set;
        }

        private void Start()
        {
            LoadUserData();
            _ = ModActionUtils.RunCoroutine(retrieveDataOnStartCoroutine());
        }

        private IEnumerator retrieveDataOnStartCoroutine()
        {
            downloadedNewsCount = PlayerPrefs.GetInt(PREV_NEWS_COUNT_PREF_KEY, 0);
            if (DateTime.TryParse(PlayerPrefs.GetString(DATA_REFRESH_TIME_PLAYER_PREF_KEY, "default"), out DateTime timeToRefreshData))
                if (DateTime.Now < timeToRefreshData)
                    yield break;

            yield return new WaitUntil(() => MultiplayerLoginManager.Instance.IsLoggedIntoPlayfab());
            yield return new WaitForSecondsRealtime(2f);

            DownloadNewsInfoFile(delegate
            {
                PlayerPrefs.SetString(DATA_REFRESH_TIME_PLAYER_PREF_KEY, DateTime.Now.AddDays(3).ToString());
            }, null, false);
            yield break;
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

        public bool ShouldHighlightNewsButton()
        {
            return m_userData != null && m_userData.ShouldHighlightNewsButton(m_downloadedNewsInfoList);
        }

        public void SetHasSeenNews()
        {
            if (m_downloadedNewsInfoList == null || m_downloadedNewsInfoList.News.IsNullOrEmpty())
                return;

            NewsUserData newsUserData = m_userData;
            if (newsUserData == null)
            {
                newsUserData = new NewsUserData();
                m_userData = newsUserData;
            }
            newsUserData.FixValues();

            newsUserData.NumOfNewsSeen = m_downloadedNewsInfoList.News.Count;

            SaveUserData();
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
                m_downloadedNewsInfoList = null;

            if (m_downloadedNewsInfoList != null)
            {
                DelegateScheduler.Instance.Schedule(delegate
                {
                    if (m_downloadedNewsInfoList.News != null)
                    {
                        int c = m_downloadedNewsInfoList.News.Count;
                        PlayerPrefs.SetInt(PREV_NEWS_COUNT_PREF_KEY, c);
                        downloadedNewsCount = c;
                    }

                    callback?.Invoke(m_downloadedNewsInfoList);
                }, 1f);
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

                    int c = newsInfoList.News.Count;
                    PlayerPrefs.SetInt(PREV_NEWS_COUNT_PREF_KEY, c);
                    downloadedNewsCount = c;
                }
                catch (Exception exc)
                {
                    errorCallback?.Invoke(exc.ToString());
                    return;
                }
                m_downloadedNewsInfoList = newsInfoList;
                callback?.Invoke(newsInfoList);
            }, errorCallback, out _);
        }
    }
}
