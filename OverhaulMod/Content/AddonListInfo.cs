using System.Collections.Generic;

namespace OverhaulMod.Content
{
    public class AddonListInfo
    {
        public List<AddonInfo> Addons;

        public void FixValues()
        {
            if (Addons == null)
                Addons = new List<AddonInfo>();
        }
    }
}
