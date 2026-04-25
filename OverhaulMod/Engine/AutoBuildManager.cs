using OverhaulMod.UI;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class AutoBuildManager : Singleton<AutoBuildManager>
    {
        [ModSetting(ModSettingsConstants.AUTO_BUILD_KEY_BIND, KeyCode.U)]
        public static KeyCode AutoBuildKeyBind;

        /*[ModSetting(ModSettingsConstants.AUTO_BUILD_ACTIVATION_ON_MATCH_START, false)]
        public static bool AutoBuildActivationOnMatchStart;*/

        [ModSetting(ModSettingsConstants.AUTO_BUILD_INDEX_TO_USE_ON_MATCH_START, -1)]
        public static int AutoBuildIndexToUseOnMatchStart;

        private bool _hasSelectedUpgradesForMatch;

        private float _timeLeftBeforeAutoActivationReset;

        private UIAutoBuildSelectionMenu _autoBuildSelectionMenu;
        public UIAutoBuildSelectionMenu autoBuildSelectionMenu
        {
            get
            {
                if (!_autoBuildSelectionMenu)
                {
                    _autoBuildSelectionMenu = ModUIConstants.ShowAutoBuildSelectionMenu();
                }
                return _autoBuildSelectionMenu;
            }
        }

        public bool isInAutoBuildConfigurationMode
        {
            get;
            set;
        }

        public AutoBuildListInfo buildList
        {
            get;
            set;
        }

        private bool _isApplyingBuild;

        private void Start()
        {
            LoadBuildList();
        }

        private void Update()
        {
            if (!GameModeManager.Is(GameMode.BattleRoyale) && !GameModeManager.Is(GameMode.MultiplayerDuel))
                return;

            if (Input.GetKeyDown(AutoBuildKeyBind))
            {
                UpgradeUI upgradeUI = ModCache.UIRoot.UpgradeUI;
                if (upgradeUI.gameObject.activeSelf)
                {
                    autoBuildSelectionMenu.Show();
                }
            }
            else if (Input.GetKeyUp(AutoBuildKeyBind))
            {
                autoBuildSelectionMenu.Hide();
            }

            if (_hasSelectedUpgradesForMatch)
            {
                _timeLeftBeforeAutoActivationReset -= Time.deltaTime;
                if (_timeLeftBeforeAutoActivationReset <= 0f)
                    _hasSelectedUpgradesForMatch = false;
                else
                    return;
            }

            if (AutoBuildIndexToUseOnMatchStart <= -1)
                return;

            BattleRoyaleManager battleRoyaleManager = BattleRoyaleManager.Instance;
            if (battleRoyaleManager)
            {
                UpgradeUI upgradeUI = ModCache.UIRoot.UpgradeUI;
                if (!upgradeUI || !upgradeUI.gameObject.activeInHierarchy)
                    return;

                int secondsLeft = battleRoyaleManager.GetSecondsToGameStart();
                if (!_hasSelectedUpgradesForMatch && secondsLeft > 7 && secondsLeft < 10)
                {
                    _hasSelectedUpgradesForMatch = true;
                    _timeLeftBeforeAutoActivationReset = 15f;
                    ApplyBuild(AutoBuildIndexToUseOnMatchStart);
                }
            }
        }

        public void LoadBuildList()
        {
            string oldPath = Path.Combine(ModDirectories.ModUserDataFolder, "AutoBuildInfo.json");
            AutoBuildInfo oldAutoBuildInfo;
            if (File.Exists(oldPath))
            {
                try
                {
                    oldAutoBuildInfo = ModDataManager.DeserializeFile<AutoBuildInfo>("AutoBuildInfo.json", false);
                    oldAutoBuildInfo.FixValues();

                    File.Delete(oldPath);
                }
                catch (Exception)
                {
                    oldAutoBuildInfo = null;
                }
            }
            else
            {
                oldAutoBuildInfo = null;
            }

            AutoBuildListInfo autoBuildListInfo;
            try
            {
                autoBuildListInfo = ModDataManager.DeserializeFile<AutoBuildListInfo>("AutoBuilds.json", false);
                autoBuildListInfo.FixValues();
            }
            catch (Exception)
            {
                autoBuildListInfo = new AutoBuildListInfo();
                autoBuildListInfo.FixValues();
            }

            if (oldAutoBuildInfo != null)
            {
                autoBuildListInfo.Builds.Add(oldAutoBuildInfo);
                SaveBuildsInfo();
            }
            buildList = autoBuildListInfo;
        }

        public void SaveBuildsInfo()
        {
            ModDataManager.SerializeToFile("AutoBuilds.json", buildList, false);
        }

        public void ResetUpgrades(Dictionary<UpgradeType, int> dictionary = null, int skillPoints = 4)
        {
            GameDataManager gameDataManager = GameDataManager.Instance;
            GameData gameData = gameDataManager._tempTitleScreenData;
            gameData.PlayerUpgrades.Clear();
            if (dictionary != null)
                foreach (KeyValuePair<UpgradeType, int> kv in dictionary)
                    gameData.PlayerUpgrades.Add(kv.Key, kv.Value);

            gameData.AvailableSkillPoints = skillPoints;

            GlobalEventManager.Instance.Dispatch("UpgradesReset");
            GlobalEventManager.Instance.Dispatch("AvailableSkillPointsChanged");
        }

        public void ApplyBuild(int index)
        {
            if (_isApplyingBuild)
                return;

            FirstPersonMover firstPersonMover = CharacterTracker.Instance?.GetPlayerRobot();
            if (!firstPersonMover)
                return;

            UpgradeUI upgradeUI = ModCache.UIRoot?.UpgradeUI;
            if (!upgradeUI || !upgradeUI.gameObject.activeInHierarchy)
                return;

            List<AutoBuildInfo> builds = buildList.Builds;
            if (builds.IsNullOrEmpty() || index < 0 || index >= builds.Count)
                return;

            AutoBuildInfo autoBuildInfo = builds[index];
            if (autoBuildInfo == null || autoBuildInfo.Upgrades.IsNullOrEmpty())
                return;

            _isApplyingBuild = true;
            _ = base.StartCoroutine(applyBuildCoroutine(autoBuildInfo));
        }

        private IEnumerator applyBuildCoroutine(AutoBuildInfo autoBuildInfo)
        {
            string playFabId = MultiplayerLoginManager.Instance.GetLocalPlayFabID();
            UpgradeUI upgradeUI = ModCache.UIRoot.UpgradeUI;
            List<UpgradeTypeAndLevel> list = autoBuildInfo.Upgrades;
            for (int i = 0; i < list.Count; i++)
            {
                UpgradeTypeAndLevel ul = list[i];
                UpgradeUIIcon icon = upgradeUI.GetUpgradeUIIcon(ul.UpgradeType, ul.UpgradeType == UpgradeType.Armor ? 0 : ul.Level);
                if (icon && icon.GetCanUpgradeRightNow(playFabId))
                {
                    icon.OnButtonClicked();

                    float timeOut = Time.unscaledTime + 2f;
                    while (ul.UpgradeType == UpgradeType.Armor ? icon.GetCanUpgradeRightNow(playFabId) : UpgradeManager.Instance.GetPlayerUpgradeLevel(ul.UpgradeType, playFabId) == 0 && Time.unscaledTime < timeOut)
                        yield return null;

                    timeOut = Time.unscaledTime + 0.1f;
                    while (Time.unscaledTime < timeOut)
                        yield return null;
                }
            }

            if (UpgradeManager.Instance.GetAvailableSkillPoints(playFabId) != 0)
            {
                upgradeUI.OnExitButtonClicked();
            }

            _isApplyingBuild = false;
            yield break;
        }

        public static string GetBuildDisplayName(string name)
        {
            return name.IsNullOrEmpty() ? "[No name]" : name;
        }
    }
}
