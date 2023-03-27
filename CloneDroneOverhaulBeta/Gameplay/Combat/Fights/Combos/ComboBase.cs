using UnityEngine;

namespace CDOverhaul.Gameplay.Combat.Fights
{
    public class ComboBase : OverhaulBehaviour
    {
        private float m_TimeToAllowTriggering;
        public bool CanTrigger => Time.time >= m_TimeToAllowTriggering;

        public virtual bool TryTrigger(FirstPersonMover mover, FPMoveCommand command)
        {
            return CanTrigger;
        }

        public void StartCooldown(float seconds)
        {
            m_TimeToAllowTriggering = Time.time + seconds;
        }

        public void Unbalance(FirstPersonMover mover, float value)
        {
            CombatOverhaulUnbalancing un = mover.GetComponent<CombatOverhaulUnbalancing>();
            if (un)
            {
                un.Unbalance(value);
            }
        }
    }
}
