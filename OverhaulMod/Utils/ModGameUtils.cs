using OverhaulMod.Combat.Levels;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OverhaulMod.Utils
{
    public static class ModGameUtils
    {
        private static List<string> s_overrideActiveSections;
        public static List<string> overrideActiveSections
        {
            get
            {
                var list = s_overrideActiveSections;
                s_overrideActiveSections = null;
                return list;
            }
            set
            {
                s_overrideActiveSections = value;
            }
        }

        private static string s_overrideCurrentLevelId;
        public static string overrideCurrentLevelId
        {
            get
            {
                var list = s_overrideCurrentLevelId;
                s_overrideCurrentLevelId = null;
                return list;
            }
            set
            {
                s_overrideCurrentLevelId = value;
            }
        }

        public static bool SyncSteamAchievements()
        {
            GameplayAchievementManager gameplayAchievementManager = GameplayAchievementManager.Instance;
            if (!gameplayAchievementManager)
                return false;

            bool result = false;
            foreach (GameplayAchievement achievement in gameplayAchievementManager.Achievements)
            {
                _ = SteamUserStats.GetAchievement(achievement.SteamAchievementID, out bool isComplete);

                int currentProgress = gameplayAchievementManager.GetProgress(achievement.AchievementID);
                int targetProgress = isComplete ? achievement.TargetProgress : 0;
                if (targetProgress != currentProgress)
                    result = true;

                if (isComplete)
                    gameplayAchievementManager.SetAchievementProgress(achievement, targetProgress, true);
                else
                    gameplayAchievementManager.SetAchievementProgress(achievement, 0, true);
            }
            return result;
        }

        public static void SaveActiveSections(string fileName, string displayName, int order, int chapterIndex)
        {
            if (!GameModeManager.Is(GameMode.Story))
                return;

            List<string> sections = new List<string>();
            if (GameModeManager.IsStoryModeAfterChapter3())
            {
                StoryCheckpoint[] checkpoints = UnityEngine.Object.FindObjectsOfType<StoryCheckpoint>();
                foreach (StoryCheckpoint storyCheckpoint in checkpoints)
                {
                    if (!storyCheckpoint || !storyCheckpoint.gameObject.activeInHierarchy)
                        continue;

                    SectionMember sectionMember = storyCheckpoint?.GetComponent<SectionMember>();
                    if (sectionMember)
                    {
                        sections.AddRange(sectionMember.GetSectionGUIDs());
                    }
                    break;
                }
            }
            else if (GameModeManager.IsStoryChapter3())
            {
                OutsideArenaSpawnPoint[] checkpoints = UnityEngine.Object.FindObjectsOfType<OutsideArenaSpawnPoint>();
                foreach (OutsideArenaSpawnPoint outsideArenaSpawnPoint in checkpoints)
                {
                    if (!outsideArenaSpawnPoint || !outsideArenaSpawnPoint.gameObject.activeInHierarchy)
                        continue;

                    SectionMember sectionMember = outsideArenaSpawnPoint?.GetComponent<SectionMember>();
                    if (sectionMember)
                    {
                        sections.AddRange(sectionMember.GetSectionGUIDs());
                    }
                    break;
                }
            }

            string content = ModJsonUtils.Serialize(new ChapterSectionInfo
            {
                LevelID = LevelManager.Instance.GetCurrentLevelID(),
                EnabledSections = sections,
                DisplayName = displayName,
                Order = order,
                ChapterIndex = chapterIndex,
            });

            ModUserDataManager.Instance.WriteFile(fileName + ".json", content, true);
            _ = ModIOUtils.OpenFileExplorer(ModUserDataManager.Instance.savesFolder);
        }

        public static ChapterSectionInfo[] GetChapterSections(string directory)
        {
            if(!Directory.Exists(directory))
                return Array.Empty<ChapterSectionInfo>();

            string[] files = Directory.GetFiles(directory, "*.json");
            if (files == null || files.Length == 0)
                return Array.Empty<ChapterSectionInfo>();

            int index = 0;
            ChapterSectionInfo[] sections = new ChapterSectionInfo[files.Length];
            foreach (string file in files)
            {
                ChapterSectionInfo chapterSectionInfo = null;
                try
                {
                    chapterSectionInfo = ModJsonUtils.DeserializeStream<ChapterSectionInfo>(file);
                }
                catch
                {
                    chapterSectionInfo = new ChapterSectionInfo() { DeserializationError = true };
                }
                sections[index] = chapterSectionInfo;
                index++;
            }
            sections = sections.ToList().OrderBy(t => t.Order).ToArray();
            return sections;
        }
    }
}
