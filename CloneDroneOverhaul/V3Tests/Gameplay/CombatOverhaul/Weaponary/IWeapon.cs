using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public interface IWeapon
    {
        WeaponType WeaponType();
        void OnEquip();
        void OnUnquip();
    }
}
