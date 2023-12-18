using UnityEngine;

namespace OverhaulMod.Combat.AIBehaviours
{
    public class SprintAIBehaviour : ComposableFirstPersonMoverBehaviour
    {
        public MinMaxRange Cooldown;

        public float DontDashIfWithin;

        private float m_NextTime;

        private float m_DefaultSpeed;
        private float m_SprintSpeed;

        public bool activated
        {
            get;
            set;
        }

        public virtual float sprintMultiplier
        {
            get
            {
                return 1.1f;
            }
        }

        public virtual float sprintMaxIntervalValue
        {
            get
            {
                return 5f;
            }
        }

        public virtual float sprintActivationCooldownOverride
        {
            get
            {
                return -1f;
            }
        }

        public override void Initialize(AIController controller)
        {
            base.Initialize(controller);

            m_DefaultSpeed = controller._character.GetBaseMoveSpeed();
            m_SprintSpeed = m_DefaultSpeed * sprintMultiplier;
            m_NextTime = Time.time + Cooldown.GetRandomValue();
            Cooldown = new MinMaxRange() { Min = 4f, Max = sprintMaxIntervalValue };
        }

        public override void UpdateBehaviour(Character targetOpponentCharacter, Vector3 positionDiff, float relativeRotationX, float relativeRotationY)
        {
            if (Time.time > m_NextTime)
            {
                m_NextTime = Time.time + (!activated || sprintActivationCooldownOverride == -1f ? Cooldown.GetRandomValue() : sprintActivationCooldownOverride);
                activated = !activated;
                _firstPersonMover.MaxSpeed = activated ? m_SprintSpeed : m_DefaultSpeed;
            }
        }
    }
}
