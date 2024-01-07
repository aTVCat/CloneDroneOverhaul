using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementSettingsTab : UIElementTab
    {
        [UIElement("Text")]
        private Text m_text;

        protected override void OnInitialized()
        {
            m_text.text = tabId;
        }
    }
}
