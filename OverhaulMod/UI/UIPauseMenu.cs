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
    public class UIPauseMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("ResumeButton")]
        private readonly Button m_resumeButton;

        [UIElementAction(nameof(OnLegacyUIButtonClicked))]
        [UIElement("LegacyUIButton")]
        private readonly Button m_legacyUIButton;

        public override bool enableCursorIfVisible
        {
            get
            {
                return true;
            }
        }

        private static bool s_disableOverhauledVersion;
        public static bool disableOverhauledVersion
        {
            get
            {
                return s_disableOverhauledVersion;
            }
            set
            {
                s_disableOverhauledVersion = value;
            }
        }

        public override void Show()
        {
            base.Show();
            TimeManager.Instance.OnGamePaused();
        }

        public override void Hide()
        {
            base.Hide();
            TimeManager.Instance.OnGameUnPaused();
        }

        public void OnLegacyUIButtonClicked()
        {
            Hide();
            ModUIUtility.ShowVanillaEscMenu();
        }
    }
}
