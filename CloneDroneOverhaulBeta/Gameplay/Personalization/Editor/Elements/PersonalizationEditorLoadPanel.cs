using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorLoadPanel : PersonalizationEditorUIElement
    {
        [ActionReference(nameof(OnLoadButtonClicked))]
        [ObjectReference("LoadButton")]
        private Button m_LoadButton;

        [ActionReference(nameof(OnReloadButtonClicked))]
        [ObjectReference("Button_Reload")]
        private Button m_ReloadButton;

        [ObjectReference("NeedsReloadIndicator")]
        private GameObject m_NeedsReloadIndicator;

        public bool NeedToReload
        {
            get => m_NeedsReloadIndicator.activeSelf;
            set => m_NeedsReloadIndicator.SetActive(value);
        }

        public override void Start()
        {
            base.Start();
            NeedToReload = false;
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
