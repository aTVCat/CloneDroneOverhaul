using UnityEngine;

namespace OverhaulMod.Engine
{
    public class WeaponInvisibilityFixer : MonoBehaviour
    {
        private FirstPersonMover m_owner;

        private void Update()
        {
            if (!ModTime.Instance.HasFixedUpdated())
                return;

            FirstPersonMover firstPersonMover = m_owner;
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

            if (state != null)
            {
                WeaponType stateEquippedWeapon = (WeaponType)state.EquippedWeaponType;
                if (firstPersonMover.GetEquippedWeaponType() != stateEquippedWeapon)
                {
                    firstPersonMover.SetEquippedWeaponType(stateEquippedWeapon, false);
                }
            }
        }

        public void Initialize(FirstPersonMover firstPersonMover)
        {
            m_owner = firstPersonMover;
        }
    }
}
