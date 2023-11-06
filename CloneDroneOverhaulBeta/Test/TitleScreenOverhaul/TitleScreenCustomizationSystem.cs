using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Analytics;

namespace CDOverhaul.Patches
{
    public class TitleScreenCustomizationSystem : OverhaulBehaviour
    {
        public const string OVERRIDE_LEVEL_ID = "OverhaulOverrideLevel";

        [OverhaulSetting(OverhaulSettingConstants.Categories.INTERNAL, OverhaulSettingConstants.Sections.TITLE_SCREEN, "Override Level ID")]
        public static string OverrideLevelPath = string.Empty;

        [OverhaulSetting(OverhaulSettingConstants.Categories.INTERNAL, OverhaulSettingConstants.Sections.TITLE_SCREEN, "Override Workshop Level ID")]
        public static string OverrideWorkshopLevelID = string.Empty;

        [OverhaulSettingDropdownParameters("Left@Center@")]
        [OverhaulSetting(OverhaulSettingConstants.Categories.INTERNAL, OverhaulSettingConstants.Sections.TITLE_SCREEN, "UI Alignment")]
        public static int UIAlignment = 0;

        public bool OverridesLevel()
        {
            return GameModeManager.Is(GameMode.None) && !string.IsNullOrEmpty(OverrideLevelPath);
        }

        public bool OverridesLevelWithWorkshop()
        {
            return GameModeManager.Is(GameMode.None) && !string.IsNullOrEmpty(OverrideWorkshopLevelID);
        }

        public LevelDescription GetOverrideLevelDescription()
        {
            LevelDescription level = null;
            foreach (LevelDescription levelDescription in LevelManager.Instance._endlessLevels)
                if (levelDescription.LevelID == OVERRIDE_LEVEL_ID)
                    level = levelDescription;

            if (level == null)
                level = new LevelDescription()
                {
                    GeneratedUniqueID = Guid.NewGuid().ToString(),
                    LevelTags = new List<LevelTags>() { LevelTags.LevelEditor },
                    LevelID = OVERRIDE_LEVEL_ID,
                };

            level.PrefabName = "&leveloverride" + OverrideLevelPath;
            return level;
        }

        public void SpawnLevel(out string error)
        {
            error = null;

            bool justOverrides = OverridesLevel();
            bool overridesWithWorkshop = OverridesLevelWithWorkshop();
            if (justOverrides && !overridesWithWorkshop)
            {
                spawnUserLevel(out error);
                return;
            }
            spawnGameLevel(out error);
        }

        private void spawnGameLevel(out string error)
        {
            error = null;

            GameDataManager dataManager = GameDataManager.Instance;
            if (!dataManager)
            {
                error = "DataManager script is missing!";
                return;
            }

            LevelManager levelManager = LevelManager.Instance;
            if (!levelManager)
            {
                error = "LevelManager script is missing!";
                return;
            }

            levelManager.CleanUpLevelThisFrame();
            if (OverridesLevelWithWorkshop())
            {
                List<LevelDescription> list = WorkshopLevelManager.Instance._endlessWorkshopLevels;
                if (!list.IsNullOrEmpty())
                {
                    dataManager.SetCurrentLevelID(list[UnityEngine.Random.Range(0, list.Count)].LevelID);
                }
                else
                {
                    levelManager.PickNextLevel();
                }
            }
            else
            {
                levelManager.PickNextLevel();
            }
            levelManager.SpawnCurrentLevelNow();
            ArenaCameraManager.Instance.ShowTitleScreenCamera();
            ArenaCameraManager.Instance.SetArenaPreviewModeActive(true);
        }

        private void spawnUserLevel(out string error)
        {
            error = null;

            GameDataManager dataManager = GameDataManager.Instance;
            if (!dataManager)
            {
                error = "DataManager script is missing!";
                return;
            }

            LevelManager levelManager = LevelManager.Instance;
            if (!levelManager)
            {
                error = "LevelManager script is missing!";
                return;
            }

            LevelDescription levelDescription = GetOverrideLevelDescription();
            if(!levelManager._endlessLevels.Contains(levelDescription))
                levelManager._endlessLevels.Add(levelDescription);

            dataManager.SetCurrentLevelID(OVERRIDE_LEVEL_ID);

            base.StartCoroutine(spawnLevelCoroutine());
        }

        private IEnumerator spawnLevelCoroutine()
        {
            LevelManager.Instance.CleanUpLevelThisFrame();
            yield return base.StartCoroutine(LevelManager.Instance.SpawnCurrentLevel(false, null, delegate
            {
                ArenaCameraManager.Instance.ShowTitleScreenCamera();
                ArenaCameraManager.Instance.SetArenaPreviewModeActive(true);
            }));
            yield break;
        }
    }
}
