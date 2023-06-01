﻿using CDOverhaul.Gameplay;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class OverhaulCameraController : OverhaulController
    {
        private Camera[] m_AllCameras;
        private Camera m_MainCamera;

        public bool IsMainCameraNull => !IsDisposedOrDestroyed() && m_MainCamera != null;
        public static Camera MechCameraPrefab => PlayerCameraManager.Instance.MechCameraTransformPrefab.GetComponentInChildren<Camera>();
        public static Camera BattleCruiserCameraPrefab => PlayerCameraManager.Instance.BattleCruiserCameraPrefab.GetComponentInChildren<Camera>();

        public override void Initialize()
        {
            _ = OverhaulEventsController.AddEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, RefreshVisibleCameras);
        }

        protected override void OnDisposed()
        {
            OverhaulEventsController.RemoveEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, RefreshVisibleCameras);
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
                m_AllCameras = Camera.allCameras;

            return m_AllCameras;
        }
        public Camera GetMainCamera() => m_MainCamera;

        public override string[] Commands() => null;
        public override string OnCommandRan(string[] command) => null;
    }
}
