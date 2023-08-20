using CDOverhaul.Gameplay;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class OverhaulCameraController : OverhaulManager
    {
        private Camera m_MainCamera;
        public Camera mainCamera
        {
            get
            {
                if (!m_MainCamera)
                    m_MainCamera = Camera.main;

                return m_MainCamera;
            }
            private set => m_MainCamera = value;
        }

        public override void Start()
        {
            base.Start();
            OverhaulEventsController.AddEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, RefreshVisibleCameras);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            OverhaulEventsController.RemoveEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, RefreshVisibleCameras);
        }

        public void RefreshVisibleCameras(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
        }
    }
}
