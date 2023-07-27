using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class DifficultySelectionPage : FullscreenWindowPageBase
    {
        private static readonly Color[] s_OutlineColors = new Color[]
        {
            "#800000".ToColor(),
            "#BDB000".ToColor(),
            "#2BA100".ToColor(),
        };

        private static readonly Color[] s_ShadowColors = new Color[]
        {
            "#400000".ToColor(),
            "#6F6000".ToColor(),
            "#134D00".ToColor(),
        };

        private Button m_NextDiffButton;
        private Button m_PrevDiffButton;

        private Text m_CurDifficultyText;
        private Outline m_CurDfficultyOutline;
        private Shadow m_CurDfficultyShadow;

        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            m_CurDifficultyText = MyModdedObject.GetObject<Text>(0);
            m_PrevDiffButton = MyModdedObject.GetObject<Button>(1);
            m_PrevDiffButton.onClick.AddListener(OnPrevDifficultyClicked);
            m_NextDiffButton = MyModdedObject.GetObject<Button>(2);
            m_NextDiffButton.onClick.AddListener(OnNextDifficultyClicked);

            Shadow[] outlines = m_CurDifficultyText.GetComponents<Shadow>();
            m_CurDfficultyOutline = (Outline)outlines[0];
            m_CurDfficultyShadow = outlines[1];
        }

        public override void OnEnable()
        {
            RefreshText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                OnNextDifficultyClicked();

            if (Input.GetKeyDown(KeyCode.Q))
                OnPrevDifficultyClicked();
        }

        public void OnNextDifficultyClicked()
        {
            int currentIndex = SettingsManager.Instance.GetStoryDifficultyIndex();
            if (currentIndex > 1)
                return;

            SettingsManager.Instance.SetStoryDifficultyIndex(currentIndex + 1);
            RefreshText();
        }

        public void OnPrevDifficultyClicked()
        {
            int currentIndex = SettingsManager.Instance.GetStoryDifficultyIndex();
            if (currentIndex < 1)
                return;

            SettingsManager.Instance.SetStoryDifficultyIndex(currentIndex - 1);
            RefreshText();
        }

        public void RefreshText()
        {
            int currentDifficultyIndex = SettingsManager.Instance.GetStoryDifficultyIndex();

            Color outline = s_OutlineColors[2 - currentDifficultyIndex];
            Color shadow = s_ShadowColors[2 - currentDifficultyIndex];

            m_CurDfficultyShadow.effectColor = shadow;
            m_CurDfficultyOutline.effectColor = outline;
            m_CurDifficultyText.text = ((DifficultyLevel)currentDifficultyIndex).ToString();

            m_NextDiffButton.interactable = currentDifficultyIndex != 2;
            m_PrevDiffButton.interactable = currentDifficultyIndex != 0;
        }
    }
}
