using Bolt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat.Fights
{
    public class CombatOverhaulCombosBehaviour : CombatOverhaulMechanic
    {
        private List<ComboBase> m_Combos;

        public void Initialize(List<ComboBase> combos)
        {
            m_Combos = new List<ComboBase>();
            foreach(ComboBase cBase in combos)
            {
                m_Combos.Add((ComboBase)base.gameObject.AddComponent(cBase.GetType()));
            }
        }

        public override void OnPreCommandExecute(FPMoveCommand command)
        {
            if (m_Combos.IsNullOrEmpty())
            {
                return;
            }

            int i = 0;
            do
            {
                ComboBase combo = m_Combos[i];
                if (combo == null)
                {
                    i++;
                    continue;
                }
                if (combo.TryTrigger(base.FirstPersonMover, command)) break;
                i++;

            } while (i < m_Combos.Count);
        }
    }
}
