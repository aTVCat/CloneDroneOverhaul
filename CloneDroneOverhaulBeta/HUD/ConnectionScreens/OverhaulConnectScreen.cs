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
    public class OverhaulConnectScreen : OverhaulUIVer2
    {
        public static bool IsEnabled() => OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsConnectionScreensRedesignEnabled;

        public override void Initialize()
        {
            base.Initialize();
            if (!IsEnabled())
                return;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
