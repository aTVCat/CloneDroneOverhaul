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
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnAllowEnemiesUseWeaponSkinsToggled))]
        [UIElement("EnemiesUseSkinsToggle")]
        private readonly Toggle m_allowEnemiesUseWeaponSkinsToggle;

        private bool m_disallowCallbacks;

        protected override void OnInitialized()
        {
            m_disallowCallbacks = true;
            m_allowEnemiesUseWeaponSkinsToggle.isOn = PersonalizationUserInfo.AllowEnemiesUseSkins;
            m_disallowCallbacks = false;
        }

        public override void OnDisable()
        {
            ModSettingsDataManager.Instance.Save();
        }

        public void OnAllowEnemiesUseWeaponSkinsToggled(bool value)
        {
            if (m_disallowCallbacks)
                return;

            ModSettingsManager.SetBoolValue(ModSettingsConstants.ALLOW_ENEMIES_USE_WEAPON_SKINS, value, true);
        }
    }
}
