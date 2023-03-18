using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat.Fights
{
    public class DoubleStrike : ComboBase
    {

        private bool m_WasSwingingWeaponLastCheck;
        private float m_TimeToAllowAttackActivation;

        public override bool TryTrigger(FirstPersonMover mover, FPMoveCommand command)
        {
            if (!base.CanTrigger)
            {
                return false;
            }

            bool sideSwing = mover.IsSwingingMeleeWeaponHorizontally();
            if (sideSwing != m_WasSwingingWeaponLastCheck)
            {
                if (sideSwing)
                {
                    m_TimeToAllowAttackActivation = Time.time + 0.6f;
                }
            }
            m_WasSwingingWeaponLastCheck = sideSwing;

            if (sideSwing && mover.IsWeaponDamageActive() && command.Input.AttackKeyDown)
            {
                float time = Time.time;
                if (m_TimeToAllowAttackActivation < time || time < m_TimeToAllowAttackActivation - 0.8f)
                {
                    return false;
                }

                playAnimation(mover);
                StartCooldown(2f);
                Unbalance(mover, 2f);
                return true;
            }
            return false;
        }

        private void playAnimation(FirstPersonMover mover)
        {
            CharacterModdedAnimationsExpansion exp = mover.GetComponent<CharacterModdedAnimationsExpansion>();
            if (exp)
            {
                exp.PlayCustomAnimaton("Combo_DoubleStrike");
            }
        }
    }
}