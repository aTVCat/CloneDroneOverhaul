using OverhaulMod.Engine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class DropdownWeaponVariantOptionData : Dropdown.OptionData
    {
        public WeaponVariant Value;

        public DropdownWeaponVariantOptionData(WeaponVariant value)
        {
            switch (value)
            {
                case WeaponVariant.Normal:
                    text = "Normal";
                    break;
                case WeaponVariant.OnFire:
                    text = "Fire";
                    break;
                case WeaponVariant.NormalMultiplayer:
                    text = "Normal (Multiplayer)";
                    break;
                case WeaponVariant.OnFireMultiplayer:
                    text = "Fire (Multiplayer)";
                    break;
                default:
                    text = value.ToString();
                    break;
            }
            Value = value;
        }
    }
}
