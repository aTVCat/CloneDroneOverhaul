using OverhaulMod.Utils;
using System.IO;

namespace OverhaulMod.Engine
{
    public class TitleScreenCustomizationInfo
    {
        public TitleScreenBackgroundInfo StaticBackgroundInfo;

        public void FixValues()
        {
            if (StaticBackgroundInfo == null) StaticBackgroundInfo = new TitleScreenBackgroundInfo();

            LevelDescription levelDescription = StaticBackgroundInfo.Level;
            if (levelDescription == null) return;

            if (levelDescription.LevelJSONPath.IsNullOrEmpty() || !File.Exists(levelDescription.LevelJSONPath))
            {
                StaticBackgroundInfo.Level = null;
            }
        }
    }
}
