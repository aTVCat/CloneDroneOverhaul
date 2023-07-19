using CDOverhaul.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulAchievementsMenu : OverhaulUIVer2
    {
        public static bool IsEnabled() => OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsAchievementsMenuRedesignEnabled;

        [ObjectReference("BackButton")]
        private Button m_BackButton;

        public override void Initialize()
        {
            base.Initialize();
            if (!IsEnabled())
                return;

            m_BackButton.AddOnClickListener(Hide);
            BaseFixes.ChangeButtonAction(TitleScreen.transform, "AchievementsButton", Show);
        }

        public override void Show()
        {
            base.Show();
            HideTitleScreenButtons();
        }

        public override void Hide()
        {
            base.Hide();
            ShowTitleScreenButtons();
        }
    }
}
