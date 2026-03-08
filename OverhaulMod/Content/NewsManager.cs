using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections;

namespace OverhaulMod.Content
{
    /// <summary>
    /// out of order for now
    /// </summary>
    public class NewsManager : Singleton<NewsManager>
    {
        public const string REPOSITORY_FILE = "NewsInfo.json";

        private IEnumerator retrieveDataOnStartCoroutine()
        {
            ScheduledActionsManager scheduledActionsManager = ScheduledActionsManager.Instance;
            if (!scheduledActionsManager.ShouldExecuteAction(ScheduledActionType.RefreshNews))
                yield break;

            DownloadNewsInfoFile(delegate
            {
                scheduledActionsManager.SetActionExecuted(ScheduledActionType.RefreshNews);
            }, null);
            yield break;
        }

        public void DownloadNewsInfoFile(Action<NewsInfoList> callback, Action<string> errorCallback)
        {
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
                callback?.Invoke(newsInfoList);
            }, errorCallback, out _);
        }
    }
}
