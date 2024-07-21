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
            return newsInfoList == null || newsInfoList.News.IsNullOrEmpty()
                ? NewsManager.downloadedNewsCount > NumOfNewsSeen
                : newsInfoList.News.Count > NumOfNewsSeen;
        }
    }
}
