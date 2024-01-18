using OverhaulMod.Utils;
using System;
using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class ModLevelDescriptionList
    {
        public List<LevelDescription> LevelDescriptions;

        [NonSerialized]
        private List<LevelDescription> m_fixedLevelDescriptions;

        public List<LevelDescription> GetFixedLevelDescriptions()
        {
            if (m_fixedLevelDescriptions != null)
                return m_fixedLevelDescriptions;

            if (LevelDescriptions.IsNullOrEmpty())
                return new List<LevelDescription>();

            List<LevelDescription> fixedLevelDescriptions = new List<LevelDescription>();
            foreach (LevelDescription level in LevelDescriptions)
            {
                LevelDescription levelDescription = new LevelDescription()
                {
                    LevelJSONPath = ModCore.dataFolder + "levels/" + level.LevelJSONPath,
                    LevelID = level.LevelID,
                    LevelEditorDifficultyIndex = level.LevelEditorDifficultyIndex,
                    DifficultyTier = level.DifficultyTier,
                    LevelTags = new List<LevelTags>()
                };
                fixedLevelDescriptions.Add(levelDescription);
            }
            m_fixedLevelDescriptions = fixedLevelDescriptions;
            return fixedLevelDescriptions;
        }
    }
}
