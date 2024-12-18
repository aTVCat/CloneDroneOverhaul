using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIExclusivePerkMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnEditorButtonClicked))]
        [UIElement("EditorButton")]
        private readonly Button m_editorButton;

        [UIElement("UnlockedPerkDisplay", false)]
        private readonly ModdedObject m_unlockedPerkDisplay;

        [UIElement("Content")]
        private readonly Transform m_container;

        [UIElement("NothingIndicator")]
        private readonly GameObject m_nothingIndicator;

        public override bool hideTitleScreen => true;

        public override void Show()
        {
            base.Show();
            Populate();

            string error = ExclusivePerkManager.Instance.GetError();
            if (error != null)
            {
                ModUIUtils.MessagePopup(true, "Could not get data. Retry?", error, 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "Ok", "Retry", "No", null, delegate
                {
                    LoadDataFromRepository();
                }, null);
            }

            m_editorButton.gameObject.SetActive(ModUserInfo.isDeveloper);
        }

        public void Populate()
        {
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            System.Collections.Generic.List<ExclusivePerkInfo> list = ExclusivePerkManager.Instance.GetUnlockedPerks();
            if (list.IsNullOrEmpty())
            {
                m_nothingIndicator.SetActive(true);
            }
            else
            {
                m_nothingIndicator.SetActive(false);
                foreach (ExclusivePerkInfo perkInfo in list)
                {
                    ModdedObject moddedObject = Instantiate(m_unlockedPerkDisplay, m_container);
                    moddedObject.gameObject.SetActive(true);
                    moddedObject.GetObject<Text>(0).text = perkInfo.DisplayName;
                    moddedObject.GetObject<Image>(1).sprite = perkInfo.Icon.IsNullOrEmpty() ? null : ModResources.Sprite(AssetBundleConstants.PERK_ICONS, perkInfo.Icon);
                }
            }
        }

        public void OnEditorButtonClicked()
        {
            ModUIConstants.ShowExclusivePerksEditor(base.transform);
        }

        public void LoadDataFromRepository()
        {
        }
    }
}
