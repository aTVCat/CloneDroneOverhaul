using CDOverhaul.Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Multiplayer
{
    public class PlayerStatusWorldCanvas : OverhaulBehaviour
    {
        private OverhaulCameraController m_CameraController;
        private OverhaulCameraController cameraController
        {
            get
            {
                if (!m_CameraController)
                    m_CameraController = OverhaulController.GetController<OverhaulCameraController>();

                return m_CameraController;
            }
        }

        private Text m_Text;

        public void Initialize()
        {
            ModdedObject m = base.GetComponent<ModdedObject>();
            m_Text = m.GetObject<Text>(0);
            SetText(string.Empty);
        }

        public void SetText(string text)
        {
            m_Text.text = text;
        }

        protected override void OnDisposed()
        {
            m_Text = null;
            m_CameraController = null;
        }

        private void LateUpdate()
        {
            if (IsDisposedOrDestroyed())
                return;

            Camera cam = cameraController.mainCamera;
            if (cam)
                base.transform.rotation = Quaternion.LookRotation(-(cam.transform.position - base.transform.position));
        }
    }
}