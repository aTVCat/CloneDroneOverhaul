using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class CameraFOVController : MonoBehaviour
    {
        [ModSetting(ModSettingsConstants.ENABLE_FOV_OVERRIDE, false)]
        public static bool EnableFOVOverride;

        [ModSetting(ModSettingsConstants.CAMERA_FOV_OFFSET, 0f)]
        public static float FOVOffset;

        private CameraManager m_cameraManager;

        private Animator m_cameraAnimator;

        private Camera m_camera;

        private FirstPersonMover m_owner;

        private void Start()
        {
            m_cameraManager = CameraManager.Instance;
            m_camera = base.GetComponent<Camera>();
            m_cameraAnimator = base.GetComponentInParent<Animator>();
        }

        private void LateUpdate()
        {
            if (!EnableFOVOverride)
                return;

            FirstPersonMover owner = m_owner;
            if (!owner || owner._isGrabbedForUpgrade)
                return;

            Animator animator = m_cameraAnimator;
            if (!animator || !animator.enabled /*|| (Time.timeScale <= 0f && animator.updateMode != AnimatorUpdateMode.UnscaledTime)*/)
                return;

            Camera camera = m_camera;
            if (!camera)
                return;

            CameraManager cameraManager = m_cameraManager;
            if (cameraManager.enableForceFOVOffset)
                camera.fieldOfView += cameraManager.forceFOVOffset;
            else
                camera.fieldOfView += FOVOffset;
        }

        public void SetOwner(FirstPersonMover firstPersonMover)
        {
            m_owner = firstPersonMover;
        }
    }
}
