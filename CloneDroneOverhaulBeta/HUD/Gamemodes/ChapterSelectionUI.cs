using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD.Gamemodes
{
    public class ChapterSelectionUI : OverhaulBehaviour
    {
        private OverhaulGamemodesUI m_GamemodesUI;

        private int m_SelectedChapter;

        public ChapterSelectionUI Initialize(OverhaulGamemodesUI gamemodesUI)
        {
            m_GamemodesUI = gamemodesUI;
            reset();
            return this;
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            m_GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/chapter1Preview.jpeg");
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        private void reset()
        {
            m_SelectedChapter = 1;
        }

        public override void OnEnable()
        {
            reset();
        }

        private void Update()
        {
            if (!OverhaulGamemodesUI.AllowSwitching)
                return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                if(m_SelectedChapter < 5) m_SelectedChapter++;
                m_GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/chapter" + m_SelectedChapter + "Preview.jpeg");
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (m_SelectedChapter > 1) m_SelectedChapter--;
                m_GamemodesUI.ChangeBackgroundTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/chapter" + m_SelectedChapter + "Preview.jpeg");
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
                m_GamemodesUI.Hide();
            }
        }
    }
}
