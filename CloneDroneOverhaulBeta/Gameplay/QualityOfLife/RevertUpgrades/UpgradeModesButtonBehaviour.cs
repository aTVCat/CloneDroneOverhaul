﻿using UnityEngine;
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

            UpgradeModesSystem modesController = OverhaulGameplayManager.reference?.upgradeModes;
            if (modesController)
            {
                modesController.SetMode(UpgradeMode.Upgrade);
            }

            base.transform.localScale = GameModeManager.IsSinglePlayer() && !GameModeManager.IsInLevelEditor() && !GameModeManager.IsChapter3Or4() ? Vector3.one : Vector3.zero;
        }

        protected override void OnDisposed()
        {
            m_Button = null;
        }
    }
}
