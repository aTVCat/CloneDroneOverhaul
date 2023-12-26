using System;
using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class ModLevelSectionInfo
    {
        public List<string> EnabledSections;

        public string LevelID;

        public string DisplayName;

        public int Order;

        public int ChapterIndex;

        [NonSerialized]
        public MetagameProgress MetaGameProgress;

        [NonSerialized]
        public bool DeserializationError;
    }
}
