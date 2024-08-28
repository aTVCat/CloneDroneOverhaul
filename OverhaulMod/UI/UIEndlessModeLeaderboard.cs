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

        [UIElement("LeaderboardEntryDisplayPrefab", false)]
        private readonly ModdedObject m_leaderboardEntry;
        [UIElement("Content")]
        private readonly Transform m_content;

        [UIElement("NoRecordsIndicator")]
        private readonly GameObject m_noRecordsIndicatorObject;

        public override bool refreshOnlyCursor => true;

        public List<HighScoreData> displayingList
        {
            get;
            private set;
        }

        public string fileName
        {
            get;
            private set;
        }

        public void Populate(List<HighScoreData> list, string file)
        {
            displayingList = list;
            fileName = file;
            clearList();

            bool nullOrEmpty = list.IsNullOrEmpty();
            m_exportButton.interactable = !nullOrEmpty;
            m_clearButton.interactable = !nullOrEmpty;
            m_noRecordsIndicatorObject.SetActive(nullOrEmpty);

            if (nullOrEmpty)
                return;

            int position = 1;
            foreach (HighScoreData data in list)
            {
                ModdedObject moddedObject = Instantiate(m_leaderboardEntry, m_content);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = data.HumanFacts.GetFullName();
                moddedObject.GetObject<Text>(1).text = position.ToString() + ".";
                moddedObject.GetObject<Text>(2).text = data.LevelReached.ToString();
                moddedObject.GetObject<Text>(2).color = EndlessModeManager.Instance.GetNextLevelDifficultyTierDescription(data.LevelReached - 1).TextColor;
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
            List<HighScoreData> list = displayingList;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (HighScoreData data in list)
            {
                _ = stringBuilder.Append(string.Format("{0}. {1} - {2}\r\n", new object[] { position, data.HumanFacts.GetFullName(), data.LevelReached }));
                position++;
            }

            string file = ModCore.savesFolder + "LeaderboardExport.txt";
            ModFileUtils.WriteText(stringBuilder.ToString(), file);
            _ = ModFileUtils.OpenFile(file);
        }

        public void OnSavesFolderButtonClicked()
        {
            _ = ModFileUtils.OpenFileExplorer(ModCore.savesFolder);
        }

        public void OnClearButtonClicked()
        {
            ModUIUtils.MessagePopup(true, "Confirm clearing leaderboard data?", LocalizationManager.Instance.GetTranslatedString("action_cannot_be_undone"), 125f, MessageMenu.ButtonLayout.EnableDisableButtons, string.Empty, "Yes", "No", null, delegate
            {
                List<HighScoreData> list = displayingList;
                list.Clear();
                ModJsonUtils.WriteStream(DataRepository.Instance.GetFullPath(fileName), list);

                Populate(list, fileName);
            });
        }
    }
}
