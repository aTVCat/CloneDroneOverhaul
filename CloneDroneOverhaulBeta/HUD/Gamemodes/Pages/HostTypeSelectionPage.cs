using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class HostTypeSelectionPage : FullscreenWindowPageBase
    {
        private Button m_PublicButton;
        private Button m_PrivateButton;

        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            m_PublicButton = MyModdedObject.GetObject<Button>(0);
            m_PrivateButton = MyModdedObject.GetObject<Button>(1);
            m_PrivateButton.onClick.AddListener(OnPrivateClick);
        }

        public void OnPrivateClick()
        {
            FullscreenWindow.GoToPage(2);
        }
    }
}
