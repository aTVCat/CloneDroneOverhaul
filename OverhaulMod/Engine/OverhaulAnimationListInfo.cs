using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class OverhaulAnimationListInfo
    {
        public List<OverhaulAnimationInfo> Animations;

        public void FixValues()
        {
            if (Animations == null)
                Animations = new List<OverhaulAnimationInfo>();
        }
    }
}
