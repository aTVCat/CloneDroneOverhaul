using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
