using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Combat
{
    public class CharacterExtension : MonoBehaviour
    {
        [ModSetting(ModSettingsConstants.ENABLE_SCROLL_TO_SWITCH_WEAPON, true)]
        public static bool EnableScrollToSwitchWeapon;

        [ModSetting(ModSettingsConstants.WEAPON_SWITCH_COOLDOWN, 0.1f)]
        public static float WeaponSwitchCooldown;

        private float _weaponSwitchCooldown;

        private bool _hasNotSwitchedWeaponWithScrolling;

        public int LastServerFrameDoubleJumped;

        public bool HasDoubleJumpAbility;

        private PlayerInputController _inputController;

        private FirstPersonMover _owner;
        public FirstPersonMover owner
        {
            get
            {
                if (!_owner)
                {
                    _owner = base.GetComponent<FirstPersonMover>();
                }
                return _owner;
            }
        }

        private void Start()
        {
            OnUpgradesRefreshed(owner._upgradeCollection);
            _inputController = owner._playerInputController;
        }

        private void Update()
        {
            _weaponSwitchCooldown = Mathf.Max(0f, _weaponSwitchCooldown - Time.deltaTime);
            if (!EnableScrollToSwitchWeapon || !allowSwitchingWeapons() || _weaponSwitchCooldown > 0f || _hasNotSwitchedWeaponWithScrolling)
                return;

            FirstPersonMover firstPersonMover = owner;
            if (firstPersonMover && firstPersonMover.IsMainPlayer() && !firstPersonMover.IsAimingBow())
            {
                float scroll = Input.mouseScrollDelta.y;
                if (scroll > 0.1f)
                {
                    selectNextWeapon(firstPersonMover);
                }
                else if (scroll < -0.1f)
                {
                    selectPreviousWeapon(firstPersonMover);
                }
            }
        }

        private bool allowSwitchingWeapons()
        {
            return _inputController && _inputController.enabled && !InputManager.Instance.IsCursorEnabled();
        }

        public void OnUpgradesRefreshed(UpgradeCollection upgrades)
        {
            HasDoubleJumpAbility = upgrades.HasUpgrade(ModUpgradesManager.DOUBLE_JUMP_UPGRADE);
        }

        private void selectNextWeapon(FirstPersonMover firstPersonMover)
        {
            _weaponSwitchCooldown = WeaponSwitchCooldown;
            _hasNotSwitchedWeaponWithScrolling = true;

            ModGameUtils.WaitForPlayerInputUpdate(delegate (IFPMoveCommandInput commandInput)
            {
                _hasNotSwitchedWeaponWithScrolling = false;
                commandInput.NextWeapon = true;
            });
        }

        private void selectPreviousWeapon(FirstPersonMover firstPersonMover)
        {
            _weaponSwitchCooldown = WeaponSwitchCooldown;
            _hasNotSwitchedWeaponWithScrolling = true;

            ModGameUtils.WaitForPlayerInputUpdate(delegate (IFPMoveCommandInput commandInput)
            {
                _hasNotSwitchedWeaponWithScrolling = false;

                List<WeaponType> list = new List<WeaponType>(firstPersonMover._equippedWeapons);
                list.Remove(WeaponType.Shield);
                if (list.Count == 0)
                    return;

                int num = list.IndexOf(firstPersonMover._currentWeapon);
                if (num == -1)
                    return;

                int index = num - 1;
                if (index <= -1)
                    index = list.Count - 1;

                if (list.Count == 1 && firstPersonMover._droppedWeapons.Count > 0)
                {
                    FirstPersonMover.dispatchAttemptedChangeToDroppedWeapon(firstPersonMover._droppedWeapons[0]);
                }

                WeaponType weaponType = list[index];
                if (GameModeManager.IsMultiplayer())
                {
                    if (weaponType == WeaponType.Sword)
                    {
                        commandInput.Weapon1 = true;
                    }
                    else if (weaponType == WeaponType.Bow)
                    {
                        commandInput.Weapon2 = true;
                    }
                    else if (weaponType == WeaponType.Hammer)
                    {
                        commandInput.Weapon3 = true;
                    }
                    else if (weaponType == WeaponType.Spear)
                    {
                        commandInput.Weapon4 = true;
                    }
                }
                else
                {
                    firstPersonMover.SetEquippedWeaponType(weaponType);
                }
            });
        }
    }
}
