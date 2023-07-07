using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD
{
    public class AdvancedPhotomodeUI : OverhaulUI
    {
        private GameObject m_PanelGameObject;

        public override void Initialize()
        {
            m_PanelGameObject = MyModdedObject.GetObject<Transform>(0).gameObject;
            Hide();
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            SetPanelActive(true);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        public void SetPanelActive(bool value)
        {
            m_PanelGameObject.gameObject.SetActive(value);
        }
    }
}
