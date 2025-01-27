﻿using OverhaulMod.Utils;
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

        private float m_timeToAllowUnclampedFovUntil;

        private float m_lerpedOffset;

       // private bool m_hasAddedEventListeners;

        private void Start()
        {
            //refreshReferences();
            m_timeToAllowUnclampedFovUntil = Time.time + 1f;
            m_lerpedOffset = getFovOffset();

            /*GlobalEventManager.Instance.AddEventListener(CameraManager.CINEMATIC_CAMERA_TURNED_OFF_EVENT, refreshReferences);
            m_hasAddedEventListeners = true;*/
        }

        /*private void OnDestroy()
        {
            if (m_hasAddedEventListeners)
            {
                GlobalEventManager.Instance.RemoveEventListener(CameraManager.CINEMATIC_CAMERA_TURNED_OFF_EVENT, refreshReferences);
            }
        }*/

        private void LateUpdate()
        {
            if (!EnableFOVOverride && !CameraManager.EnableFirstPersonMode)
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

            m_lerpedOffset = Mathf.Lerp(m_lerpedOffset, !GameModeManager.UsesMultiplayerSpawnPoints() || owner.HasConstructionFinished() ? getFovOffset() : 0f, Time.unscaledDeltaTime * 9f);
            camera.fieldOfView = Mathf.Min(camera.fieldOfView + m_lerpedOffset, Time.time < m_timeToAllowUnclampedFovUntil ? 165f : 110f);
        }

        /*private void refreshReferences()
        {
            if (!m_cameraManager)
                m_cameraManager = CameraManager.Instance;

            if (!m_camera)
                m_camera = base.GetComponent<Camera>();

            if (!m_cameraAnimator)
                m_cameraAnimator = base.GetComponentInParent<Animator>();
        }*/

        public void Initialize(CameraManager cameraManager, Camera camera, Animator animator, FirstPersonMover firstPersonMover)
        {
            m_cameraManager = cameraManager;
            m_camera = camera;
            m_cameraAnimator = animator;
            m_owner = firstPersonMover;
        }

        private float getFovOffset()
        {
            bool fovOverrideEnabled = EnableFOVOverride;
            CameraManager cameraManager = m_cameraManager;
            return cameraManager.enableForceFOVOffset ? cameraManager.forceFOVOffset : (CameraManager.EnableFirstPersonMode ? Mathf.Min(fovOverrideEnabled ? FOVOffset + 15f : 15f, 40f) : (fovOverrideEnabled ? FOVOffset : 0));
        }
    }
}
