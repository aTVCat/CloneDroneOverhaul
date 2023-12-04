using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class OverhaulCameraManager : OverhaulManager<OverhaulCameraManager>
    {
        public const string CAMERA_CHANGED_EVENT = "CameraChanged";

        private Camera m_MainCamera;
        public Camera mainCamera
        {
            get
            {
                if (!m_MainCamera || !m_MainCamera.enabled || !m_MainCamera.gameObject.activeInHierarchy)
                {
                    m_MainCamera = Camera.main;
                    OverhaulEvents.DispatchEvent(CAMERA_CHANGED_EVENT, m_MainCamera);
                }

                return m_MainCamera;
            }
            private set => m_MainCamera = value;
        }

        public void RefreshMainCamera(Camera camera)
        {
            mainCamera = camera;
        }
    }
}
