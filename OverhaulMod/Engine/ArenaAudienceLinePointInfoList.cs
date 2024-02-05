using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class ArenaAudienceLinePointInfoList
    {
        public List<ArenaAudienceLinePointInfo> Points;

        public void FixValues()
        {
            if (Points == null)
                Points = new List<ArenaAudienceLinePointInfo>();
        }
    }
}
