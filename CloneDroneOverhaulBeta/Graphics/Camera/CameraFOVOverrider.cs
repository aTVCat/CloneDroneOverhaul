using CDOverhaul.Gameplay.Combat;
using CDOverhaul.HUD;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class CameraFOVOverrider : OverhaulBehaviour
    {
        public const float DEFAULT_VALUE = 0f;
        public const float FIRST_PERSON_DEFAULT_VALUE = 20f;

        public const float PAUSED_VALUE = -20f;

        public float DeltaTimeMultiplier
        {
            get;
            set;
        } = 12.5f;

        private bool m_OverrideFOV = true;
        public bool OverrideFOV
        {
            get => m_OverrideFOV && !m_PhotoManager.IsInPhotoMode();
            set => m_OverrideFOV = value;
        }

        private float m_CurrentFOV;

        private Camera m_Camera;
        private FirstPersonMover m_Owner;
        private TimeManager m_TimeManager;
        private PhotoManager m_PhotoManager;
        private UpgradeUI m_UpgradeUI;

        private LevelEditorCinematicCamera m_CinematicCamera;
        private bool m_CineCameraOn;
        private bool m_HasRefreshedCinematicCameraOnStart;

        public void SetUpReferences(FirstPersonMover newOwner)
        {
            m_UpgradeUI = GameUIRoot.Instance.UpgradeUI;
            m_PhotoManager = PhotoManager.Instance;
            m_TimeManager = TimeManager.Instance;
            m_Owner = newOwner;

            OverhaulEvents.AddEventListener<LevelEditorCinematicCamera>(GlobalEvents.CinematicCameraTurnedOn, OnCinematicCameraTurnedOn, true);
        }

        protected override void OnDisposed()
        {
            OverhaulEvents.RemoveEventListener<LevelEditorCinematicCamera>(GlobalEvents.CinematicCameraTurnedOn, OnCinematicCameraTurnedOn, true);
        }

        private void LateUpdate()
        {
            if (!OverrideFOV)
                return;

            if (!m_Owner)
            {
                base.enabled = false;
                return;
            }

            if (!m_Camera)
            {
                m_Camera = base.GetComponent<Camera>();
                if (!m_Camera)
                {
                    base.enabled = false;
                    return;
                }
            }

            if (!m_HasRefreshedCinematicCameraOnStart)
            {
                RefreshCinematicCameraOnStart();
                m_HasRefreshedCinematicCameraOnStart = true;
            }

            if (m_CineCameraOn)
            {
                if (!m_CinematicCamera || !m_CinematicCamera.HasTakenOverPlayerCamera())
                    OnCinematicCameraTurnedOff();

                if (m_CineCameraOn)
                    return;
            }

            float deltaTime = Time.unscaledDeltaTime * DeltaTimeMultiplier;
            m_CurrentFOV = Mathf.Lerp(m_CurrentFOV, GetTargetFoV(), deltaTime);
            m_Camera.fieldOfView += m_CurrentFOV;
        }

        public float GetTargetFoV()
        {
            return m_TimeManager.IsGamePaused() && !m_UpgradeUI.gameObject.activeSelf && OverhaulPauseMenu.UseZoom
                ? PAUSED_VALUE + ViewModesController.FOVOffset
                : (ViewModesController.IsFirstPersonModeEnabled ? FIRST_PERSON_DEFAULT_VALUE : DEFAULT_VALUE) + ViewModesController.FOVOffset + RobotCameraZoomExpansion.FOVOffset;
        }

        public void RefreshCinematicCameraOnStart()
        {
            if (!m_Camera || !m_Camera.transform.parent || !m_Camera.transform.parent.parent)
                return;

            Transform cinematicCameraTransform = m_Camera.transform.parent.parent;
            if (cinematicCameraTransform.gameObject.name.Contains("CinematicCamera"))
            {
                m_CinematicCamera = cinematicCameraTransform.GetComponent<LevelEditorCinematicCamera>();
                m_CineCameraOn = m_CinematicCamera;
            }
        }

        private void OnCinematicCameraTurnedOn(LevelEditorCinematicCamera cinematicCamera)
        {
            m_CinematicCamera = cinematicCamera;
            m_CineCameraOn = true;
        }

        private void OnCinematicCameraTurnedOff()
        {
            m_CinematicCamera = null;
            m_CineCameraOn = false;
        }
    }
}
