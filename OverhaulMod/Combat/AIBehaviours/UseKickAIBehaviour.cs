using UnityEngine;

namespace OverhaulMod.Combat.AIBehaviours
{
    public class UseKickAIBehaviour : ComposableFirstPersonMoverBehaviour
    {
        private float m_NextTime;

        public override bool DisableOtherWeaponsFireThisFrame()
        {
            AIComposableBehaviourController behaviourController = _composableController;
            if (behaviourController)
            {
                Character character = behaviourController._targetOpponentCharacter;
                if (character && Time.time > m_NextTime - 2f && (character.transform.position - base.transform.position).magnitude < 7f)
                {
                    return true;
                }
            }
            return false;
        }

        public override void UpdateBehaviour(Character targetOpponentCharacter, Vector3 positionDiff, float relativeRotationX, float relativeRotationY)
        {
            if (positionDiff.magnitude < 15f && Time.time > m_NextTime && !_composableController.IsMovementInputDisabledByAnyAIBehaviour())
            {
                m_NextTime = Time.time + 5f;

                _firstPersonMover.AddVelocity(_firstPersonMover.transform.forward * 25f);
                _firstPersonMover.SetSecondAttackKeyDown(true);
            }
        }
    }
}
