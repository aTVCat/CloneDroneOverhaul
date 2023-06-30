﻿using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class AboutOverhaulMenu : OverhaulUI
    {
        private Button m_GoBack;

        public override void Initialize()
        {
            base.gameObject.SetActive(false);

            m_GoBack = MyModdedObject.GetObject<Button>(0);
            m_GoBack.onClick.AddListener(OnBackClick);
        }

        public void Show()
        {
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
            base.gameObject.SetActive(true);
        }

        public void Hide()
        {
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
            base.gameObject.SetActive(false);
        }

        public void OnBackClick()
        {
            Hide();
        }
    }
}
