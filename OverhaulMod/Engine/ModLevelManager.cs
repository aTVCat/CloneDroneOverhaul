using OverhaulMod.Utils;
using System.Collections.Generic;

namespace OverhaulMod.Engine
{
    public class ModLevelManager : Singleton<ModLevelManager>
    {
        public const string CHAPTER_SECTIONS_FOLDER = "chapterSections/";

        public const string CHAPTER_1_SECTIONS_CACHE_KEY = "StoryC1_Sections";
        public const string CHAPTER_2_SECTIONS_CACHE_KEY = "StoryC2_Sections";

        private string m_chapterSectionsFolder;
        public string chapterSectionsFolder
        {
            get
            {
                if (m_chapterSectionsFolder == null)
                {
                    m_chapterSectionsFolder = ModCore.dataFolder + CHAPTER_SECTIONS_FOLDER;
                }
                return m_chapterSectionsFolder;
            }
        }

        public bool SetStoryModeLevelProgress(ModLevelSectionInfo chapterSectionInfo)
        {
            if (chapterSectionInfo.DeserializationError)
                return false;

            GameDataManager gameDataManager = GameDataManager.Instance;
            if (!gameDataManager)
                return false;

            MetagameProgressManager metagameProgressManager = MetagameProgressManager.Instance;
            switch (chapterSectionInfo.ChapterIndex)
            {
                case 1:
                case 2:
                    metagameProgressManager._data.CurrentProgress = chapterSectionInfo.MetaGameProgress;
                    metagameProgressManager._data.DifficultyLastMeasuredForLevelID = null;
                    metagameProgressManager.saveData();
                    break;
                case 3:
                    metagameProgressManager.ResetToChapter3();
                    if (chapterSectionInfo.Order == 0)
                    {
                        metagameProgressManager.ResetToChapter3();
                    }
                    else
                    {

                        metagameProgressManager._data.CurrentProgress = MetagameProgress.P6_EnteredFleetBeacon;
                        metagameProgressManager._data.DifficultyLastMeasuredForLevelID = null;
                        metagameProgressManager.saveData();
                    }
                    break;
                case 4:
                    if (chapterSectionInfo.Order == 0)
                    {
                        metagameProgressManager.ResetToChapter4();
                    }
                    else
                    {

                        metagameProgressManager._data.CurrentProgress = MetagameProgress.P8_SpawnedInBattleCruiser;
                        metagameProgressManager._data.DifficultyLastMeasuredForLevelID = null;
                        metagameProgressManager.saveData();
                    }
                    break;
                case 5:
                    metagameProgressManager.ResetToChapter5();
                    break;
            }

            List<string> fakeLevelIds = new List<string>();
            if (chapterSectionInfo.ChapterIndex == 1 || chapterSectionInfo.ChapterIndex == 2)
            {
                for (int i = 0; i < chapterSectionInfo.Order; i++)
                {
                    fakeLevelIds.Add("FakeLevelID_" + i);
                }
            }

            gameDataManager._storyModeData = new GameData
            {
                CurentLevelID = chapterSectionInfo.LevelID,
                CurrentLevelSectionsVisible = chapterSectionInfo.EnabledSections,
                TransferredToEnemyType = EnemyType.Spear1,
                AllyTransferredToEnemyType = EnemyType.Swordsman5,
                OldTransferredToEnemyType = EnemyType.Spear1,
                OldAllyTransferredToEnemyType = EnemyType.Swordsman5,
                NumConsciousnessTransfersLeft = 5,
                PlayerUpgrades = UpgradeManager.Instance.CreateDefaultPlayerUpgrades(),
                AvailableSkillPoints = chapterSectionInfo.Order,
                LevelIDsBeatenThisPlaythrough = fakeLevelIds,
                HumanFacts = HumanFactsManager.Instance.GetRandomFactSet(),
                _isDirty = true
            };

            MetagameProgressManager.Instance._data.FirstEscapedHumansData = new EscapedHumanData
            {
                HumanFacts = HumanFactsManager.Instance.GetRandomFactSet(),
                Upgrades = UpgradeManager.Instance.CreateDefaultPlayerUpgrades()
            };
            MetagameProgressManager.Instance._data.SecondEscapedHumansData = new EscapedHumanData
            {
                HumanFacts = HumanFactsManager.Instance.GetRandomFactSet(),
                Upgrades = UpgradeManager.Instance.CreateDefaultPlayerUpgrades()
            };

            ModGameUtils.overrideActiveSections = chapterSectionInfo.EnabledSections;
            ModGameUtils.overrideCurrentLevelId = chapterSectionInfo.LevelID;
            return true;
        }

        public ModLevelSectionInfo[] GenerateChapterSections(bool isSecondChapter)
        {
            string cacheKey = isSecondChapter ? CHAPTER_2_SECTIONS_CACHE_KEY : CHAPTER_1_SECTIONS_CACHE_KEY;
            if (ModAdvancedCache.TryGet(cacheKey, out ModLevelSectionInfo[] sections))
                return sections;

            int chapterIndex = isSecondChapter ? 2 : 1;
            List<ModLevelSectionInfo> list = new List<ModLevelSectionInfo>();
            foreach (LevelDescription level in LevelManager.Instance._storyModeLevels)
            {
                if (level == null || string.IsNullOrEmpty(level.LevelID))
                    continue;

                int chapterLevel = ModParseUtils.TryParseToInt(level.LevelID.Replace("Story", string.Empty), -1) - (isSecondChapter ? 10 : 0);
                if (chapterLevel >= 1 && chapterLevel <= 10)
                {
                    list.Add(new ModLevelSectionInfo()
                    {
                        LevelID = level.LevelID,
                        EnabledSections = new List<string>(),
                        DisplayName = $"Level {chapterLevel} {GetChapterLevelEventName(chapterLevel, chapterIndex, true)}",
                        Order = chapterLevel - 1,
                        ChapterIndex = chapterIndex,
                        MetaGameProgress = GetMetagameProgress(chapterLevel, chapterIndex),
                    });
                }
            }
            sections = list.ToArray();
            ModAdvancedCache.Add(cacheKey, sections);
            return sections;
        }

        public MetagameProgress GetMetagameProgress(int levelIndex, int chapterIndex)
        {
            MetagameProgress metagameProgress = MetagameProgress.P0_None;
            if (chapterIndex == 1)
            {
                if (levelIndex >= 6)
                    metagameProgress = MetagameProgress.P1_EmperorArrived;
            }
            else if (chapterIndex == 2)
            {
                if (levelIndex < 5)
                    metagameProgress = MetagameProgress.P2_FirstHumanEscaped;
                else if (levelIndex == 5)
                    metagameProgress = MetagameProgress.P3_ReachedAlphaCentauri;
                else if (levelIndex > 5)
                    metagameProgress = MetagameProgress.P4_HarvestStarted;
            }
            return metagameProgress;
        }

        public string GetChapterLevelEventName(int levelIndex, int chapterIndex, bool addBrackets)
        {
            string result = string.Empty;
            if (chapterIndex == 1)
            {
                if (levelIndex == 5)
                    result = "Emperor arrival";
            }
            else if (chapterIndex == 2)
            {
                if (levelIndex == 5)
                    result = "Alpha Centauri";
                if (levelIndex == 6)
                    result = "Harvest start";
            }
            return addBrackets && result != string.Empty ? $"({result})" : result;
        }
    }
}
