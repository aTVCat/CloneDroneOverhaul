using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class WeaponInvisibilityFixer : MonoBehaviour
    {
        public FirstPersonMover Owner;

        private void Update()
        {
            if (!ModTime.hasFixedUpdated)
                return;

            FirstPersonMover firstPersonMover = Owner;
            if (!firstPersonMover || firstPersonMover.IsMainPlayer() || firstPersonMover.IsDetached())
                return;

            IFirstPersonMoverState state;
            try
            {
                state = firstPersonMover.state as IFirstPersonMoverState;
            }
            catch
            {
                state = null;
            }

            if(state != null)
            {
                WeaponType stateEquippedWeapon = (WeaponType)state.EquippedWeaponType;
                if(firstPersonMover.GetEquippedWeaponType() != stateEquippedWeapon)
                {
                    firstPersonMover.SetEquippedWeaponType(stateEquippedWeapon, false);
                }
            }
        }
    }
}
