using System.Collections.Generic;

namespace OverhaulMod.Content
{
    public class AddonListInfo
    {
        public List<AddonDownloadInfo> Addons;

        public void FixValues()
        {
            if (Addons == null)
                Addons = new List<AddonDownloadInfo>();
        }
    }
}
