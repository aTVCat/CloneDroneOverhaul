using System.Collections.Generic;

namespace OverhaulMod.Content
{
    public class ContentListInfo
    {
        public List<ContentDownloadInfo> ContentToDownload;

        public void FixValues()
        {
            if (ContentToDownload == null)
                ContentToDownload = new List<ContentDownloadInfo>();
        }
    }
}
