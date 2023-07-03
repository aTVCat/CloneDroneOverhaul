using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.QualityOfLife
{
    public class UpgradeModesButtonBehaviour : OverhaulBehaviour
    {
        private Button m_Button;

        public override void OnEnable()
        {
            if (!m_Button)
            {
                m_Button = base.GetComponent<Button>();
                if (!m_Button)
                    return;
            }

            UpgradeModesController modesController = OverhaulController.GetController<UpgradeModesController>();
            if (modesController)
            {
                modesController.SetMode(UpgradeMode.Upgrade);
            }

            base.transform.localScale = GameModeManager.IsSinglePlayer() ? Vector3.one : Vector3.zero;
        }

        protected override void OnDisposed()
        {
            m_Button = null;
        }
    }
}
