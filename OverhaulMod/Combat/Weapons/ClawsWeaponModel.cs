using System;
using UnityEngine;

namespace OverhaulMod.Combat.Weapons
{
    public class ClawsWeaponModel : ModWeaponModel
    {
        public override AttackDirection attackDirections
        {
            get
            {
                return AttackDirection.Left | AttackDirection.Right;
            }
        }

        public override AttackDirection defaultAttackDirection
        {
            get
            {
                return AttackDirection.Right;
            }
        }

        public override void Configure(FirstPersonMover owner)
        {
            base.MeleeImpactArea = null;
            base.BodyPartsToDrop = Array.Empty<MindSpaceBodyPart>();
            base.PartsToDrop = Array.Empty<Transform>();
            base.PartsToHideInsteadOfRoot = Array.Empty<GameObject>();
        }

        public override void OnRefreshWeaponAnimatorProperties(FirstPersonMover owner)
        {
            owner._characterModel.SetUpperBool("HasClaws", true);
            owner._characterModel.SetUpperAnimator(WeaponManager.Instance.GetClawAnimator(false));
        }
    }
}
