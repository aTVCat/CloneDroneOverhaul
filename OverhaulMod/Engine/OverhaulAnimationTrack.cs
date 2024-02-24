using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class OverhaulAnimationTrack
    {
        public List<OverhaulAnimationKeyframe> Keyframes;

        public string ObjectPath;

        public void FixValues()
        {
            if (Keyframes == null)
                Keyframes = new List<OverhaulAnimationKeyframe>();
        }

        public void SetValueOnObject(object obj, int keyFrame)
        {

        }
    }
}
