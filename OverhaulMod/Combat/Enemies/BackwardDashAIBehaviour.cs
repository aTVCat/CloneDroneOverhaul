using UnityEngine;

namespace OverhaulMod.Combat.Enemies
{
    public class BackwardDashAIBehaviour : ComposableFirstPersonMoverBehaviour
    {
        private float m_NextTime;

        public virtual float dashMultiplier
        {
            get
            {
                return -20f;
            }
        }

        public override void UpdateBehaviour(Character targetOpponentCharacter, Vector3 positionDiff, float relativeRotationX, float relativeRotationY)
        {
            float time = Time.time;

            if (positionDiff.magnitude < 10f && time > m_NextTime && !_composableController.IsMovementInputDisabledByAnyAIBehaviour())
            {
                m_NextTime = time + 5f;

                FirstPersonMover firstPersonMover = targetOpponentCharacter as FirstPersonMover;
                if (firstPersonMover)
                {
                    bool shouldRunAway = firstPersonMover.IsSwingingMeleeWeapon() || firstPersonMover.IsKicking();
                    if (shouldRunAway)
                    {
                        _firstPersonMover.AddVelocity(_firstPersonMover.transform.forward * dashMultiplier);
                        _firstPersonMover.transform.eulerAngles += new Vector3(0f, 180f, 0f);
                        _firstPersonMover.playDashAnimation(false);
                    }
                }
            }
        }
    }
}
