using OverhaulMod.Utils;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIEndlessModeLeaderboard : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnExitButtonClicked))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnExportButtonClicked))]
        [UIElement("ExportButton")]
        private readonly Button m_exportButton;

        [UIElementAction(nameof(OnClearButtonClicked))]
        [UIElement("ClearButton")]
        private readonly Button m_clearButton;

        [UIElementAction(nameof(OnSavesFolderButtonClicked))]
        [UIElement("SavesFolderButton")]
        private readonly Button m_savesFolderButton;

        [UIElement("LeaderboardEntryDisplayPrefab")]
        private readonly ModdedObject m_leaderboardEntry;
        [UIElement("Content")]
        private readonly Transform m_content;

        public override void Show()
        {
            base.Show();

            m_leaderboardEntry.gameObject.SetActive(false);

            Populate();
        }

        public override void Hide()
        {
            base.Hide();
            clearList();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (base.gameObject.activeSelf)
                Hide();
        }

        public void Populate()
        {
            clearList();

            int position = 1;
            EndlessModeManager endlessModeManager = EndlessModeManager.Instance;
            List<HighScoreData> list = GameDataManager.Instance._endlessHighScores;
            foreach (HighScoreData data in list)
            {
                ModdedObject moddedObject = Instantiate(m_leaderboardEntry, m_content);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = data.HumanFacts.GetFullName();
                moddedObject.GetObject<Text>(1).text = position.ToString() + ".";
                moddedObject.GetObject<Text>(2).text = data.LevelReached.ToString();
                moddedObject.GetObject<Text>(2).color = endlessModeManager.GetNextLevelDifficultyTierDescription(data.LevelReached - 1).TextColor;
                moddedObject.GetObject<GameObject>(3).SetActive(position % 2 == 0);

                position++;
            }
        }

        private void clearList()
        {
            if (m_content.childCount != 0)
                TransformUtils.DestroyAllChildren(m_content);
        }

        public void OnExitButtonClicked()
        {
            Hide();
        }

        public void OnExportButtonClicked()
        {
            int position = 1;
            List<HighScoreData> list = GameDataManager.Instance._endlessHighScores;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (HighScoreData data in list)
            {
                _ = stringBuilder.Append(string.Format("{0}. {1} - {2}\r\n", new object[] { position, data.HumanFacts.GetFullName(), data.LevelReached }));
                position++;
            }

            string file = ModCore.savesFolder + "LeaderboardExport.txt";
            ModIOUtils.WriteText(stringBuilder.ToString(), file);
            _ = ModIOUtils.OpenFile(file);

            _ = stringBuilder.Clear();
        }

        public void OnSavesFolderButtonClicked()
        {
            _ = ModIOUtils.OpenFileExplorer(ModCore.savesFolder);
        }

        public void OnClearButtonClicked()
        {
            ModUIUtility.MessagePopup(true, "Confirm clearing leaderboard data?", "This action cannot be undone", 125f, MessageMenu.ButtonLayout.EnableDisableButtons, string.Empty, "Yes", "No", null, delegate
            {
                List<HighScoreData> list = GameDataManager.Instance._endlessHighScores;
                list.Clear();
                DataRepository.Instance.Save(list, "EndlessHighScores", false, true);

                Populate();
            });
        }
    }
}
