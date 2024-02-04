using OverhaulMod.Utils;
using System.Collections.Generic;
using System.IO;

namespace OverhaulMod.Engine
{
    public class TitleScreenBackgroundInfo
    {
        public LevelDescription Level;

        public DifficultyTier Tier;

        public bool IsImage()
        {
            return Level == null;
        }
    }
}
