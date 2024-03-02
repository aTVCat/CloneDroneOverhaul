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

        private Transform m_playerCameraTransform;
        private SettingsManager m_settingsManager;
        private FirstPersonMover m_owner;

        private Vector3 m_rotation;
        private float m_cursorMovementVelocityX, m_cursorMovementVelocityY;

        public bool enableControl
        {
            get
            {
                FirstPersonMover owner = m_owner;
                return owner && owner.IsPlayerCameraActive() && m_settingsManager && !PhotoManager.Instance.IsInPhotoMode();
            }
        }

        public bool forceInitialRotation
        {
            get
            {
                if (!EnableRolling)
                    return true;

                FirstPersonMover owner = m_owner;
                return Cursor.visible || !owner || owner.IsAimingBow() || !owner.IsPlayerInputEnabled();
            }
        }

        public void SetConfiguration(Camera camera, FirstPersonMover firstPersonMover)
        {
            m_settingsManager = SettingsManager.Instance;
            m_playerCameraTransform = camera.transform;
            m_owner = firstPersonMover;
        }

        private void LateUpdate()
        {
            if (!enableControl)
                return;

            bool forceZero = forceInitialRotation;

            FirstPersonMover firstPersonMover = m_owner;
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
            }

            float x = 0f;
            if (!forceZero)
            {
                bool moveForward = firstPersonMover._isMovingForward;
                bool moveBackward = firstPersonMover._isMovingBack;
                if (getBool(moveForward, moveBackward))
                    x = moveForward ? HORIZONTAL_TILT : -HORIZONTAL_TILT;
                if (firstPersonMover.IsJumping() || firstPersonMover.IsFreeFallingWithNoGroundInSight())
                    x += 1f;
            }

            UpdateViewBobbing();
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
                float cursorY = forceZero ? 0f : player.GetAxis(6) * (m_settingsManager.GetInvertMouse() ? 1f : -1f) * multiply;

                m_cursorMovementVelocityX = Mathf.Lerp(m_cursorMovementVelocityX, cursorX * 0.8f, deltaTimeMultiplied) * ts;
                m_cursorMovementVelocityY = Mathf.Lerp(m_cursorMovementVelocityY, cursorY * 0.8f, deltaTimeMultiplied) * ts;
            }
            else
            {
                m_cursorMovementVelocityX = 0f;
                m_cursorMovementVelocityY = 0f;
            }

            bool isOnFloorFirstPersonMode = CameraManager.EnableFirstPersonMode && firstPersonMover.IsOnFloorFromKick() && !firstPersonMover.IsGettingUpFromKick();
            float limit = isOnFloorFirstPersonMode ? 90f : 10f;

            Vector3 newTargetRotation = m_rotation;
            newTargetRotation.x = Mathf.Clamp(Mathf.Lerp(newTargetRotation.x, isOnFloorFirstPersonMode ? -60f : targetX, multiply) + m_cursorMovementVelocityY, -limit, limit);
            newTargetRotation.y = Mathf.Clamp(Mathf.Lerp(newTargetRotation.y, targetY, multiply) + m_cursorMovementVelocityX, -limit, limit);
            newTargetRotation.z = Mathf.Clamp(Mathf.Lerp(newTargetRotation.z, targetZ, multiply), -limit, limit);
            m_rotation = newTargetRotation;

            m_playerCameraTransform.localEulerAngles = newTargetRotation;
        }

        public void UpdateViewBobbing()
        {
            if (!EnableBobbing)
            {
                AdditionalXOffset = 0f;
                AdditionalZOffset = 0f;
                return;
            }

            FirstPersonMover owner = m_owner;
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
