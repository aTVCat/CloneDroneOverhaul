using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorFileImportPanel : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(OnHelpButtonClicked))]
        [UIElement("HelpButton")]
        private readonly Button m_helpButton;

        [UIElementAction(nameof(OnImportVoxButtonClicked))]
        [UIElement("ImportVoxButton")]
        private readonly Button m_importVoxButton;

        [UIElement("FileDisplayPrefab", false)]
        private readonly ModdedObject m_fileDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_fileDisplayContainer;

        public void OnHelpButtonClicked()
        {
            ModUIConstants.ShowPersonalizationEditorHelpMenu(UIPersonalizationEditor.instance.transform);
        }

        public void OnImportVoxButtonClicked()
        {

        }
    }
}
