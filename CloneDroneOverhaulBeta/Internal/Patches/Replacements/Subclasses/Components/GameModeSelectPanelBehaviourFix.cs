using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Patches
{
    public class GameModeSelectPanelBehaviourFix : OverhaulBehaviour
    {
        private Transform m_Box;

        public override void OnEnable()
        {
            if (!m_Box)
            {
                m_Box = base.transform.FindChildRecurisve("Box");
                if (!m_Box)
                    return;
            }

            m_Box.gameObject.SetActive(true);
        }
    }
}
