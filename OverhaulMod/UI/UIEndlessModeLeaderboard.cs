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
        private readonly Button m_ExitButton;

        [UIElementAction(nameof(OnExportButtonClicked))]
        [UIElement("ExportButton")]
        private readonly Button m_ExportButton;

        [UIElementAction(nameof(OnSavesFolderButtonClicked))]
        [UIElement("SavesFolderButton")]
        private readonly Button m_SavesFolderButton;

        [UIElement("LeaderboardEntryDisplayPrefab")]
        private readonly ModdedObject m_LeaderboardEntry;
        [UIElement("Content")]
        private readonly Transform m_Content;

        public override void Show()
        {
            base.Show();

            m_LeaderboardEntry.gameObject.SetActive(false);

            Populate();
        }

        public override void Hide()
        {
            base.Hide();

            Clear();
        }

        public override void Update()
        {
            base.Update();
            if (ModActionUtils.IsGoBackKeyDown())
            {
                Hide();
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (base.gameObject.activeSelf)
                Hide();
        }

        public void Populate()
        {
            Clear();

            int position = 1;
            EndlessModeManager endlessModeManager = EndlessModeManager.Instance;
            List<HighScoreData> list = GameDataManager.Instance._endlessHighScores;
            foreach (HighScoreData data in list)
            {
                ModdedObject moddedObject = Instantiate(m_LeaderboardEntry, m_Content);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = data.HumanFacts.GetFullName();
                moddedObject.GetObject<Text>(1).text = position.ToString() + ".";
                moddedObject.GetObject<Text>(2).text = data.LevelReached.ToString();
                moddedObject.GetObject<Text>(2).color = endlessModeManager.GetNextLevelDifficultyTierDescription(data.LevelReached - 1).TextColor;
                moddedObject.GetObject<GameObject>(3).SetActive(position % 2 == 0);

                position++;
            }
        }

        public void Clear()
        {
            TransformUtils.DestroyAllChildren(m_Content);
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
            ModIOUtils.WriteText(stringBuilder.ToString(), ModCore.savesFolder + "LeaderboardExport.txt");

            _ = stringBuilder.Clear();
        }

        public void OnSavesFolderButtonClicked()
        {
            _ = ModIOUtils.OpenFileExplorer(ModCore.savesFolder);
        }
    }
}
