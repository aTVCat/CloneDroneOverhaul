using OverhaulMod.Utils;
using System;
using UnityEngine;

namespace OverhaulMod.Combat.Weapons
{
    public class ScytheWeaponModel : ModWeaponModel
    {
        public override float attackSpeed
        {
            get
            {
                return 0.9f;
            }
        }

        public override float disableAttacksForSeconds
        {
            get
            {
                return 1.1f;
            }
        }

        public override AttackDirection attackDirections
        {
            get
            {
                return base.attackDirections | AttackDirection.Right;
            }
        }

        public override RuntimeAnimatorController animatorControllerOverride
        {
            get
            {
                return WeaponManager.Instance.Multiplayer_DefaultUpperBodyAnimator;
            }
        }

        public override void OnInstantiated(FirstPersonMover owner)
        {
            MeleeWeaponAITuning meleeWeaponTuning = (MeleeWeaponAITuning)WeaponAITuningManager.Instance.SwordAITuning.MemberwiseClone();
            meleeWeaponTuning.MaxRangeToAttack = 13f;
            meleeWeaponTuning.RunForwardUntilRange = 7.5f;
            AITuning = meleeWeaponTuning;

            base.transform.localPosition = new Vector3(1.2f, -0.2f, 0f);
            base.transform.localEulerAngles = new Vector3(0f, 90f, 90f);
            base.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);

            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            SwordHitArea hitArea = moddedObject.GetObject<GameObject>(0).AddComponent<SwordHitArea>();
            hitArea.EdgePoint1 = moddedObject.GetObject<Transform>(1);
            hitArea.EdgePoint2 = moddedObject.GetObject<Transform>(2);
            hitArea.SetOwner(owner);
            ModSwordBlockArea swordBlockArea = moddedObject.GetObject<GameObject>(0).AddComponent<ModSwordBlockArea>();
            swordBlockArea.BlockHammers = true;
            swordBlockArea.BounceWeaponOnEnvironmentImpact = true;
            swordBlockArea.DestroyProjectilesOnImpact = true;
            swordBlockArea.SwordHitArea = hitArea;
            swordBlockArea.FireUpgrade = ModUpgradesManager.SCYTHE_FIRE_UPGRADE;
            base.MeleeImpactArea = hitArea;
            base.BodyPartsToDrop = Array.Empty<MindSpaceBodyPart>();
            base.PartsToDrop = new Transform[]
            {
                moddedObject.GetObject<Transform>(3),
                moddedObject.GetObject<Transform>(4)
            };
            base.PartsToHideInsteadOfRoot = Array.Empty<GameObject>();

            RefreshRenderer();
        }

        public override void OnRefreshWeaponAnimatorProperties(FirstPersonMover owner)
        {
            owner._characterModel.SetUpperBool("HasSword", true);
        }

        public override void OnUpgradesRefresh(FirstPersonMover owner)
        {
            RefreshRenderer();

            MeleeImpactArea.SetFireSpreadDefinition(owner.HasUpgrade(ModUpgradesManager.SCYTHE_FIRE_UPGRADE) ? FireManager.Instance.GetFireSpreadDefinition(FireType.FlameBreathPlayer) : null);
        }

        public void RefreshRenderer()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            if (!moddedObject)
                return;

            bool fire = GetOwner().HasUpgrade(ModUpgradesManager.SCYTHE_FIRE_UPGRADE);
            MeshRenderer meshRenderer = moddedObject.GetObject<MeshRenderer>(3);
            if (meshRenderer)
            {
                meshRenderer.material.shader = Shader.Find("Standard");
                meshRenderer.gameObject.SetActive(!fire);
            }
            MeshRenderer meshRenderer2 = moddedObject.GetObject<MeshRenderer>(4);
            if (meshRenderer2)
            {
                meshRenderer2.material.shader = Shader.Find("Standard");
                meshRenderer2.gameObject.SetActive(fire);
            }
        }
    }
}
