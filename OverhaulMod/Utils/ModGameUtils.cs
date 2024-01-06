using OverhaulMod.Engine;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OverhaulMod.Utils
{
    public static class ModGameUtils
    {
        private static List<string> s_overrideActiveSections;
        public static List<string> overrideActiveSections
        {
            get
            {
                List<string> list = s_overrideActiveSections;
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
                string list = s_overrideCurrentLevelId;
                s_overrideCurrentLevelId = null;
                return list;
            }
            set
            {
                s_overrideCurrentLevelId = value;
            }
        }

        public static Dictionary<string, string> currentLevelMetaData { get; set; }

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
            if (GameModeManager.IsStoryChapter4() || GameModeManager.IsStoryChapter5())
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
            else
            {
                sections.AddRange(LevelSectionManager.Instance._showingSections.ToList());
            }

            string content = ModJsonUtils.Serialize(new ModLevelSectionInfo
            {
                LevelID = LevelManager.Instance.GetCurrentLevelID(),
                EnabledSections = sections,
                DisplayName = displayName,
                Order = order,
                ChapterIndex = chapterIndex,
            });

            ModDataManager.Instance.WriteFile(fileName + ".json", content, true);
            _ = ModIOUtils.OpenFileExplorer(ModDataManager.Instance.savesFolder);
        }

        public static ModLevelSectionInfo[] GetChapterSections(string directory, int chapterIndex)
        {
            if (chapterIndex == 1)
                return ModLevelManager.Instance.GenerateChapterSections(false);
            if (chapterIndex == 2)
                return ModLevelManager.Instance.GenerateChapterSections(true);

            directory += chapterIndex;
            if (ModAdvancedCache.Has(directory))
                return ModAdvancedCache.Get<ModLevelSectionInfo[]>(directory);

            if (!Directory.Exists(directory))
                return Array.Empty<ModLevelSectionInfo>();

            string[] files = Directory.GetFiles(directory, "*.json");
            if (files == null || files.Length == 0)
                return Array.Empty<ModLevelSectionInfo>();

            int index = 0;
            ModLevelSectionInfo[] sections = new ModLevelSectionInfo[files.Length];
            foreach (string file in files)
            {
                ModLevelSectionInfo chapterSectionInfo = null;
                try
                {
                    chapterSectionInfo = ModJsonUtils.DeserializeStream<ModLevelSectionInfo>(file);
                }
                catch
                {
                    chapterSectionInfo = new ModLevelSectionInfo() { DeserializationError = true };
                }
                sections[index] = chapterSectionInfo;
                index++;
            }
            sections = sections.ToList().OrderBy(t => t.Order).ToArray();
            ModAdvancedCache.Add(directory, sections);
            return sections;
        }

        public static string GetSpeakerNameText(SpeakerNames speakerName)
        {
            return (speakerName.ToString() + ":").AddColor(Color.white);
        }
    }
}
