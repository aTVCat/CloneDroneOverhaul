using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class CameraFOVController : MonoBehaviour
    {
        public const float DEFAULT_SHORTENED_DISTANCE_ADDITION = 0.1f;

        public const float DEFAULT_FOV = 60f;

        public const float FOV_MAX_POSITIVE_OFFSET = 55f;

        [ModSetting(ModSettingsConstants.ENABLE_FOV_OVERRIDE, false)]
        public static bool EnableFOVOverride;

        [ModSetting(ModSettingsConstants.CAMERA_FOV_OFFSET, 0f)]
        public static float FOVOffset;

        private CameraManager _cameraManager;

        private Animator _cameraAnimator;

        private Camera _camera;

        private PlayerCameraMover _playerCameraMover;

        private FirstPersonMover _owner;

        private float _timeToAllowUnclampedFovUntil;

        private float _lerpedOffset;

        private void Start()
        {
            _timeToAllowUnclampedFovUntil = Time.time + 1f;
            _lerpedOffset = getFovOffset();
        }

        private void LateUpdate()
        {
            if (!EnableFOVOverride && !CameraManager.EnableFirstPersonMode)
                return;

            FirstPersonMover owner = _owner;
            if (!owner || owner._isGrabbedForUpgrade)
                return;

            PlayerCameraMover playerCameraMover = _playerCameraMover;
            if (!playerCameraMover || !playerCameraMover.enabled)
                return;

            Animator animator = _cameraAnimator;
            if (!animator || !animator.enabled)
                return;

            Camera camera = _camera;
            if (!camera)
                return;

            float expectedFovOffset = getFovOffset();
            _playerCameraMover.ShortenedDistanceAddition = Mathf.Lerp(DEFAULT_SHORTENED_DISTANCE_ADDITION, 0.7f, expectedFovOffset / FOV_MAX_POSITIVE_OFFSET);

            _lerpedOffset = Mathf.Lerp(_lerpedOffset, !GameModeManager.UsesMultiplayerSpawnPoints() || owner.HasConstructionFinished() ? expectedFovOffset : 0f, Time.unscaledDeltaTime * 9f);
            float finalFieldOfView = Mathf.Min(camera.fieldOfView + _lerpedOffset, Time.time < _timeToAllowUnclampedFovUntil ? 165f : 120f);

            camera.fieldOfView = finalFieldOfView;
        }

        public void Initialize(CameraManager cameraManager, Camera camera, Animator animator, PlayerCameraMover playerCameraMover, FirstPersonMover firstPersonMover)
        {
            _cameraManager = cameraManager;
            _camera = camera;
            _cameraAnimator = animator;
            _playerCameraMover = playerCameraMover;
            _owner = firstPersonMover;
        }

        private float getFovOffset()
        {
            bool fovOverrideEnabled = EnableFOVOverride;
            CameraManager cameraManager = _cameraManager;
            return cameraManager.enableForceFOVOffset ? cameraManager.forceFOVOffset : (CameraManager.EnableFirstPersonMode ? Mathf.Min(fovOverrideEnabled ? FOVOffset + 15f : 15f, FOV_MAX_POSITIVE_OFFSET) : (fovOverrideEnabled ? FOVOffset : 0));
        }
    }
}
