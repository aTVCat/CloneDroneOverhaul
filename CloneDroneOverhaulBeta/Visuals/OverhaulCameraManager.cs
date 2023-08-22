using CDOverhaul.Gameplay;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class OverhaulCameraManager : OverhaulManager<OverhaulCameraManager>
    {
        private Camera m_MainCamera;
        public Camera mainCamera
        {
            get
            {
                if (!m_MainCamera || !m_MainCamera.enabled || !m_MainCamera.gameObject.activeInHierarchy)
                    m_MainCamera = Camera.main;

                return m_MainCamera;
            }
            private set => m_MainCamera = value;
        }

        public void RefreshMainCamera(Camera camera)
        {
            this.mainCamera = camera;
        }
    }
}
