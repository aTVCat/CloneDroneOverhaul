using OverhaulMod.Utils;
using System.Collections.Generic;

namespace OverhaulMod.Content
{
    public class NewsUserData
    {
        public List<string> AnsweredSurveys;

        public int NumOfNewsSeen;

        public void FixValues()
        {
            if (AnsweredSurveys == null)
                AnsweredSurveys = new List<string>();
        }

        public bool HasAnswered(string title)
        {
            return AnsweredSurveys.Contains(title);
        }

        public bool ShouldHighlightNewsButton(NewsInfoList newsInfoList)
        {
            if(newsInfoList == null || newsInfoList.News.IsNullOrEmpty())
            {
                return NewsManager.downloadedNewsCount > NumOfNewsSeen;
            }
            return newsInfoList.News.Count > NumOfNewsSeen;
        }
    }
}
