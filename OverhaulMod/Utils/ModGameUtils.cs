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
        private static readonly List<Action<IFPMoveCommandInput>> _playerInputUpdateActions = new List<Action<IFPMoveCommandInput>>();

        public static int GetNumOfAchievements()
        {
            GameplayAchievementManager gameplayAchievementManager = GameplayAchievementManager.Instance;
            GameplayAchievement[] achievements = gameplayAchievementManager.Achievements;

            int result = achievements.Length;
            for (int i = 0; i < achievements.Length; i++)
                if ((!ExperimentalBranchManager.Instance.ShowChapter5 && achievements[i].IsHiddenUntilChapter5Released) || (GameVersionManager.IsConsoleBuild() && !achievements[i].IsAvailableOnConsoles))
                    result--;

            return result;
        }

        public static int GetNumOfAchievementsCompleted()
        {
            GameplayAchievementManager gameplayAchievementManager = GameplayAchievementManager.Instance;
            GameplayAchievement[] achievements = gameplayAchievementManager.Achievements;

            int num = 0;
            for (int i = 0; i < achievements.Length; i++)
                if (!((!ExperimentalBranchManager.Instance.ShowChapter5 && achievements[i].IsHiddenUntilChapter5Released) || (GameVersionManager.IsConsoleBuild() && !achievements[i].IsAvailableOnConsoles)) && achievements[i].IsComplete())
                    num++;

            return num;
        }

        public static void InvokePlayerInputUpdateAction(IFPMoveCommandInput fpmoveCommand)
        {
            List<Action<IFPMoveCommandInput>> list = _playerInputUpdateActions;
            if (list.IsNullOrEmpty()) return;

            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    list[i](fpmoveCommand);
                }
                catch { }
            }
            list.Clear();
        }

        public static bool SyncSteamAchievements() // todo: sync game with steam or steam with game
        {
            GameplayAchievementManager gameplayAchievementManager = GameplayAchievementManager.Instance;
            if (!gameplayAchievementManager)
                return false;

            bool result = false;
            foreach (GameplayAchievement achievement in gameplayAchievementManager.Achievements)
            {
                if (SteamUserStats.GetAchievement(achievement.SteamAchievementID, out bool isComplete))
                {
                    bool isAchievementWithTargetProgressOverThan1 = achievement.TargetProgress > 1;
                    int inGameProgress = gameplayAchievementManager.GetProgress(achievement.AchievementID);
                    int steamProgress = isComplete ? achievement.TargetProgress : 0;
                    if (steamProgress != inGameProgress)
                        result = true;

                    if (isComplete)
                        gameplayAchievementManager.SetAchievementProgress(achievement, steamProgress, true);
                    else
                        gameplayAchievementManager.SetAchievementProgress(achievement, isAchievementWithTargetProgressOverThan1 ? inGameProgress : 0, true);
                }
            }
            return result;
        }

        public static void SaveActiveSections(string fileName, string displayName, int order, int chapterIndex)
        {
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

            ModDataManager.WriteFile(fileName + ".json", content, true);
            _ = ModFileUtils.OpenFileExplorer(ModDirectories.SavesFolder);
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
                catch (Exception)
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
            return $"{LocalizationManager.Instance.GetTranslatedString($"enum_{speakerName}")}:";
        }

        public static List<Renderer> GetRenderersOfBodyPart(this FirstPersonMover firstPersonMover, MechBodyPartType bodyPartType)
        {
            if (!firstPersonMover || bodyPartType == MechBodyPartType.None)
            {
                return new List<Renderer>();
            }

            MechBodyPart bodyPart = firstPersonMover.GetBodyPart(bodyPartType);
            if (bodyPart == null)
            {
                return new List<Renderer>();
            }

            Renderer[] renderers = bodyPart.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return new List<Renderer>();

            List<Renderer> result = new List<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (!renderer.GetComponent<SwordHitArea>())
                {
                    result.Add(renderer);
                }
            }

            return result;
        }

        public static List<Renderer> GetRenderersOfBodyPart(this FirstPersonMover firstPersonMover, string bodyPart)
        {
            if (!firstPersonMover || bodyPart.IsNullOrEmpty())
            {
                return new List<Renderer>();
            }

            Transform bodyPartParent = firstPersonMover.GetBodyPartParent(bodyPart);
            if (bodyPartParent == null || bodyPartParent.childCount == 0)
            {
                return new List<Renderer>();
            }

            for (int i = 0; i < bodyPartParent.childCount; i++)
            {
                if (bodyPartParent.GetChild(i).name == "model")
                {
                    bodyPartParent = bodyPartParent.GetChild(i);
                    break;
                }
            }

            Renderer[] renderers = bodyPartParent.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return new List<Renderer>();

            List<Renderer> result = new List<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                if (!renderer.GetComponent<SwordHitArea>())
                {
                    result.Add(renderer);
                }
            }

            return result;
        }

        public static bool IsGamePaused()
        {
            return TimeManager.Instance.IsGamePaused() && Cursor.visible;
        }

        public static bool IsGamePausedOrCursorVisible()
        {
            return TimeManager.Instance.IsGamePaused() || Cursor.visible;
        }

        public static void WaitForPlayerInputUpdate(Action<IFPMoveCommandInput> action)
        {
            _playerInputUpdateActions.Add(action);
        }

        public static void FadeThenStopMusic(float duration)
        {
            AudioManager audioManager = AudioManager.Instance;
            if (audioManager.MusicAudioSource.mute)
                return;

            audioManager.FadeOutMusic(duration);
            ModActionUtils.DoInTime(delegate
            {
                audioManager.StopMusic();
            }, duration);
        }
    }
}
