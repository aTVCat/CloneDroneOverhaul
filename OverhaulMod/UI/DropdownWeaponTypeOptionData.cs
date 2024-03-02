using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
