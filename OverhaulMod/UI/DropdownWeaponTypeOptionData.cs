using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class DropdownWeaponTypeOptionData : Dropdown.OptionData
    {
        public WeaponType Weapon;

        public DropdownWeaponTypeOptionData(WeaponType weaponType)
        {
            text = weaponType.ToString();
            Weapon = weaponType;
        }
    }
}
