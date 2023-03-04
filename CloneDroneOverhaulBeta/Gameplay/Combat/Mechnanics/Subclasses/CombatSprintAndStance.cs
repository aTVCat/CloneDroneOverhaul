using System.Collections;
using UnityEngine;
using static KopiLua.Lua;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace CDOverhaul.Gameplay.Combat
{
    public class CombatSprintAndStance : CombatOverhaulMechanic
    {
        /// <summary>
        /// The state of robot. Null - default, false - stance, true - sprint
        /// </summary>
        private bool? m_State;

        private float m_Speed;

        private bool m_ShouldDash;
        private float m_MaxTimeToDash;

        private bool m_HasJetpack;
        private bool m_CanSprint;
        private float m_TimeToGiveSprintBack;

        public override void Start()
        {
            base.Start();
            SetState(null);
            SetCanSprint(true);
            m_Speed = GetDefaultSpeed();

            RefreshJetpackUpgrade();
        }

        protected override void OnRefresh()
        {
            RefreshJetpackUpgrade();
        }

        public override void OnPreCommandExecute(FPMoveCommand command)
        {
            if (m_ShouldDash)
            {
                return;
            }
            command.Input.JetpackHeld = false;
        }

        private void Update()
        {
            if (IsOwnerMainPlayer())
            {

                if (IsPressed(KeyCode.LeftControl, 1))
                {
                    SetState(false);
                }
                else
                {
                    SetState(null);
                }

                if (IsPressed(KeyCode.LeftShift, 0))
                {
                    if (m_HasJetpack)
                    {
                        m_ShouldDash = true;
                    }
                    else
                    {
                        if (Time.time < m_MaxTimeToDash)
                        {
                            m_ShouldDash = true;
                            m_MaxTimeToDash = -1f;
                            return;
                        }
                        else
                        {
                            m_ShouldDash = false;
                        }
                        m_MaxTimeToDash = Time.time + 0.2f;
                    }
                }
                if (!m_HasJetpack)
                {
                    if (m_CanSprint && IsPressed(KeyCode.LeftShift, 1))
                    {
                        if (!TryConsumeEnergy(0.375f))
                        {
                            SetCanSprint(false);
                            m_TimeToGiveSprintBack = Time.time + 5f;
                            return;
                        }
                        SetState(true);
                    }
                }
            }

            if(Time.time >= m_TimeToGiveSprintBack)
            {
                SetCanSprint(true);
            }

            SetRobotSpeed(GetSpeed(true), true, FirstPersonMover.IsKicking() || FirstPersonMover.HasFallenDown() || FirstPersonMover.IsGettingUpFromKick());
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

        public void SetState(bool? value)
        {
            m_State = value;
        }

        public float GetRobotSpeedForState(bool? state)
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
                m_Speed += (GetRobotSpeedForState(m_State) - m_Speed) * 6 * Time.deltaTime;
            }
            return m_Speed;
        }

        public void SetCanSprint(bool value)
        {
            m_CanSprint = value;
        }

        public void RefreshJetpackUpgrade()
        {
            m_HasJetpack = UpgradeCollection.HasUpgrade(UpgradeType.Jetpack);
        }
    }
}