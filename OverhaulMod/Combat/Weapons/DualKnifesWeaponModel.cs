using System;
using UnityEngine;

namespace OverhaulMod.Combat.Weapons
{
    public class DualKnifesWeaponModel : ModWeaponModel
    {
        private HammerImpactMeleeArea m_secondaryImpactArea;

        public override float attackSpeed
        {
            get
            {
                return 1.6f;
            }
        }

        public override AttackDirection attackDirections
        {
            get
            {
                return AttackDirection.Left | AttackDirection.Right | AttackDirection.Forward;
            }
        }

        public override AttackDirection defaultAttackDirection
        {
            get
            {
                return AttackDirection.Forward;
            }
        }

        private void OnEnable()
        {
            if (m_secondaryImpactArea)
                m_secondaryImpactArea.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (m_secondaryImpactArea)
                m_secondaryImpactArea.gameObject.SetActive(false);
        }

        public override void OnInstantiated(FirstPersonMover owner)
        {
            _ = createImpactArea(true, base.transform, owner);
            Transform handL = TransformUtils.FindChildRecursive(owner.transform, "HandL");
            if (handL)
                m_secondaryImpactArea = createImpactArea(false, handL, owner);

            base.BodyPartsToDrop = Array.Empty<MindSpaceBodyPart>();
            base.PartsToDrop = Array.Empty<Transform>();
            base.PartsToHideInsteadOfRoot = Array.Empty<GameObject>();
        }

        public override void SetWeaponDamageActive(bool value)
        {
            base.SetWeaponDamageActive(value);

            FirstPersonMover firstPersonMover = GetOwner();
            if (firstPersonMover && m_secondaryImpactArea)
            {
                m_secondaryImpactArea.SetDamageActive(value && !firstPersonMover.IsDamaged(MechBodyPartType.LeftArm));
            }
        }

        public override void OnRefreshWeaponAnimatorProperties(FirstPersonMover owner)
        {
            owner._characterModel.SetUpperBool("HasSword", true);
            owner._characterModel.SetLegsFloat("AttackSpeed", 1.5f);
            owner._characterModel.SetUpperAnimator(WeaponManager.Instance.DefaultUpperBodyAnimator);
        }

        private HammerImpactMeleeArea createImpactArea(bool isMain, Transform parent, FirstPersonMover owner)
        {
            GameObject impactAreaObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            impactAreaObject.name = "ImpactArea";
            impactAreaObject.transform.SetParent(parent, false);
            impactAreaObject.transform.localScale = Vector3.one;
            impactAreaObject.transform.localEulerAngles = Vector3.zero;
            impactAreaObject.transform.localPosition = Vector3.zero;
            impactAreaObject.layer = 18;
            BoxCollider boxCollider = impactAreaObject.AddComponent<BoxCollider>();
            boxCollider.size = Vector3.one * 2f;
            Rigidbody rigidbody = impactAreaObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            HammerImpactMeleeArea hammerImpactMeleeArea = impactAreaObject.AddComponent<HammerImpactMeleeArea>();
            hammerImpactMeleeArea.SetOwner(owner);
            hammerImpactMeleeArea.ImpactCenterPoint = impactAreaObject.transform;
            hammerImpactMeleeArea.DamageSourceType = DamageSourceType.Hammer;
            hammerImpactMeleeArea.DisableScreenShake = true;
            hammerImpactMeleeArea.ImpactMultiplier = 0.1f;
            hammerImpactMeleeArea.TiedToHammerUpgrade = false;
            hammerImpactMeleeArea.NumOfPieces = 2;
            if (isMain)
            {
                base.MeleeImpactArea = hammerImpactMeleeArea;
                base.BodyPartsToDrop = Array.Empty<MindSpaceBodyPart>();
                base.PartsToDrop = Array.Empty<Transform>();
                base.PartsToHideInsteadOfRoot = Array.Empty<GameObject>();
            }
            owner.AddDeathListener(delegate
            {
                if (impactAreaObject)
                    Destroy(impactAreaObject);
            });
            return hammerImpactMeleeArea;
        }
    }
}
