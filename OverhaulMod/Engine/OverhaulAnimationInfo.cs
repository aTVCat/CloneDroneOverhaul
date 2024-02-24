using System.Collections.Generic;

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
