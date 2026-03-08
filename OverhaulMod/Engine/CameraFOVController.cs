using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class CameraFOVController : MonoBehaviour
    {
        public const float FOV_MAX_POSITIVE_OFFSET = 55F;

        [ModSetting(ModSettingsConstants.ENABLE_FOV_OVERRIDE, false)]
        public static bool EnableFOVOverride;

        [ModSetting(ModSettingsConstants.CAMERA_FOV_OFFSET, 0f)]
        public static float FOVOffset;

        private CameraManager _cameraManager;

        private Animator _cameraAnimator;

        private Camera _camera;

        private FirstPersonMover _owner;

        private float _timeToAllowUnclampedFovUntil;

        private float _lerpedOffset;

        // private bool _hasAddedEventListeners;

        private void Start()
        {
            //refreshReferences();
            _timeToAllowUnclampedFovUntil = Time.time + 1f;
            _lerpedOffset = getFovOffset();

            /*GlobalEventManager.Instance.AddEventListener(CameraManager.CINEMATIC_CAMERA_TURNED_OFF_EVENT, refreshReferences);
            _hasAddedEventListeners = true;*/
        }

        /*private void OnDestroy()
        {
            if (_hasAddedEventListeners)
            {
                GlobalEventManager.Instance.RemoveEventListener(CameraManager.CINEMATIC_CAMERA_TURNED_OFF_EVENT, refreshReferences);
            }
        }*/

        private void LateUpdate()
        {
            if (!EnableFOVOverride && !CameraManager.EnableFirstPersonMode)
                return;

            FirstPersonMover owner = _owner;
            if (!owner || owner._isGrabbedForUpgrade)
                return;

            Animator animator = _cameraAnimator;
            if (!animator || !animator.enabled /*|| (Time.timeScale <= 0f && animator.updateMode != AnimatorUpdateMode.UnscaledTime)*/)
                return;

            Camera camera = _camera;
            if (!camera)
                return;

            _lerpedOffset = Mathf.Lerp(_lerpedOffset, !GameModeManager.UsesMultiplayerSpawnPoints() || owner.HasConstructionFinished() ? getFovOffset() : 0f, Time.unscaledDeltaTime * 9f);
            camera.fieldOfView = Mathf.Min(camera.fieldOfView + _lerpedOffset, Time.time < _timeToAllowUnclampedFovUntil ? 165f : 110f);
        }

        /*private void refreshReferences()
        {
            if (!_cameraManager)
                _cameraManager = CameraManager.Instance;

            if (!_camera)
                _camera = base.GetComponent<Camera>();

            if (!_cameraAnimator)
                _cameraAnimator = base.GetComponentInParent<Animator>();
        }*/

        public void Initialize(CameraManager cameraManager, Camera camera, Animator animator, FirstPersonMover firstPersonMover)
        {
            _cameraManager = cameraManager;
            _camera = camera;
            _cameraAnimator = animator;
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
