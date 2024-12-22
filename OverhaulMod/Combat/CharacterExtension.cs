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

        private float m_weaponSwitchCooldown;

        private bool m_hasNotSwitchedWeaponWithScrolling;

        public int LastServerFrameDoubleJumped;

        public bool HasDoubleJumpAbility;

        private FirstPersonMover m_owner;
        public FirstPersonMover owner
        {
            get
            {
                if (!m_owner)
                {
                    m_owner = base.GetComponent<FirstPersonMover>();
                }
                return m_owner;
            }
        }

        private void Start()
        {
            OnUpgradesRefreshed(owner._upgradeCollection);
        }

        private void Update()
        {
            m_weaponSwitchCooldown = Mathf.Max(0f, m_weaponSwitchCooldown - Time.deltaTime);
            if (!EnableScrollToSwitchWeapon || m_weaponSwitchCooldown > 0f || m_hasNotSwitchedWeaponWithScrolling)
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

        public void OnUpgradesRefreshed(UpgradeCollection upgrades)
        {
            HasDoubleJumpAbility = upgrades.HasUpgrade(ModUpgradesManager.DOUBLE_JUMP_UPGRADE);
        }

        private void selectNextWeapon(FirstPersonMover firstPersonMover)
        {
            m_weaponSwitchCooldown = 0.1f;
            m_hasNotSwitchedWeaponWithScrolling = true;

            ModGameUtils.WaitForPlayerInputUpdate(delegate (IFPMoveCommandInput commandInput)
            {
                m_hasNotSwitchedWeaponWithScrolling = false;
                commandInput.NextWeapon = true;
            });
        }

        private void selectPreviousWeapon(FirstPersonMover firstPersonMover)
        {
            m_weaponSwitchCooldown = 0.1f;
            m_hasNotSwitchedWeaponWithScrolling = true;

            ModGameUtils.WaitForPlayerInputUpdate(delegate (IFPMoveCommandInput commandInput)
            {
                m_hasNotSwitchedWeaponWithScrolling = false;

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
