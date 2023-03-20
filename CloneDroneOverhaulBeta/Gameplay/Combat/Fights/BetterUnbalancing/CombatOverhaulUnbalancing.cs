using UnityEngine;

namespace CDOverhaul.Gameplay.Combat.Fights
{
    public class CombatOverhaulUnbalancing : CombatOverhaulMechanic
    {
        public const float MaxUnbalancing = 8f;

        private float m_UnbalancedValue;
        public float UnbalancedValue
        {
            get
            {
                return m_UnbalancedValue;
            }
            set
            {
                m_UnbalancedValue = Mathf.Clamp(value, 0f, MaxUnbalancing);
            }
        }

        public CombatSprintAndStance SprintAndStance;

        public override void OnPreCommandExecute(FPMoveCommand command)
        {
            LowerUnbalancedValue();
            float pong = Mathf.PingPong(Time.time, 1.5f) - 0.75f;
            //command.Input.HorizontalMovement += pong * UnbalancedValue;
            SprintAndStance.MaxSpeedOffset = pong * UnbalancedValue;

            if (!FirstPersonMover.HasFallenDown() && UnbalancedValue >= 4f)
            {
                FirstPersonMover.SendFalling(-base.transform.up, 1f);
            }
        }

        public void LowerUnbalancedValue()
        {
            UnbalancedValue -= Time.deltaTime;
        }

        public void Unbalance(float value)
        {
            UnbalancedValue += value;
            if(FirstPersonMover.IsOnGroundServer()) FirstPersonMover.AddVelocity(base.transform.up + (base.transform.forward * 6));
        }

        public override void Start()
        {
            base.Start();
            UnbalancedValue = 0f;
        }
    }
}