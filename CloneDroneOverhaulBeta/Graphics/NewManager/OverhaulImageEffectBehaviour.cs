using CDOverhaul.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class OverhaulImageEffectBehaviour : OverhaulBehaviour
    {
        public static readonly string[] IgnoredCameras = new string[]
        {
            "TitleScreenLogoCamera",
            "UICamera",
            "ArenaCamera"
        };

        protected Camera PreviousCamera;
        protected Camera CurrentCamera;

        private OverhaulCameraController m_CameraController;
        public OverhaulCameraController cameraController
        {
            get
            {
                if (!m_CameraController)
                    m_CameraController = OverhaulController.GetController<OverhaulCameraController>();

                return m_CameraController;
            }
        }

        public override void Start()
        {
            AddListeners(false);
            RefreshMainCamera();
        }

        protected override void OnDisposed()
        {
            AddListeners(true);
        }

        public void AddListeners(bool remove)
        {
            if (remove)
            {
                OverhaulEventsController.RemoveEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, internalPatchCamera);
                OverhaulEventsController.RemoveEventListener<Camera>(OverhaulGameplayCoreController.CurrentCameraSwitchedEventString, internalPatchCamera);
            }
            else
            {
                OverhaulEventsController.AddEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, internalPatchCamera);
                OverhaulEventsController.AddEventListener<Camera>(OverhaulGameplayCoreController.CurrentCameraSwitchedEventString, internalPatchCamera);
            }
        }

        private void internalPatchCamera(Camera camera)
        {
            if (camera && (camera.orthographic || IgnoredCameras.Contains(camera.gameObject.name)))
                return;

            PatchCamera(camera);
        }

        public virtual void PatchCamera(Camera camera)
        {
            PreviousCamera = CurrentCamera;
            CurrentCamera = camera;
        }

        public void RefreshMainCamera()
        {
            PreviousCamera = null;
            CurrentCamera = null;
            internalPatchCamera(cameraController.mainCamera);
        }

        public void Reload()
        {
            AddListeners(false);
            RefreshMainCamera();
        }
    }
}
