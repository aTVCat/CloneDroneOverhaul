using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Engine
{
    public class OverhaulAnimationInfo
    {
        public List<OverhaulAnimationTrack> Tracks;

        public void FixValues()
        {
            if (Tracks == null)
                Tracks = new List<OverhaulAnimationTrack>();
        }
    }
}
