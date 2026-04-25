using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace OverhaulMod.Engine
{
    public class ModLevelDescriptionList
    {
        public List<LevelDescription> LevelDescriptions;

        [NonSerialized]
        private List<LevelDescription> _fixedLevelDescriptions;

        public List<LevelDescription> GetFixedLevelDescriptions()
        {
            if (_fixedLevelDescriptions != null)
                return _fixedLevelDescriptions;

            if (LevelDescriptions.IsNullOrEmpty())
                return new List<LevelDescription>();

            List<LevelDescription> fixedLevelDescriptions = new List<LevelDescription>();
            foreach (LevelDescription level in LevelDescriptions)
            {
                LevelDescription levelDescription = new LevelDescription()
                {
                    LevelJSONPath = Path.Combine(ModDirectories.DataFolder, "levels", level.LevelJSONPath),
                    LevelID = level.LevelID,
                    LevelEditorDifficultyIndex = level.LevelEditorDifficultyIndex,
                    DifficultyTier = level.DifficultyTier,
                    LevelTags = new List<LevelTags>()
                };
                fixedLevelDescriptions.Add(levelDescription);
            }
            _fixedLevelDescriptions = fixedLevelDescriptions;
            return fixedLevelDescriptions;
        }
    }
}
