using OverhaulMod.Utils;

namespace OverhaulMod.Combat.Levels
{
    public class ModLevelManager : Singleton<ModLevelManager>
    {
        public const string CHAPTER_SECTIONS_FOLDER = "chapterSections/";

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

            switch (chapterSectionInfo.ChapterIndex)
            {
                case 1:
                    MetagameProgressManager.Instance.ResetToChapter1();
                    break;
                case 2:
                    MetagameProgressManager.Instance.ResetToChapter2();
                    break;
                case 3:
                    MetagameProgressManager.Instance.ResetToChapter3();
                    break;
                case 4:
                    MetagameProgressManager.Instance.ResetToChapter4();
                    break;
                case 5:
                    MetagameProgressManager.Instance.ResetToChapter5();
                    break;
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
                _isDirty = true
            };
            ModGameUtils.overrideActiveSections = chapterSectionInfo.EnabledSections;
            ModGameUtils.overrideCurrentLevelId = chapterSectionInfo.LevelID;
            return true;
        }
    }
}
