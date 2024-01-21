using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class WorkshopBrowserButtonBehaviour : OverhaulBehaviour
    {
        private Button m_button;

        private SteamManager m_steamManager;
        private SteamManager steamManager
        {
            get
            {
                if (!m_steamManager)
                {
                    m_steamManager = SteamManager.Instance;
                }
                return m_steamManager;
            }
        }

        public override void Start()
        {
            m_button = base.GetComponent<Button>();
            if (!m_button)
                base.enabled = false;
        }

        private void Update()
        {
            SteamManager manager = steamManager;
            if (!m_button || !manager)
                return;

            bool initialized = manager.Initialized;
            m_button.interactable = initialized;
            Image image = m_button.image;
            if (image)
            {
                Color color = image.color;
                color.a = initialized ? 1f : 0.5f;
                image.color = color;
            }
        }
    }
}
