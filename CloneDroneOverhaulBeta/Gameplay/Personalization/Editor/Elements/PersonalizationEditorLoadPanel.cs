using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorLoadPanel : PersonalizationEditorUIElement
    {
        [ActionReference(nameof(OnLoadButtonClicked))]
        [ObjectReference("LoadButton")]
        private readonly Button m_LoadButton;

        [ActionReference(nameof(OnReloadButtonClicked))]
        [ObjectReference("Button_Reload")]
        private readonly Button m_ReloadButton;

        [ObjectReference("NeedsReloadIndicator")]
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
