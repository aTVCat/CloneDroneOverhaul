using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIDebugMenu : OverhaulUIBehaviour
    {
        public override bool enableUIOverLogoMode => true;

        public override bool enableCursor => true;

        [UIElementAction(nameof(OnUpdateBuildCompilationInfoButtonClicked))]
        [UIElement("UpdateBuildInfoButton")]
        private readonly Button m_buildCompilationInfoButton;

        [UIElementAction(nameof(OnSkipToNightmariumButtonClicked))]
        [UIElement("SkipToNightmariumButton")]
        private readonly Button m_skipToNightmariumButton;

        [UIElementAction(nameof(OnSkillPointsButtonClicked))]
        [UIElement("MoreSkillPointsButton")]
        private readonly Button m_skillPointsButton;

        [UIElementAction(nameof(OnExportAllUpgradesButtonClicked))]
        [UIElement("ExportAllUpgradesButton")]
        private readonly Button m_exportAllUpgradesButton;

        [UIElementAction(nameof(OnWipLabelToggleChanged))]
        [UIElement("ShowWipLabelToggle")]
        private readonly Toggle m_wipLabelToggle;

        protected override void OnInitialized()
        {
            m_wipLabelToggle.isOn = ModSettingsManager.GetBoolValue(ModSettingsConstants.SHOW_DEVELOPER_BUILD_LABEL);
        }

        public override void Hide()
        {
            base.Hide();
            ModSettingsDataManager.Instance.Save();
        }

        public void OnSkillPointsButtonClicked()
        {
            GameDataManager.Instance.SetAvailableSkillPoints(1000);
        }

        public void OnSkipToNightmariumButtonClicked()
        {
            if (!GameModeManager.Is(GameMode.Endless))
            {
                ModUIUtils.MessagePopupOK("This button only works in endless mode.", "It really does");
                return;
            }

            int levelsToBeat = 46 - GameDataManager.Instance.GetNumberOfLevelsWon();
            if (levelsToBeat < 1)
                return;

            GameData currentGameData = GameDataManager.Instance._endlessModeData;
            for (int i = 0; i < levelsToBeat; i++)
            {
                LevelManager.Instance.PickNextLevel();
                currentGameData.LevelIDsBeatenThisPlaythrough.Add(currentGameData.CurentLevelID);
                currentGameData.LevelPrefabsBeatenThisPlaythrough.Add(LevelManager.Instance.GetCurrentLevelDescription().PrefabName);
            }
            DebugManager.Instance.KillAllEnemies();
            currentGameData.SetDirty(true);
        }

        public void OnUpdateBuildCompilationInfoButtonClicked()
        {
            ModBuildInfo.GenerateExtraInfo();
            ModUIUtils.MessagePopupOK("Successfully saved compilation date.", "Successfully saved compilation date.");

            UIVersionLabel versionLabel = UIVersionLabel.instance;
            if (versionLabel)
            {
                versionLabel.RefreshLabels();
            }
        }

        public void OnExportAllUpgradesButtonClicked()
        {
            System.Collections.Generic.List<UpgradeDescription> list = UpgradeManager.Instance.UpgradeDescriptions;
            UpgradeDescription lastUd = list.Last();

            StringBuilder stringBuilder = new StringBuilder();
            foreach (UpgradeDescription ud in list)
            {
                _ = stringBuilder.Append(ud.UpgradeName);
                _ = stringBuilder.Append(" (");
                _ = stringBuilder.Append(ud.UpgradeType);
                _ = stringBuilder.Append(", ");
                _ = stringBuilder.Append(ud.Level);
                _ = stringBuilder.Append(")");
                if (ud != lastUd)
                    _ = stringBuilder.Append("\r\n");
            }

            ModDataManager.Instance.WriteFile("AllUpgradesExport.txt", stringBuilder.ToString(), true);
            _ = ModFileUtils.OpenFileExplorer(ModCore.savesFolder);
        }

        public void OnWipLabelToggleChanged(bool value)
        {
            ModSettingsManager.SetBoolValue(ModSettingsConstants.SHOW_DEVELOPER_BUILD_LABEL, value);
        }
    }
}
