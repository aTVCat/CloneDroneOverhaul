using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorLoadPanel : PersonalizationEditorUIElement
    {
        [UIElementActionReference(nameof(OnLoadButtonClicked))]
        [UIElementReferenceAttribute("LoadButton")]
        private readonly Button m_LoadButton;

        [UIElementActionReference(nameof(OnReloadButtonClicked))]
        [UIElementReferenceAttribute("Button_Reload")]
        private readonly Button m_ReloadButton;

        [UIElementReferenceAttribute("NeedsReloadIndicator")]
        private readonly GameObject m_NeedsReloadIndicator;

        public bool NeedsToReload
        {
            get => m_NeedsReloadIndicator.activeSelf;
            set => m_NeedsReloadIndicator.SetActive(value);
        }

        public override void Start()
        {
            base.Start();
            NeedsToReload = false;
        }

        public void OnLoadButtonClicked()
        {
            EditorUI.ItemsBrowser.Show();
        }

        public void OnReloadButtonClicked()
        {

        }
    }
}
