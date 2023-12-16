using System;
using System.Collections.Generic;

namespace OverhaulMod.Combat.Levels
{
    public class ChapterSectionInfo
    {
        public List<string> EnabledSections;

        public string LevelID;

        public string DisplayName;

        public int Order;

        public int ChapterIndex;

        [NonSerialized]
        public bool DeserializationError;
    }
}
