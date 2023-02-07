using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat_Update
{
    /// <summary>
    /// New machanic!
    /// </summary>
    public class DropThrowNTakeWeapons : FirstPersonMoverExtention
    {
        /// <summary>
        /// Check if robot is holding any weapon
        /// </summary>
        public bool HasAnyWeaponInHand
        {
            get
            {
                WeaponModel m = Owner.GetEquippedWeaponModel();
                if (m == null || m.WeaponType == WeaponType.None)
                {
                    return false;
                }
                return true;
            }
        }

        private float _timeGettingReadyToDropWeapon;
        private bool _waitingUserToStopDroppingWeapon;

        protected override void Initialize(FirstPersonMover owner)
        {
        }

        private void Update()
        {
            if (!Owner.IsMainPlayer())
            {
                return;
            }

            bool mouseButtonDown = Input.GetMouseButton(1);
            if (!_waitingUserToStopDroppingWeapon && !Owner.IsJetpackEngaged() && !Owner.IsAimingBow() && mouseButtonDown)
            {
                _timeGettingReadyToDropWeapon += Time.deltaTime;
                if (_timeGettingReadyToDropWeapon >= 1f)
                {
                    DropCurrentWeapon();
                    _waitingUserToStopDroppingWeapon = true;
                }
            }
            if (!mouseButtonDown)
            {
                _waitingUserToStopDroppingWeapon = false;
                _timeGettingReadyToDropWeapon = 0f;
            }
        }

        public void DropCurrentWeapon()
        {
            if (!HasAnyWeaponInHand)
            {
                return;
            }
            // play animation
            WeaponModel m = Owner.GetEquippedWeaponModel();
        }

        public void EquipWeapon()
        {
        }
    }
}