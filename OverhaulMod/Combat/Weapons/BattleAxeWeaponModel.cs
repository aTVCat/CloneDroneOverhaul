using OverhaulMod.Utils;
using System;
using UnityEngine;

namespace OverhaulMod.Combat.Weapons
{
    public class BattleAxeWeaponModel : ModWeaponModel
    {
        public float VelocityToAdd = 20f;

        public override float attackSpeed
        {
            get
            {
                return 0.95f;
            }
        }

        public override float disableAttacksForSeconds
        {
            get
            {
                return 1.3f;
            }
        }

        public override RuntimeAnimatorController animatorControllerOverride
        {
            get
            {
                return WeaponManager.Instance.Multiplayer_HammerUpperBodyAnimator;
            }
        }

        public override void OnInstantiated(FirstPersonMover owner)
        {
            MeleeWeaponAITuning meleeWeaponTuning = (MeleeWeaponAITuning)WeaponAITuningManager.Instance.SwordAITuning.MemberwiseClone();
            meleeWeaponTuning.MaxRangeToAttack = 12f;
            meleeWeaponTuning.RunForwardUntilRange = 6f;
            AITuning = meleeWeaponTuning;

            base.transform.localPosition = new Vector3(1.2f, -0.2f, 0f);
            base.transform.localEulerAngles = new Vector3(0f, 90f, 90f);
            base.transform.localScale = new Vector3(1.25f, 1.25f, 1.75f);

            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            ModDividedSwordHitArea hitArea = moddedObject.GetObject<GameObject>(0).AddComponent<ModDividedSwordHitArea>();
            hitArea.Blades = new System.Collections.Generic.List<BladeEdgePoints>()
            {
                new BladeEdgePoints() { EdgePoint1 = moddedObject.GetObject<Transform>(1), EdgePoint2 = moddedObject.GetObject<Transform>(2)},
                new BladeEdgePoints() { EdgePoint1 = moddedObject.GetObject<Transform>(3), EdgePoint2 = moddedObject.GetObject<Transform>(4)}
            };

            hitArea.EdgePoint1 = moddedObject.GetObject<Transform>(1);
            hitArea.EdgePoint2 = moddedObject.GetObject<Transform>(2);
            hitArea.SetOwner(owner);
            ModSwordBlockArea swordBlockArea = moddedObject.GetObject<GameObject>(0).AddComponent<ModSwordBlockArea>();
            swordBlockArea.BlockHammers = false;
            swordBlockArea.BounceWeaponOnEnvironmentImpact = true;
            swordBlockArea.DestroyProjectilesOnImpact = true;
            swordBlockArea.SwordHitArea = hitArea;
            swordBlockArea.FireUpgrade = ModUpgradesManager.AXE_FIRE_UPGRADE;
            base.MeleeImpactArea = hitArea;
            base.BodyPartsToDrop = Array.Empty<MindSpaceBodyPart>();
            base.PartsToDrop = new Transform[]
            {
                moddedObject.GetObject<Transform>(5),
                moddedObject.GetObject<Transform>(6)
            };
            base.PartsToHideInsteadOfRoot = Array.Empty<GameObject>();

            RefreshRenderer();
        }

        public override void OnRefreshWeaponAnimatorProperties(FirstPersonMover owner)
        {
            owner._characterModel.SetUpperBool("HasHammer", true);
        }

        public override void OnUpgradesRefresh(FirstPersonMover owner)
        {
            RefreshRenderer();

            MeleeImpactArea.SetFireSpreadDefinition(owner.HasUpgrade(ModUpgradesManager.AXE_FIRE_UPGRADE) ? FireManager.Instance.GetFireSpreadDefinition(FireType.FlameBreathPlayer) : null);
        }

        public override void SetWeaponDamageActive(bool value)
        {
            base.SetWeaponDamageActive(value);
            FirstPersonMover owner = base.GetOwner();
            if (value && owner && owner.IsAttachedAndAlive() && owner._isMovingForward)
            {
                owner.AddVelocity(owner.transform.forward * VelocityToAdd * (owner.IsJumping() ? 0.45f : 1f));
            }
        }

        public void RefreshRenderer()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            if (!moddedObject)
                return;

            MeshRenderer meshRenderer = moddedObject.GetObject<MeshRenderer>(5);
            if (meshRenderer)
            {
                meshRenderer.material.shader = Shader.Find("Standard");
            }
            MeshRenderer meshRenderer2 = moddedObject.GetObject<MeshRenderer>(6);
            if (meshRenderer2)
            {
                meshRenderer2.material.shader = Shader.Find("Standard");
            }
        }
    }
}
