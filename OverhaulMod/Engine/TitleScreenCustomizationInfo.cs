using System.Collections.Generic;
using System.IO;

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

            LevelDescription levelDescription = StaticBackgroundInfo.Level;
            if (levelDescription != null && !string.IsNullOrEmpty(levelDescription.LevelJSONPath) && !File.Exists(levelDescription.LevelJSONPath))
            {
                StaticBackgroundInfo.Level = null;
            }
        }
    }
}
