using OverhaulMod.Utils;
using Rewired;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class CameraRollingController : MonoBehaviour
    {
        public const float MULTIPLIER = 0.125f;
        public const float HORIZONTAL_TILT = 1.8f;
        public const float ONE_LEG_TILT = 2.6f;

        [ModSetting(ModSettingsConstants.ENABLE_CAMERA_BOBBING, true)]
        public static bool EnableBobbing;

        [ModSetting(ModSettingsConstants.ENABLE_CAMERA_ROLLING, true)]
        public static bool EnableRolling;

        public float AdditionalXOffset, AdditionalZOffset;

        private Camera _camera;
        private Transform _playerCameraTransform;
        private SettingsManager _settingsManager;
        private FirstPersonMover _owner;

        private Vector3 _rotation;
        private float _cursorMovementVelocityX, _cursorMovementVelocityY;

        public bool enableControl
        {
            get
            {
                FirstPersonMover owner = _owner;
                return owner && owner.IsPlayerCameraActive() && _settingsManager && !PhotoManager.Instance.IsInPhotoMode();
            }
        }

        public bool forceInitialRotation
        {
            get
            {
                if (!EnableRolling)
                    return true;

                FirstPersonMover owner = _owner;
                return Cursor.visible || !owner || owner.IsAimingBow() || owner.IsRidingOtherCharacter() || !owner.IsPlayerInputEnabled() || owner._isGrabbedForUpgrade;
            }
        }

        public void Initialize(Camera camera, FirstPersonMover firstPersonMover)
        {
            _settingsManager = SettingsManager.Instance;
            _playerCameraTransform = camera.transform;
            _owner = firstPersonMover;
            _camera = camera;
        }

        private void LateUpdate()
        {
            if (!_playerCameraTransform)
                return;

            if (!enableControl)
                return;

            bool forceZero = forceInitialRotation;

            FirstPersonMover firstPersonMover = _owner;
            float x = 0f;
            float z = 0f;
            if (!forceZero)
            {
                bool moveLeft = firstPersonMover._isMovingLeft;
                bool rightLegDamaged = firstPersonMover.IsDamaged(MechBodyPartType.RightLeg);
                bool moveRight = firstPersonMover._isMovingRight;
                bool leftLegDamaged = firstPersonMover.IsDamaged(MechBodyPartType.LeftLeg);
                if (getBool(moveLeft, moveRight))
                    z = moveLeft ? HORIZONTAL_TILT : -HORIZONTAL_TILT;
                if (getBool(leftLegDamaged, rightLegDamaged))
                    z += leftLegDamaged ? ONE_LEG_TILT : -ONE_LEG_TILT;

                bool moveForward = firstPersonMover._isMovingForward;
                bool moveBackward = firstPersonMover._isMovingBack;
                if (getBool(moveForward, moveBackward))
                    x = moveForward ? HORIZONTAL_TILT : -HORIZONTAL_TILT;
                if (firstPersonMover.IsJumping() || firstPersonMover.IsFreeFallingWithNoGroundInSight())
                    x += 1f;
            }

            if (_camera)
            {
                _camera.nearClipPlane = CameraManager.EnableFirstPersonMode ? 0.1f : 0.3f;
            }

            UpdateViewBobbing(forceZero);
            UpdateRotation(firstPersonMover, forceZero, x + AdditionalXOffset, 0f, z + AdditionalZOffset);
        }

        public void UpdateRotation(FirstPersonMover firstPersonMover, bool forceZero, float targetX, float targetY, float targetZ)
        {
            float deltaTime = Time.deltaTime;
            float deltaTimeMultiplied = deltaTime * 20f;
            float multiply = MULTIPLIER * deltaTime * 20f;

            Player player = ReInput.players.GetPlayer(0);
            if (player != null)
            {
                float ts = Mathf.Min(1f, Time.timeScale);
                float cursorX = forceZero ? 0f : player.GetAxis(7) * multiply;
                float cursorY = forceZero ? 0f : player.GetAxis(6) * (_settingsManager.GetInvertMouse() ? 1f : -1f) * multiply;

                _cursorMovementVelocityX = Mathf.Lerp(_cursorMovementVelocityX, cursorX * 0.8f, deltaTimeMultiplied) * ts;
                _cursorMovementVelocityY = Mathf.Lerp(_cursorMovementVelocityY, cursorY * 0.8f, deltaTimeMultiplied) * ts;
            }
            else
            {
                _cursorMovementVelocityX = 0f;
                _cursorMovementVelocityY = 0f;
            }

            bool isOnFloorFirstPersonMode = CameraManager.EnableFirstPersonMode && firstPersonMover.IsOnFloorFromKick() && !firstPersonMover.IsGettingUpFromKick();
            float limit = isOnFloorFirstPersonMode ? 90f : 10f;

            Vector3 newTargetRotation = _rotation;
            newTargetRotation.x = Mathf.Clamp(Mathf.Lerp(newTargetRotation.x, isOnFloorFirstPersonMode ? -60f : targetX, multiply) + _cursorMovementVelocityY, -limit, limit);
            newTargetRotation.y = Mathf.Clamp(Mathf.Lerp(newTargetRotation.y, targetY, multiply) + _cursorMovementVelocityX, -limit, limit);
            newTargetRotation.z = Mathf.Clamp(Mathf.Lerp(newTargetRotation.z, targetZ, multiply), -limit, limit);
            _rotation = newTargetRotation;

            if (!_owner._cameraHolderAnimator || !_owner._cameraHolderAnimator.enabled)
                return;

            _playerCameraTransform.localEulerAngles = newTargetRotation;
        }

        public void UpdateViewBobbing(bool forceZero)
        {
            if (!EnableBobbing || forceZero)
            {
                AdditionalXOffset = 0f;
                AdditionalZOffset = 0f;
                return;
            }

            FirstPersonMover owner = _owner;
            if (!owner)
            {
                AdditionalXOffset = 0f;
                AdditionalZOffset = 0f;
                return;
            }

            bool firstPerson = CameraManager.EnableFirstPersonMode;
            float time = Time.time;

            float multiplier = owner._isMovingForward || owner._isMovingRight || owner._isMovingLeft || owner._isMovingBack ? (firstPerson ? 8f : 1.75f) : (firstPerson ? 4f : 0.5f);
            if (firstPerson)
            {
                float sin = Mathf.Sin(time * multiplier);
                AdditionalXOffset = sin * 0.65f;
                AdditionalZOffset = sin * 0.3f;
            }
            else
            {
                AdditionalXOffset = 0f;
                AdditionalZOffset = 0f;
            }
        }

        private bool getBool(bool a, bool b) => (a || b) && !(a && b);
    }
}
