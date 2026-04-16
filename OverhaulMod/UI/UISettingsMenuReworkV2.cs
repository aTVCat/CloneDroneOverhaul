using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UISettingsMenuReworkV2 : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        public override bool HideTitleScreen => true;

        protected override void OnInitialized()
        {
            SettingsMenu settingsMenu = ModCache.SettingsMenu;
            if (settingsMenu) settingsMenu.populateSettings();
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
            ModSettingsDataManager.Instance.Save();
        }
    }
}