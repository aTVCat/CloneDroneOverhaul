using UnityEngine;

namespace OverhaulMod.Combat.Weapons
{
    public class ModWeaponModel : WeaponModel
    {
        public WeaponAITuning AITuning;

        public bool IsModelActive;

        public virtual float attackSpeed => 1f;

        public virtual float disableAttacksForSeconds => 1f;

        public virtual AttackDirection attackDirections => AttackDirection.Forward;

        public virtual AttackDirection defaultAttackDirection => AttackDirection.Forward;

        public virtual void Awake()
        {
            IsModelActive = true;
        }

        public virtual void OnInstantiated(FirstPersonMover owner)
        {
        }

        public virtual void OnRefreshWeaponAnimatorProperties(FirstPersonMover owner)
        {
        }

        public virtual void OnUpgradesRefresh(FirstPersonMover owner)
        {
        }

        public virtual void SetIsModelActive(bool value)
        {
            IsModelActive = value;
        }

        public virtual bool GetIsModelActive()
        {
            return IsModelActive;
        }

        public virtual GameObject GetModel()
        {
            return null;
        }
    }
}