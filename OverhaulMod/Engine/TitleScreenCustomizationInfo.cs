using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Engine
{
    public class TitleScreenCustomizationInfo
    {
        public TitleScreenBackgroundInfo StaticBackgroundInfo;

        public List<TitleScreenBackgroundInfo> SlideshowInfo;

        public void FixValues()
        {
            if (StaticBackgroundInfo == null)
                StaticBackgroundInfo = new TitleScreenBackgroundInfo();

            if (SlideshowInfo == null)
                SlideshowInfo = new List<TitleScreenBackgroundInfo>();
        }
    }
}
