using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Gameplay
{
    public class CharacterExtension : MonoBehaviour
    {
        [ModSetting(ModSettingIDs.ENABLE_SCROLL_TO_SWITCH_WEAPON, true)]
        public static bool EnableScrollToSwitchWeapon;

        [ModSetting(ModSettingIDs.WEAPON_SWITCH_COOLDOWN, 0.1f)]
        public static float WeaponSwitchCooldown;

        private float _weaponSwitchCooldown;

        private bool _hasNotSwitchedWeaponWithScrolling;

        private float _timeToAllowDoubleJump;

        private float _doubleJumpTime;

        private GameObject _doubleJumpTrail1, _doubleJumpTrail2;

        private List<ParticleSystem> _doubleJumpParticles;

        private bool _hasDoubleJumpVisuals;

        public int MaxJumps;

        public int LastServerFrameDoubleJumped;

        public int DoubleJumpCount;

        private PlayerInputController _inputController;

        private FirstPersonMover _owner;

        private void Start()
        {
            OnUpgradesRefreshed(_owner._upgradeCollection);
        }

        private void Update()
        {
            FirstPersonMover firstPersonMover = _owner;
            if (!firstPersonMover) return;

            if (HasDoubleJumpAbility())
            {
                if (DoubleJumpCount != 0 && firstPersonMover.IsOnGroundServer()) DoubleJumpCount = 0;

                if (_hasDoubleJumpVisuals)
                {
                    bool shouldShowTrails = DoubleJumpCount != 0 && TimeManager.GetBoltServerTime() < _doubleJumpTime + 1f;
                    foreach (ParticleSystem trail in _doubleJumpParticles)
                        trail.SetEmissionEnabled(shouldShowTrails);
                }
            }

            _weaponSwitchCooldown = Mathf.Max(0f, _weaponSwitchCooldown - Time.deltaTime);
            if (!EnableScrollToSwitchWeapon || !allowSwitchingWeapons() || _weaponSwitchCooldown > 0f || _hasNotSwitchedWeaponWithScrolling)
                return;

            if (firstPersonMover.IsMainPlayer() && !firstPersonMover.IsAimingBow())
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

        public void Initialize(FirstPersonMover firstPersonMover)
        {
            _owner = firstPersonMover;
            _inputController = firstPersonMover._playerInputController;

            Transform footRTransform = firstPersonMover.GetBodyPartParent("FootR");
            Transform footLTransform = firstPersonMover.GetBodyPartParent("FootL");
            if (footLTransform && footRTransform)
            {
                bool hasDoubleJumpUpgrade = firstPersonMover.HasUpgrade(ModUpgradesManager.DOUBLE_JUMP_UPGRADE);

                _doubleJumpParticles = new List<ParticleSystem>();

                _doubleJumpTrail1 = Instantiate(ModResources.Prefab(ModAssetBundles.VFX, "VFX_DoubleJumpTrail"), footLTransform, false);
                _doubleJumpTrail1.SetActive(hasDoubleJumpUpgrade);
                _doubleJumpParticles.AddRange(_doubleJumpTrail1.GetComponentsInChildren<ParticleSystem>(true));
                _doubleJumpTrail2 = Instantiate(ModResources.Prefab(ModAssetBundles.VFX, "VFX_DoubleJumpTrail"), footRTransform, false);
                _doubleJumpTrail2.SetActive(hasDoubleJumpUpgrade);
                _doubleJumpParticles.AddRange(_doubleJumpTrail2.GetComponentsInChildren<ParticleSystem>(true));

                _hasDoubleJumpVisuals = true;
            }
        }

        public void OnUpgradesRefreshed(UpgradeCollection upgrades)
        {
            bool hasDoubleJumpUpgrade = upgrades.HasUpgrade(ModUpgradesManager.DOUBLE_JUMP_UPGRADE);
            MaxJumps = hasDoubleJumpUpgrade ? (1 + upgrades.GetUpgradeLevel(ModUpgradesManager.DOUBLE_JUMP_UPGRADE)) : 1;

            if (_hasDoubleJumpVisuals)
            {
                _doubleJumpTrail1.gameObject.SetActive(hasDoubleJumpUpgrade);
                _doubleJumpTrail2.gameObject.SetActive(hasDoubleJumpUpgrade);
            }
        }

        public bool HasDoubleJumpAbility() => MaxJumps > 1;

        public bool CanPerformDoubleJump() => HasDoubleJumpAbility() && DoubleJumpCount < MaxJumps - 1 && TimeManager.GetBoltServerTime() >= _timeToAllowDoubleJump;

        public void OnPerformedDoubleJump()
        {
            DoubleJumpCount++;
            _doubleJumpTime = TimeManager.GetBoltServerTime();
            _timeToAllowDoubleJump = _doubleJumpTime + 1f;
        }

        private bool allowSwitchingWeapons()
        {
            return _inputController && _inputController.enabled && !InputManager.Instance.IsCursorEnabled();
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
