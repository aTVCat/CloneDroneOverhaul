using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIMultiplayerGameModeSelectScreen : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("Content")]
        private readonly Transform m_content;

        [UIElement("Box")]
        private readonly GameObject m_box;

        [UIElement("ScrollRect")]
        private readonly ScrollRect m_scrollRect;

        public override bool hideTitleScreen => true;

        public override bool forceCancelHide
        {
            get
            {
                return !m_box.activeSelf;
            }
        }

        protected override void OnInitialized()
        {
            ModActionUtils.DoNextFrame(delegate
            {
                m_scrollRect.verticalNormalizedPosition = 0.5f;
            });
        }

        public override void Show()
        {
            base.Show();

            ModCache.titleScreenUI.MultiplayerModeSelectScreen.Show();
            ModCache.titleScreenUI.MultiplayerModeSelectScreen.SetMainScreenVisible(true);

            Populate();
        }

        public override void Hide()
        {
            base.Hide();

            ModCache.titleScreenUI.MultiplayerModeSelectScreen.Hide();
        }

        public void Populate()
        {
            if (m_content.childCount != 0)
                TransformUtils.DestroyAllChildren(m_content);

            GameModeCardData[] array = ModCache.titleScreenUI.MultiplayerModeSelectScreen.GameModeData;
            for (int i = 0; i < array.Length; i++)
            {
                GameModeCardButton gameModeCardButton = Instantiate(ModCache.titleScreenUI.MultiplayerModeSelectScreen.ButtonPrefab, m_content);
                gameModeCardButton.transform.localScale = Vector3.one;
                gameModeCardButton.Initialize(array[i]);
            }
        }

        public void SetMainScreenVisible(bool value)
        {
            m_box.SetActive(value);

            if (value)
                Populate();
        }
    }
}
