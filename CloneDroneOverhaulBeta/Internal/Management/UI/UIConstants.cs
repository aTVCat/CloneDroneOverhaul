using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.HUD
{
    public static class UIConstants
    {
        /// <summary>
        /// <see cref="UITestElements"/>
        /// </summary>
        public const string UI_TEST = "UI_TestElements";
        public static void ShowUITest()
        {
            OverhaulUIManager.reference.Show<UITestElements>(UI_TEST);
        }

        /// <summary>
        /// <see cref="UIOverhaulVersionLabel"/>
        /// </summary>
        public const string UI_VERSION_LABEL = "UI_VersionLabel";
        public static void ShowVersionLabel()
        {
            OverhaulUIManager.reference.Show<UIOverhaulVersionLabel>(UI_VERSION_LABEL);
        }

        /// <summary>
        /// <see cref="UISettingsMenu"/>
        /// </summary>
        public const string UI_SETTINGS_MENU = "UI_SettingsMenu";
        public static void ShowSettingsMenu()
        {
            OverhaulUIManager.reference.Show<UISettingsMenu>(UI_SETTINGS_MENU);
        }

        public static class Arguments
        {
            public const string DONT_UPDATE_EFFECTS = "dontUpdateEffects";
        }
    }
}
