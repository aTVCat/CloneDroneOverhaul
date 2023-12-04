using UnityEngine;

namespace OverhaulMod.Combat.Weapons
{
    public class ModWeaponModel : WeaponModel
    {
        public WeaponAITuning AITuning;

        public virtual float attackSpeed
        {
            get
            {
                return 1f;
            }
        }

        public virtual float disableAttacksForSeconds
        {
            get
            {
                return 1.5f;
            }
        }

        public virtual bool equipIfEnemy
        {
            get
            {
                return false;
            }
        }

        public virtual AttackDirection attackDirections
        {
            get
            {
                return AttackDirection.Left | AttackDirection.Forward;
            }
        }

        public virtual AttackDirection defaultAttackDirection
        {
            get
            {
                return AttackDirection.Forward;
            }
        }

        public virtual RuntimeAnimatorController animatorControllerOverride
        {
            get
            {
                return null;
            }
        }

        public virtual void Configure(FirstPersonMover owner)
        {
        }

        public virtual void OnRefreshWeaponAnimatorProperties(FirstPersonMover owner)
        {
        }

        public virtual void OnUpgradesRefresh(FirstPersonMover owner)
        {
        }
    }
}
