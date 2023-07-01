using CDOverhaul.Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class PlayerStatusWorldCanvas : OverhaulBehaviour
    {
        private Text m_Text;

        public void Initialize()
        {
            ModdedObject m = base.GetComponent<ModdedObject>();
            if (m == null)
            {
                base.gameObject.SetActive(false);
                return;
            }

            m_Text = m.GetObject<Text>(0);
            SetText(string.Empty);
        }

        public void SetText(string text)
        {
            if (m_Text != null)
            {
                m_Text.text = text;
            }
        }

        protected override void OnDisposed()
        {
            m_Text = null;
        }

        private void LateUpdate()
        {
            Camera cam = OverhaulGraphicsController.CameraController.GetMainCamera();
            if (cam != null)
            {
                base.transform.rotation = Quaternion.LookRotation(-(cam.transform.position - base.transform.position));
            }
        }
    }
}