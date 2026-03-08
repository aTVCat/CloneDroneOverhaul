using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIPersonalizationSettingsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _exitButton;

        [UIElementAction(nameof(OnAllowEnemiesUseWeaponSkinsToggled))]
        [UIElement("EnemiesUseSkinsToggle")]
        private readonly Toggle _allowEnemiesUseWeaponSkinsToggle;

        private bool _disallowCallbacks;

        protected override void OnInitialized()
        {
            _disallowCallbacks = true;
            _allowEnemiesUseWeaponSkinsToggle.isOn = PersonalizationUserInfo.AllowEnemiesUseSkins;
            _disallowCallbacks = false;
        }

        public override void OnDisable()
        {
            ModSettingsDataManager.Instance.Save();
        }

        public void OnAllowEnemiesUseWeaponSkinsToggled(bool value)
        {
            if (_disallowCallbacks)
                return;

            ModSettingsManager.SetBoolValue(ModSettingsConstants.ALLOW_ENEMIES_USE_WEAPON_SKINS, value, true);
        }
    }
}
