using System.Collections.Generic;

namespace OverhaulMod.Content
{
    public class ExclusivePerkInfoList
    {
        public List<ExclusivePerkInfo> List;

        public void FixValues()
        {
            if (List == null)
                List = new List<ExclusivePerkInfo>();
        }
    }
}
