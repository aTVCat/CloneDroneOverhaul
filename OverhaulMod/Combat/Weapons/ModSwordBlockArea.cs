using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Combat.Weapons
{
    public class ModSwordBlockArea : SwordBlockArea
    {
        public UpgradeType FireUpgrade;

        public new void OnTriggerEnter(Collider otherCollider)
        {
            SwordHitArea hitArea = SwordHitArea;
            if (!hitArea || !hitArea.IsDamageActive())
                return;

            if (BounceWeaponOnEnvironmentImpact && Tags.IsEnvironment(otherCollider.tag) && !otherCollider.isTrigger)
            {
                FirstPersonMover owner = hitArea.Owner;
                if (!owner)
                    return;

                AudioManager audioManager = ModCache.audioManager;
                AttackManager attackManager = ModCache.attackManager;
                AudioLibrary audioLibrary = ModCache.audioLibrary;

                Transform t = base.transform;
                float pitch = owner.WeaponEnvironmentImpactPitch;
                float time = Time.time;

                Vector3 edgePointCenter = hitArea.GetEdgePointCenter();
                hitArea.Owner.OnWeaponCollidedWithEnvironment();
                if (time > _lastEnvironmentImpactTime + 0.3f)
                {
                    if (hitArea.Owner.HasUpgrade(FireUpgrade))
                    {
                        _ = audioManager.PlayClipAtTransform(audioLibrary.FireSwordEnvironmentImpacts, t);
                        _ = ModCache.globalFireParticleSystem.CreateGroundImpactVFX(edgePointCenter);
                    }
                    else if (hitArea.Owner.CharacterType == EnemyType.EmperorNonCombat || hitArea.Owner.CharacterType == EnemyType.EmperorCombat)
                    {
                        attackManager.CreateEmperorWeaponEnvironmentImpactVFX(edgePointCenter);
                        _ = audioManager.PlayClipAtTransform(audioLibrary.SwordEnvironmentImpacts_Emperor, t, 0f, false, 1f, pitch, 0f);
                    }
                    else
                    {
                        if (IsMindSpaceWeapon)
                            _ = audioManager.PlayClipAtTransform(audioLibrary.MindSpaceEnvironmentSwordImpact, t, 0f, false, 1f, pitch, 0f);
                        else
                            _ = audioManager.PlayClipAtTransform(audioLibrary.SwordBlocks, t, 0f, false, 1f, pitch, 0f);

                        attackManager.CreateSwordBlockVFX(edgePointCenter);
                    }
                }
                _lastEnvironmentImpactTime = time;
            }
        }
    }
}
