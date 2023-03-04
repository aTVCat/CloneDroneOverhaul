using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class CombatSprintAndStance : CombatOverhaulMechanic
    {
        /// <summary>
        /// The state of robot. Null - default, false - stance, true - sprint
        /// </summary>
        private bool? m_MovementState;

        private bool m_HasJetpack;
        private float m_Speed;

        private bool m_ShouldDash;
        private float m_MaxTimeToDash;

        private bool m_CanSprint;
        private float m_TimeToGiveSprintBack;

        private bool m_PressedShiftThisFrame;
        private bool m_HoldingShift;

        private bool m_PressedCtrlThisFrame;
        private bool m_HoldingCtrl;

        public override void Start()
        {
            base.Start();
            SetMovementState(null);
            SetCanSprint(true);
            m_Speed = GetDefaultSpeed();

            OnRefresh();
        }

        protected override void OnRefresh()
        {
            m_HasJetpack = UpgradeCollection.HasUpgrade(UpgradeType.Jetpack);
        }

        public override void OnPreCommandExecute(FPMoveCommand command)
        {
            if (!m_ShouldDash)
            {
                command.Input.JetpackHeld = false;
            }
            else
            {
                m_ShouldDash = false;
            }
            if(m_MovementState == false)
            {
                if (FirstPersonMover.IsOnGroundServer() && command.Input.SecondAttackDown)
                {
                    FirstPersonMover.AddVelocity(FirstPersonMover.transform.right * (30 * command.Input.HorizontalMovement));
                }
                command.Input.SecondAttackDown = false;
                command.Input.Jump = false;
            }
        }

        private void Update()
        {
            if (IsOwnerMainPlayer())
            {
                SetCtrlDown(IsPressed(KeyCode.LeftControl, 1));
                SetShiftDown(IsPressed(KeyCode.LeftShift, 1));
            }

            SetMovementState(null);
            if(!m_HoldingShift && m_HoldingCtrl)
            {
                SetMovementState(false);
            }
            else if (CanSprint() && m_HoldingShift && !m_HoldingCtrl)
            {
                SetMovementState(true);
            }

            if (m_MovementState == true)
            {
                if (m_HasJetpack)
                {
                    m_ShouldDash = true;
                }
                else
                {
                    if (!m_ShouldDash && m_PressedShiftThisFrame)
                    {
                        if (Time.time < m_MaxTimeToDash)
                        {
                            m_ShouldDash = true;
                            m_MaxTimeToDash = -1f;
                        }
                        m_MaxTimeToDash = Time.time + 0.2f;
                    }

                    if (!TryConsumeEnergy(0.375f))
                    {
                        SetCanSprint(false);
                        m_TimeToGiveSprintBack = Time.time + 5f;
                    }
                }
            }            

            if(Time.time >= m_TimeToGiveSprintBack)
            {
                SetCanSprint(true);
            }

            SetRobotSpeed(GetSpeed(true), true, FirstPersonMover.IsKicking() || FirstPersonMover.HasFallenDown() || FirstPersonMover.IsGettingUpFromKick());

            m_PressedCtrlThisFrame = false;
            m_PressedShiftThisFrame = false;
        }

        public bool TryConsumeEnergy(float amountPerSecond)
        {
            float toConsume = amountPerSecond * Time.deltaTime;
            if (!EnergySource.CanConsume(toConsume))
            {
                return false;
            }
            EnergySource.Consume(toConsume);
            return true;
        }

        public float GetTargetSpeed(bool? state)
        {
            if(state == null)
            {
                return GetDefaultSpeed();
            }
            if (state.Value)
            {
                return GetDefaultSpeed() + 5f;
            }
            else
            {
                return 4.5f;
            }
        }

        public float GetSpeed(bool updateFirst)
        {
            if (updateFirst)
            {
                m_Speed += (GetTargetSpeed(m_MovementState) - m_Speed) * 6 * Time.deltaTime;
            }
            return m_Speed;
        }


        public void SetMovementState(bool? value)
        {
            m_MovementState = value;
        }

        public void SetCanSprint(bool value)
        {
            m_CanSprint = value;
        }
        public bool CanSprint()
        {
            return m_CanSprint && m_MovementState != false;
        }

        public void SetShiftDown(bool value)
        {
            if (!m_HoldingShift) m_PressedShiftThisFrame = value;
            m_HoldingShift = value;
        }

        public void SetCtrlDown(bool value)
        {
            if (!m_HoldingCtrl) m_PressedCtrlThisFrame = value;
            m_HoldingCtrl = value;
        }
    }
}