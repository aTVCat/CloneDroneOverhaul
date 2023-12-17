using OverhaulMod.Utils;
using System.Collections.Generic;

namespace OverhaulMod.Combat.Levels
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

        public bool SetStoryModeLevelProgress(ChapterSectionInfo chapterSectionInfo)
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
            if(chapterSectionInfo.ChapterIndex == 1 || chapterSectionInfo.ChapterIndex == 2)
            {
                for(int i = 0; i < chapterSectionInfo.Order; i++)
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
                _isDirty = true
            };
            ModGameUtils.overrideActiveSections = chapterSectionInfo.EnabledSections;
            ModGameUtils.overrideCurrentLevelId = chapterSectionInfo.LevelID;
            return true;
        }

        public ChapterSectionInfo[] GenerateChapter1Sections()
        {
            if (ModAdvancedCache.TryGet(CHAPTER_1_SECTIONS_CACHE_KEY, out ChapterSectionInfo[] sections))
                return sections;

            int index = 1;
            List<ChapterSectionInfo> list = new List<ChapterSectionInfo>();
            foreach (LevelDescription level in LevelManager.Instance._storyModeLevels)
            {
                string levelId = level.LevelID.Replace("Story", string.Empty);
                int parsedInt = ModParseUtils.TryParseToInt(levelId, -1);
                if (parsedInt >= 1 && parsedInt <= 10)
                {
                    list.Add(new ChapterSectionInfo()
                    {
                        LevelID = level.LevelID,
                        EnabledSections = new List<string>(),
                        DisplayName = "Level " + parsedInt,
                        Order = parsedInt - 1,
                        ChapterIndex = 1,
                        MetaGameProgress = GetMetagameProgress(parsedInt, 1),
                    });
                }
                index++;
            }
            sections = list.ToArray();
            ModAdvancedCache.Add(CHAPTER_1_SECTIONS_CACHE_KEY, sections);
            return sections;
        }

        public ChapterSectionInfo[] GenerateChapter2Sections()
        {
            if (ModAdvancedCache.TryGet(CHAPTER_2_SECTIONS_CACHE_KEY, out ChapterSectionInfo[] sections))
                return sections;

            int index = 0;
            List<ChapterSectionInfo> list = new List<ChapterSectionInfo>();
            foreach (LevelDescription level in LevelManager.Instance._storyModeLevels)
            {
                string levelId = level.LevelID.Replace("Story", string.Empty);
                int parsedInt = ModParseUtils.TryParseToInt(levelId, -1);
                if (parsedInt >= 11 && parsedInt <= 20)
                {
                    list.Add(new ChapterSectionInfo()
                    {
                        LevelID = level.LevelID,
                        EnabledSections = new List<string>(),
                        DisplayName = "Level " + (parsedInt - 10),
                        Order = parsedInt - 11,
                        ChapterIndex = 1,
                        MetaGameProgress = GetMetagameProgress(parsedInt - 10, 2),
                    });
                }
                index++;
            }
            sections = list.ToArray();
            ModAdvancedCache.Add(CHAPTER_2_SECTIONS_CACHE_KEY, sections);
            return sections;
        }

        public MetagameProgress GetMetagameProgress(int levelIndex, int chapterIndex)
        {
            MetagameProgress metagameProgress = MetagameProgress.P0_None;
            if(chapterIndex == 1)
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
                else if (levelIndex >= 6)
                    metagameProgress = MetagameProgress.P4_HarvestStarted;
            }
            return metagameProgress;
        }
    }
}
