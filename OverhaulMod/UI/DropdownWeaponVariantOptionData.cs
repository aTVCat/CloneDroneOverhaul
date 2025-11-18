using OverhaulMod.Engine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class DropdownWeaponVariantOptionData : Dropdown.OptionData
    {
        public WeaponVariant2 Value;

        public DropdownWeaponVariantOptionData(WeaponVariant2 value)
        {
            text = WeaponVariantManager.GetWeaponVariantString(value);
            Value = value;
        }
    }
}
