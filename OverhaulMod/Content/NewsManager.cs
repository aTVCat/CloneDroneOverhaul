using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Content
{
    public class NewsManager : Singleton<NewsManager>
    {
        public const string REPOSITORY_FILE = "NewsInfo.json";

        public const string DATA_FILE = "NewsUserData.json";

        [ModSetting(ModSettingsConstants.PREV_NEWS_COUNT, 0)]
        public static int PrevNewsCount;

        [ModSetting(ModSettingsConstants.DOWNLOADED_NEWS_COUNT, 0)]
        public static int DownloadedNewsCount;

        private NewsInfoList m_downloadedNewsInfoList;

        private NewsUserData m_userData;

        public static float timeToToClearCache
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
            ScheduledActionsManager scheduledActionsManager = ScheduledActionsManager.Instance;
            if (!scheduledActionsManager.ShouldExecuteAction(ScheduledActionType.RefreshNews))
                yield break;

            DownloadNewsInfoFile(delegate
            {
                scheduledActionsManager.SetActionExecuted(ScheduledActionType.RefreshNews);
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
            return DownloadedNewsCount != PrevNewsCount;
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
            if (clearCache)
                m_downloadedNewsInfoList = null;

            if (m_downloadedNewsInfoList != null)
            {
                DelegateScheduler.Instance.Schedule(delegate
                {
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

                    ModSettingsManager.SetIntValue(ModSettingsConstants.DOWNLOADED_NEWS_COUNT, newsInfoList.News.Count);
                    ModSettingsDataManager.Instance.Save();
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
