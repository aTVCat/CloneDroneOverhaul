using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class CameraRollingController : MonoBehaviour
    {
        public const float TILT = 1.8f;
        public const float ONE_LEG_TILT = 2.6f;

        [ModSetting(ModSettingsConstants.ENABLE_CAMERA_BOBBING, true)]
        public static bool EnableBobbing;

        [ModSetting(ModSettingsConstants.ENABLE_CAMERA_ROLLING, true)]
        public static bool EnableRolling;

        public static bool Invert = true;

        public float TiltApproachMultiplier = 3f;

        public float TiltRestoreMultiplier = 3f;

        public float CursorMovementMultiplier = 0.05f;

        private Camera _camera;
        private Transform _playerCameraTransform;
        private FirstPersonMover _owner;

        private Vector3 _rotation;
        private float _verticalCursorMovement, _horizontalCursorMovement;
        public float _verticalOffset;

        public bool EnableControl
        {
            get
            {
                FirstPersonMover owner = _owner;
                return owner && owner.IsPlayerCameraActive() && !PhotoManager.Instance.IsInPhotoMode();
            }
        }

        public bool ForceInitialRotation
        {
            get
            {
                if (!EnableRolling)
                    return true;

                FirstPersonMover owner = _owner;
                return Cursor.visible || !owner || owner.IsRidingOtherCharacter() || !owner.IsPlayerInputEnabled() || owner._isGrabbedForUpgrade;
            }
        }

        public void Initialize(Camera camera, FirstPersonMover firstPersonMover)
        {
            _playerCameraTransform = camera.transform;
            _owner = firstPersonMover;
            _camera = camera;
        }

        private void LateUpdate()
        {
            if (!_playerCameraTransform || !EnableControl)
                return;

            FirstPersonMover firstPersonMover = _owner;

            bool forceZero = ForceInitialRotation;
            bool isUsingBow = firstPersonMover.GetEquippedWeaponType() == WeaponType.Bow;
            float viewBobbingGlobalMultiplier = isUsingBow ? 0.3f : 1f;

            float verticalTilt = 0f;
            float horizontalTilt = 0f;
            if (!forceZero)
            {
                bool moveLeft = firstPersonMover._isMovingLeft;
                bool rightLegDamaged = firstPersonMover.IsDamaged(MechBodyPartType.RightLeg);
                bool moveRight = firstPersonMover._isMovingRight;
                bool leftLegDamaged = firstPersonMover.IsDamaged(MechBodyPartType.LeftLeg);
                if (getBool(moveLeft, moveRight))
                    horizontalTilt = moveLeft ? TILT : -TILT;
                if (getBool(leftLegDamaged, rightLegDamaged))
                    horizontalTilt += leftLegDamaged ? ONE_LEG_TILT : -ONE_LEG_TILT;

                bool moveForward = firstPersonMover._isMovingForward;
                bool moveBackward = firstPersonMover._isMovingBack;
                if (getBool(moveForward, moveBackward) && !isUsingBow) // make quick aiming with bow easier
                    verticalTilt = moveForward ? TILT : -TILT;
                if (firstPersonMover.IsJumping() || firstPersonMover.IsFreeFallingWithNoGroundInSight())
                    verticalTilt += 1f;
            }

            if (_camera)
            {
                _camera.nearClipPlane = CameraManager.EnableFirstPersonMode ? 0.1f : 0.3f;
            }

            UpdateViewBobbing(forceZero, viewBobbingGlobalMultiplier);
            UpdateRotation(firstPersonMover, forceZero, verticalTilt, 0f, horizontalTilt + _verticalOffset);
        }

        public void UpdateRotation(FirstPersonMover firstPersonMover, bool forceZero, float targetX, float targetY, float targetZ)
        {
            float deltaTime = Time.unscaledDeltaTime;
            float targetVerticalCursorMovement = forceZero ? 0f : (firstPersonMover._verticalCursorMovement * CursorMovementMultiplier);
            targetVerticalCursorMovement *= Invert ? 1f : -1f;
            float targetHorizontalCursorMovement = forceZero ? 0f : (firstPersonMover._horizontalCursorMovement * CursorMovementMultiplier);
            targetHorizontalCursorMovement *= Invert ? -1f : 1f;

            _verticalCursorMovement = approachValue(targetVerticalCursorMovement, _verticalCursorMovement, deltaTime * TiltApproachMultiplier);
            _horizontalCursorMovement = approachValue(targetHorizontalCursorMovement, _horizontalCursorMovement, deltaTime * TiltApproachMultiplier);

            bool isOnFloorFirstPersonMode = CameraManager.EnableFirstPersonMode && firstPersonMover.IsOnFloorFromKick() && !firstPersonMover.IsGettingUpFromKick();
            float limit = isOnFloorFirstPersonMode ? 90f : 10f;

            float deltaTimeMultiplied = deltaTime * TiltRestoreMultiplier;
            Vector3 newTargetRotation = _rotation;
            newTargetRotation.x = Mathf.Clamp(Mathf.Lerp(newTargetRotation.x, isOnFloorFirstPersonMode ? -60f : targetX, deltaTimeMultiplied) + _verticalCursorMovement, -limit, limit);
            newTargetRotation.y = Mathf.Clamp(Mathf.Lerp(newTargetRotation.y, targetY, deltaTimeMultiplied) + _horizontalCursorMovement, -limit, limit);
            newTargetRotation.z = Mathf.Clamp(Mathf.Lerp(newTargetRotation.z, targetZ, deltaTimeMultiplied), -limit, limit);
            _rotation = newTargetRotation;

            if (!_owner._cameraHolderAnimator || !_owner._cameraHolderAnimator.enabled) return;

            _playerCameraTransform.localEulerAngles = newTargetRotation;
        }

        public void UpdateViewBobbing(bool forceZero, float globalMultiplier)
        {
            if (!EnableBobbing || forceZero)
            {
                _verticalOffset = 0f;
                return;
            }

            FirstPersonMover owner = _owner;
            if (!owner)
            {
                _verticalOffset = 0f;
                return;
            }

            bool firstPerson = CameraManager.EnableFirstPersonMode;
            float time = Time.time;

            float multiplier = owner._isMovingForward || owner._isMovingRight || owner._isMovingLeft || owner._isMovingBack ? (firstPerson ? 8f : 1.5f) : (firstPerson ? 1f : 0.5f);
            if (firstPerson)
            {
                float sin = Mathf.Sin(time * multiplier * globalMultiplier);
                _verticalOffset = sin * 0.3f;
            }
            else
            {
                _verticalOffset = 0f;
            }
        }

        private float approachValue(float target, float current, float deltaTime)
        {
            if (current > target)
            {
                return Mathf.Max(target, current - deltaTime);
            }
            else if (current < target)
            {
                return Mathf.Min(target, current + deltaTime);
            }
            return current;
        }

        private bool getBool(bool a, bool b) => (a || b) && !(a && b);
    }
}