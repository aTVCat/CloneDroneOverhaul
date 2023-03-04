using CDOverhaul.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class OverhaulCameraController : OverhaulController
    {
        private Camera[] m_AllCameras;
        private Camera m_MainCamera;

        public bool IsMainCameraNull => !IsDisposedOrDestroyed() && m_MainCamera != null;

        public override void Initialize()
        {
            _ = OverhaulEventManager.AddEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, RefreshVisibleCameras);
        }

        protected override void OnDisposed()
        {
            OverhaulEventManager.RemoveEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, RefreshVisibleCameras);
            m_AllCameras = null;
            m_MainCamera = null;
        }

        public void RefreshVisibleCameras(Camera mainCamera)
        {
            m_MainCamera = mainCamera;
            m_AllCameras = Camera.allCameras;
        }

        public Camera[] GetAllCameras()
        {
            if (m_AllCameras.IsNullOrEmpty())
            {
                m_AllCameras = Camera.allCameras;
            }
            return m_AllCameras;
        }
        public Camera GetMainCamera()
        {
            return m_MainCamera;
        }


        public override string[] Commands()
        {
            throw new NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new NotImplementedException();
        }
    }
}
