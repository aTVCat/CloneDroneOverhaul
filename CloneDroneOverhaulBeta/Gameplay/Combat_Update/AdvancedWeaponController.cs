using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat_Update
{
    //TODO: rename it & optimize code
    public class AdvancedWeaponController : FirstPersonMoverExtention
    {
        private static readonly object[] _argument = new object[] { -1 };

        public bool IsWeaponDamageActive => Owner.IsWeaponDamageActive();
        public bool IsGoingToSwingWeapon => Owner.CallPrivateMethod<bool>("isWaitingForMeleeStrikeToFinish", _argument);
        public WeaponType EquipedWeapon => Owner.GetEquippedWeaponType();
        public WeaponModel EquipedWeaponModel => Owner.GetEquippedWeaponModel();
        public CharacterModel OwnerModel => Owner.GetCharacterModel();

        /// <summary>
        /// Get the strength we are swinging the weapon
        /// </summary>
        public static float AttackStrength { get; private set; }

        private float _attackSpeed;
        public float AttackSpeed => EquipedWeapon == WeaponType.Hammer && !IsWeaponDamageActive && IsGoingToSwingWeapon ? _attackSpeed : GameModeManager.IsNonCoopMultiplayer() ? 0.8f : Owner.AttackSpeed;

        private float _velocityMultipler;
        private float _defaultSpeed;
        private float _speedMultipler;
        public float MovementSpeed => EquipedWeapon == WeaponType.Hammer ? _defaultSpeed * _speedMultipler : _defaultSpeed;

        private bool _weaponDamagePrevState;

        protected override void Initialize(FirstPersonMover owner)
        {
            _attackSpeed = 1f;
            _speedMultipler = 1f;
            _velocityMultipler = 0f;
            _weaponDamagePrevState = false;
            _defaultSpeed = Mathf.Clamp(owner.MaxSpeed, 7f, float.PositiveInfinity);

            if (!GameModeManager.IsNonCoopMultiplayer())
            {
                owner.AttackSpeed = 1f;
            }

            UpgradeCollection collection = base.GetComponent<UpgradeCollection>();
            OnUpgradesRefreshed(collection);
        }

        public override void OnUpgradesRefreshed(UpgradeCollection upgrades)
        {
            if (upgrades == null)
            {
                return;
            }

            int level = upgrades.GetUpgradeLevel(UpgradeType.Hammer);
            switch (level)
            {
                case 2:
                    _attackSpeed = 0.85f;
                    _speedMultipler = 0.9f;
                    _velocityMultipler = 9f;
                    break;
                case 3:
                    _attackSpeed = 0.67f;
                    _speedMultipler = 0.75f;
                    _velocityMultipler = 12f;
                    break;
                default:
                    _attackSpeed = 1f;
                    _speedMultipler = 1f;
                    _velocityMultipler = 4f;
                    break;
            }
        }

        public void SetAttackStrength(in float sensivity)
        {
            if (sensivity == 0f)
            {
                AttackStrength = sensivity;
            }
            else
            {
                AttackStrength = Mathf.Clamp(Mathf.Abs(sensivity * 0.65f), 0f, 16f);
            }
            SeveredVolumeGenerator.Instance.SeveredPartVelocity = 5.5f + AttackStrength;
        }

        public void SetDefaultSpeed(in float value)
        {
            _defaultSpeed = Mathf.Clamp(value, 7f, float.PositiveInfinity);
        }

        private void LateUpdate()
        {
            if (!OverhaulCombatController.AllowNewCombat || !Owner.IsMainPlayer())
            {
                return;
            }

            if (Time.frameCount % 2 == 0)
            {
                if (IsWeaponDamageActive || IsGoingToSwingWeapon)
                {
                    SetAttackStrength(Owner.GetPrivateField<float>("_horizontalCursorMovement"));
                }
                else
                {
                    SetAttackStrength(0f);
                }
            }
        }

        private void Update()
        {
            if (!OverhaulCombatController.AllowNewCombat)
            {
                return;
            }

            if (Time.frameCount % 2 == 0)
            {
                if (OwnerModel != null && OwnerModel.UpperAnimator != null)
                {
                    if (_weaponDamagePrevState != IsWeaponDamageActive && IsWeaponDamageActive && EquipedWeapon == WeaponType.Hammer && !Owner.IsJumping())
                    {
                        Owner.AddVelocity(OwnerModel.transform.forward * _velocityMultipler);
                    }
                    _weaponDamagePrevState = IsWeaponDamageActive;

                    OwnerModel.UpperAnimator.speed = AttackSpeed;
                }
                Owner.MaxSpeed = MovementSpeed;
            }
        }
    }
}