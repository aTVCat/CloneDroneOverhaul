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
                case WeaponVariant.IsNormal:
                    text = "Normal";
                    break;
                case WeaponVariant.IsOnFire:
                    text = "Fire";
                    break;
                case WeaponVariant.IsNormalMultiplayer:
                    text = "Normal (Multiplayer)";
                    break;
                case WeaponVariant.IsOnFireMultiplayer:
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
