using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulPatchNotes : OverhaulUI
    {
        private Button m_OkButton;
        private Button m_ModBotButton;
        private Button m_GitHubButton;

        public override void Initialize()
        {
            m_OkButton = MyModdedObject.GetObject<Button>(0);
            m_OkButton.onClick.AddListener(OnOKClicked);
            m_GitHubButton = MyModdedObject.GetObject<Button>(1);
            m_GitHubButton.onClick.AddListener(OnGitHubClicked);
            m_ModBotButton = MyModdedObject.GetObject<Button>(2);
            m_ModBotButton.onClick.AddListener(OnModBotClicked);
            Hide();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();

            m_OkButton = null;
            m_ModBotButton = null;
            m_GitHubButton = null;
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        public void OnOKClicked()
        {
            Hide();
        }

        public void OnGitHubClicked()
        {
            Application.OpenURL("https://github.com/aTVCat/CloneDroneOverhaul/releases");
        }

        public void OnModBotClicked()
        {
            Application.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
        }
    }
}