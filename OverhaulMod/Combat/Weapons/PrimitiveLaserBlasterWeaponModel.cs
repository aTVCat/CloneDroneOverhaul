using System;
using UnityEngine;

namespace OverhaulMod.Combat.Weapons
{
    public class PrimitiveLaserBlasterWeaponModel : ModWeaponModel
    {
        public override void OnInstantiated(FirstPersonMover owner)
        {
            Transform transform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            transform.SetParent(base.transform, false);
            transform.localScale = Vector3.one * 0.25f;

            base.BodyPartsToDrop = Array.Empty<MindSpaceBodyPart>();
            base.PartsToDrop = Array.Empty<Transform>();
            base.PartsToHideInsteadOfRoot = Array.Empty<GameObject>();
        }

        public override void OnExecuteAttackCommands(FirstPersonMover owner, IFPMoveCommandInput input)
        {
            if (input.AttackKeyHeld && ModTime.fixedFrameCount % 5 == 0)
            {
                Vector3 directionVector = (owner.transform.position + (owner.transform.up * 2.5f) + (owner.transform.forward * 5f) - base.transform.position).normalized;
                _ = ProjectileManager.Instance.CreateFlameBreathProjectile(base.transform.position, directionVector, owner);
            }
        }
    }
}
