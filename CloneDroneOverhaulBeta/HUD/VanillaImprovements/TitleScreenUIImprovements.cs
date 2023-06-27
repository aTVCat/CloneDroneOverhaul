using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Vanilla
{
    public class TitleScreenUIImprovements : OverhaulBehaviour
    {
        private ModdedObject m_ModdedObject;

        public OverhauledMessagePanel MessagePanel;

        public override void Start()
        {
            m_ModdedObject = base.GetComponent<ModdedObject>();
            MessagePanel = m_ModdedObject.GetObject<Transform>(0).gameObject.AddComponent<OverhauledMessagePanel>();
            MessagePanel.Initialize(this);
        }

        private void Update()
        {
            MessagePanel.gameObject.SetActive(GameModeManager.IsOnTitleScreen());
        }

        public class OverhauledMessagePanel : OverhaulBehaviour
        {
            public TitleScreenUIImprovements UIImprovements;

            public void Initialize(TitleScreenUIImprovements ui)
            {
                UIImprovements = ui;
            }
        }
    }
}
