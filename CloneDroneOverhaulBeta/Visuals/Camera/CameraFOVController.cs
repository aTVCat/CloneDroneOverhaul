using CDOverhaul.Gameplay.Combat;
using CDOverhaul.HUD;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class CameraFOVController : CameraControllerBase
    {
        public const float DEFAULT_VALUE = 0f;
        public const float FIRST_PERSON_DEFAULT_VALUE = 20f;
        public const float PAUSED_VALUE = -20f;
        public const float MULTIPLIER = 12.5f;

        private TimeManager m_TimeManager;
        private PhotoManager m_PhotoManager;
        private UpgradeUI m_UpgradeUI;

        private float m_CurrentFOV;

        private bool m_OverrideFOV = true;
        public bool OverrideFOV
        {
            get => m_OverrideFOV && !m_PhotoManager.IsInPhotoMode();
            set => m_OverrideFOV = value;
        }

        public override void Start()
        {
            base.Start();
            m_UpgradeUI = GameUIRoot.Instance.UpgradeUI;
            m_PhotoManager = PhotoManager.Instance;
            m_TimeManager = TimeManager.Instance;
        }

        private void LateUpdate()
        {
            if (!OverrideFOV || IsCinematicCameraEnabled)
                return;

            FirstPersonMover m_Owner = CameraOwner;
            Camera camera = CameraReference;
            if (!m_Owner || !camera)
            {
                DestroyBehaviour();
                return;
            }

            float deltaTime = Time.unscaledDeltaTime * MULTIPLIER;
            m_CurrentFOV = Mathf.Lerp(m_CurrentFOV, GetTargetFoV(), deltaTime);
            camera.fieldOfView += m_CurrentFOV;
        }

        public float GetTargetFoV()
        {
            return m_TimeManager.IsGamePaused() && !m_UpgradeUI.gameObject.activeSelf && OverhaulPauseMenu.UseZoom
                ? PAUSED_VALUE + ViewModesSystem.FOVOffset
                : (ViewModesSystem.IsFirstPersonModeEnabled ? FIRST_PERSON_DEFAULT_VALUE : DEFAULT_VALUE) + ViewModesSystem.FOVOffset + RobotCameraZoomExpansion.FOVOffset;
        }
    }
}
