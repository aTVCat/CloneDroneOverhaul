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
        [ActionReference(nameof(onLoadButtonClicked))]
        [ObjectReference("LoadButton")]
        private Button m_LoadButton;

        private void onLoadButtonClicked()
        {

        }
    }
}
