using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD
{
    public class UITestElements : UIController
    {
        [UIElementReference("UIElement_Dropdown")]
        private UIElementDropdown m_Dropdown;

        protected override bool HideTitleScreen() => true;
        protected override bool WaitForEscapeKeyToHide() => true;

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
