using CDOverhaul.DevTools;
using System.Diagnostics;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class CameraTiltController : CameraControllerBase
    {
        public const float MULTIPLIER = 0.2f;
        public const float HORIZONTAL_TILT = 1.8f;
        public const float ONE_LEG_TILT = 2.6f;

        [OverhaulSetting("Graphics.Camera.Rolling", true, false, "The camera will tilt in the direction of the movement")]
        public static bool EnableCameraRolling;
        [OverhaulSetting("Graphics.Camera.Invert axis", false, false, null, "Graphics.Camera.Rolling")]
        public static bool InvertAxis;

        [OverhaulSetting("Graphics.Camera.Tilt when one legged", true, false, null, "Graphics.Camera.Rolling")]
        public static bool TiltWhenOneLegged;
        [OverhaulSetting("Graphics.Camera.Tilt when jumping", true, false, null, "Graphics.Camera.Rolling")]
        public static bool TiltWhenJumping;

        [OverhaulSetting("Graphics.Camera.Lock rotation by X", false, false, null, "Graphics.Camera.Rolling")]
        public static bool LockX;
        [OverhaulSetting("Graphics.Camera.Lock rotation by Z", false, false, null, "Graphics.Camera.Rolling")]
        public static bool LockZ;

        [OverhaulSettingSliderParameters(false, 0.7f, 1.2f)]
        [OverhaulSetting("Graphics.Camera.Tilt multiplier", 1f, false, null, "Graphics.Camera.Rolling")]
        public static float TiltMultiplier;

        private Transform m_PlayerCameraTransform;
        private SettingsManager m_SettingsManager;

        private Vector3 m_TargetRotation;
        private float m_CursorMovementVelocityX;
        private float m_CursorMovementVelocityY;

        public float AdditionalXOffset;
        public float AdditionalOffsetMultiplier = 0.6f;
        public float AdditionalZOffset;

        public bool CanBeControlled
        {
            get
            {
                FirstPersonMover owner = CameraOwner;
                return owner && owner.IsMainPlayer() && CameraReference && m_SettingsManager && !PhotoManager.Instance.IsInPhotoMode();
            }
        }

        public bool ForceZero
        {
            get
            {
                FirstPersonMover owner = CameraOwner;
                return !EnableCameraRolling || Cursor.visible || owner.IsAimingBow() || !owner.IsPlayerInputEnabled();
            }
        }

        public override void Start()
        {
            base.Start();
            m_SettingsManager = SettingsManager.Instance;
            m_PlayerCameraTransform = CameraReference.transform;
        }

        private void LateUpdate()
        {
            if (!CanBeControlled)
                return;

            FirstPersonMover firstPersonMover = CameraOwner;

            Stopwatch stopwatch = OverhaulProfiler.StartTimer();
            float z = 0f;
            bool moveLeft = firstPersonMover._isMovingLeft;
            bool rightLegDamaged = firstPersonMover.IsDamaged(MechBodyPartType.RightLeg);
            bool moveRight = firstPersonMover._isMovingRight;
            bool leftLegDamaged = firstPersonMover.IsDamaged(MechBodyPartType.LeftLeg);
            if (!LockZ)
            {
                if (getBool(moveLeft, moveRight))
                    z = moveLeft ? HORIZONTAL_TILT : -HORIZONTAL_TILT;
                if (TiltWhenOneLegged && getBool(leftLegDamaged, rightLegDamaged))
                    z += leftLegDamaged ? ONE_LEG_TILT : -ONE_LEG_TILT;
            }

            float x = 0f;
            bool moveForward = firstPersonMover._isMovingForward;
            bool moveBackward = firstPersonMover._isMovingBack;
            if (!LockX)
            {
                if (getBool(moveForward, moveBackward))
                    x = moveForward ? HORIZONTAL_TILT : -HORIZONTAL_TILT;
                if (TiltWhenJumping && (firstPersonMover.IsJumping() || firstPersonMover.IsFreeFallingWithNoGroundInSight()))
                    x += 1f;
            }

            UpdateViewBobbing();
            UpdateRotation(firstPersonMover, x + AdditionalXOffset, 0f, z + AdditionalZOffset);
            stopwatch.StopTimer("CameraRollingBehaviour.LateUpdate");
        }

        public void UpdateRotation(FirstPersonMover firstPersonMover, float targetX, float targetY, float targetZ)
        {
            Stopwatch stopwatch = OverhaulProfiler.StartTimer();
            if (CanBeControlled && !(CinematicCamera && CinematicCamera.LookAtPlayer))
            {
                bool forceZero = ForceZero;
                if (forceZero)
                {
                    targetX = 0f;
                    targetY = 0f;
                    targetZ = 0f;
                }

                float deltaTime = Time.unscaledDeltaTime;
                float deltaTimeMultiplied = deltaTime * 20f;
                float multiply = MULTIPLIER * deltaTime * TiltMultiplier * 20f;

                float cursorX = forceZero ? 0f : Input.GetAxis("Mouse X") * multiply * (InvertAxis ? -1 : 1f);
                float cursorY = forceZero ? 0f : Input.GetAxis("Mouse Y") * (m_SettingsManager.GetInvertMouse() ? 1f : -1f) * multiply * (InvertAxis ? -1 : 1f);

                m_CursorMovementVelocityX = Mathf.Lerp(m_CursorMovementVelocityX, cursorX * 0.8f, deltaTimeMultiplied);
                m_CursorMovementVelocityY = Mathf.Lerp(m_CursorMovementVelocityY, cursorY * 0.8f, deltaTimeMultiplied);

                bool isOnFloorFirstPersonMode = !IsCinematicCameraEnabled && firstPersonMover.IsOnFloorFromKick() && !firstPersonMover.IsGettingUpFromKick() && ViewModesManager.IsFirstPersonModeEnabled;
                float limit = isOnFloorFirstPersonMode ? 90f : 10f;

                Vector3 newTargetRotation = m_TargetRotation;
                newTargetRotation.x = Mathf.Clamp(Mathf.Lerp(newTargetRotation.x, isOnFloorFirstPersonMode ? -60f : targetX, multiply) + m_CursorMovementVelocityY, -limit, limit);
                newTargetRotation.y = Mathf.Clamp(Mathf.Lerp(newTargetRotation.y, targetY, multiply) + m_CursorMovementVelocityX, -limit, limit);
                newTargetRotation.z = Mathf.Clamp(Mathf.Lerp(newTargetRotation.z, targetZ, multiply), -limit, limit);
                m_TargetRotation = newTargetRotation;
                m_PlayerCameraTransform.localEulerAngles = newTargetRotation;
            }
            stopwatch.StopTimer("CameraRollingBehaviour.UpdateRotation");
        }

        public void UpdateViewBobbing()
        {
            Stopwatch stopwatch = OverhaulProfiler.StartTimer();
            bool firstPerson = ViewModesManager.IsFirstPersonModeEnabled;
            float time = Time.time;
            FirstPersonMover player = CameraOwner;

            AdditionalOffsetMultiplier = player && player._isMovingForward ? (firstPerson ? 7f : 2.1f) : (firstPerson ? 3f : 0.6f);
            if (ViewModesManager.IsFirstPersonModeEnabled)
            {
                AdditionalXOffset = Mathf.Sin(time * AdditionalOffsetMultiplier) * 0.7f;
                AdditionalZOffset = Mathf.Sin((time + 0.2f) * AdditionalOffsetMultiplier * 1.2f) * 0.4f;
            }
            else
            {
                AdditionalXOffset = Mathf.Sin(time * AdditionalOffsetMultiplier) * 0.4f;
            }
            stopwatch.StopTimer("CameraRollingBehaviour.VB");
        }

        private bool getBool(bool a, bool b) => (a || b) && !(a && b);
    }
}