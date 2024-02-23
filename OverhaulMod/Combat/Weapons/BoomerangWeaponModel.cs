using OverhaulMod.Utils;
using System;
using UnityEngine;

namespace OverhaulMod.Combat.Weapons
{
    public class BoomerangWeaponModel : ModWeaponModel
    {
        public override float attackSpeed
        {
            get
            {
                return 1f;
            }
        }

        public override void OnInstantiated(FirstPersonMover owner)
        {
            base.BodyPartsToDrop = Array.Empty<MindSpaceBodyPart>();
            base.PartsToDrop = Array.Empty<Transform>();
            base.PartsToHideInsteadOfRoot = Array.Empty<GameObject>();
        }

        public override void SetWeaponDamageActive(bool value)
        {
            base.SetWeaponDamageActive(value);
        }

        public override void OnRefreshWeaponAnimatorProperties(FirstPersonMover owner)
        {
            owner._characterModel.SetLegsFloat("AttackSpeed", 1f);
            owner._characterModel.SetUpperAnimator(WeaponManager.Instance.GetDefaultUpperBodyAnimator());
        }
    }
}
