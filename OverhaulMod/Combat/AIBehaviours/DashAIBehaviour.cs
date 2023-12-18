using UnityEngine;

namespace OverhaulMod.Combat.AIBehaviours
{
    public class DashAIBehaviour : ComposableFirstPersonMoverBehaviour
    {
        public MinMaxRange WaitBetweenDashes;

        private float m_NextTime;

        public override void Initialize(AIController controller)
        {
            base.Initialize(controller);

            m_NextTime = Time.time + WaitBetweenDashes.GetRandomValue();
            WaitBetweenDashes = new MinMaxRange() { Min = 4f, Max = 7f };
        }

        public override void UpdateBehaviour(Character targetOpponentCharacter, Vector3 positionDiff, float relativeRotationX, float relativeRotationY)
        {
            if (Time.time > m_NextTime && !_composableController.IsMovementInputDisabledByAnyAIBehaviour())
            {
                m_NextTime = Time.time + WaitBetweenDashes.GetRandomValue();

                float multiplier = _firstPersonMover.IsJumping() ? 0.5f : 1f;

                _firstPersonMover.AddVelocity(_firstPersonMover.transform.forward * 25f * multiplier);
                _firstPersonMover.playDashAnimation(false);
            }
        }
    }
}
