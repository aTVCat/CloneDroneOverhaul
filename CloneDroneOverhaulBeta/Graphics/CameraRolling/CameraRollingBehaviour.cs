using ModLibrary;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class CameraRollingBehaviour : OverhaulBehaviour
    {
        public static readonly float Multiplier = 0.2f;

        public static readonly float TiltMoveHorizontal = 1.8f;
        public static readonly float TiltToAddWhenOneLegged = 2.6f;

        public static float AdditionalXOffset;
        public static float AdditionalOffsetMultiplier = 0.6f;
        public static float AdditionalZOffset;

        [SettingDropdownParameters("Unlimited@30@60@75@90@120@144@240")]
        [OverhaulSettingAttribute("Graphics.Settings.Target framerate", 2, false, "Limit maximum frames per second")]
        public static int TargetFPS;

        [OverhaulSetting("Graphics.Camera.Rolling", true, false, "The camera will tilt in the direction of the movement")]
        public static bool EnableCameraRolling;
        [OverhaulSetting("Graphics.Camera.Invert axis", false, false, null, null, null, "Graphics.Camera.Rolling")]
        public static bool InvertAxis;

        [OverhaulSetting("Graphics.Camera.Tilt when one legged", true, false, null, null, null, "Graphics.Camera.Rolling")]
        public static bool TiltWhenOneLegged;
        [OverhaulSetting("Graphics.Camera.Tilt when jumping", true, false, null, null, null, "Graphics.Camera.Rolling")]
        public static bool TiltWhenJumping;

        [OverhaulSetting("Graphics.Camera.Lock rotation by X", false, false, null, null, null, "Graphics.Camera.Rolling")]
        public static bool LockX;
        [OverhaulSetting("Graphics.Camera.Lock rotation by Z", false, false, null, null, null, "Graphics.Camera.Rolling")]
        public static bool LockZ;

        [SettingSliderParameters(false, 0.7f, 1.2f)]
        [OverhaulSetting("Graphics.Camera.Tilt multiplier", 1f, false, null, null, null, "Graphics.Camera.Rolling")]
        public static float TiltMultiplier;

        private FirstPersonMover m_Owner;
        private Camera m_PlayerCamera;
        private Transform m_PlayerCameraTransform;
        private SettingsManager m_SettingsManager;

        private Vector3 m_TargetRotation;
        private float m_CursorMovementVelocityX;
        private float m_CursorMovementVelocityY;

        public bool CanBeControlled => !IsDisposedOrDestroyed() /*&& PhotoManager.Instance != null ? PhotoManager.Instance.IsInPhotoMode() : true */&& m_Owner != null && m_PlayerCamera != null && m_SettingsManager != null;
        public bool ForceZero => !EnableCameraRolling || !CanBeControlled || Cursor.visible || !m_Owner.IsMainPlayer() || m_Owner.IsAimingBow() || !m_Owner.IsPlayerInputEnabled();

        protected override void OnDisposed()
        {
            m_Owner = null;
            m_PlayerCamera = null;
            m_PlayerCameraTransform = null;
            m_SettingsManager = null;
            OverhaulEventsController.RemoveEventListener<Character>(GlobalEvents.CharacterKilled, onDied, true);
        }

        public void Initialize(FirstPersonMover firstPersonMover, Camera playerCamera)
        {
            m_Owner = firstPersonMover;
            m_PlayerCamera = playerCamera;
            m_PlayerCameraTransform = playerCamera.transform;
            m_SettingsManager = SettingsManager.Instance;

            _ = OverhaulEventsController.AddEventListener<Character>(GlobalEvents.CharacterKilled, onDied, true);
        }

        private void onDied(Character character)
        {
            if (character == null)
            {
                return;
            }

            if (m_Owner == null || Equals(m_Owner.GetInstanceID(), character.GetInstanceID()))
            {
                DestroyBehaviour();
            }
        }

        public void UpdateRotation(float targetX, float targetY, float targetZ)
        {
            if (!CanBeControlled)
            {
                return;
            }

            if (ForceZero)
            {
                targetX = 0f;
                targetY = 0f;
                targetZ = 0f;
            }

            float curX = m_TargetRotation[0];
            float curY = m_TargetRotation[1];
            float curZ = m_TargetRotation[2];
            float multiply = Multiplier * Time.deltaTime * 15f * TiltMultiplier;

            float cursorX = ForceZero ? 0f : Input.GetAxis("Mouse X") * multiply * (InvertAxis ? -1 : 1f);
            float cursorY = ForceZero ? 0f : Input.GetAxis("Mouse Y") * (m_SettingsManager.GetInvertMouse() ? 1f : -1f) * multiply * (InvertAxis ? -1 : 1f);
            m_CursorMovementVelocityX = Mathf.Clamp(m_CursorMovementVelocityX + ((cursorX - m_CursorMovementVelocityX) * 0.5f), -15f, 15f);
            m_CursorMovementVelocityY = Mathf.Clamp(m_CursorMovementVelocityY + ((cursorY - m_CursorMovementVelocityY) * 0.5f), -15f, 15f);

            m_TargetRotation += new Vector3(((targetX - curX) * multiply) + m_CursorMovementVelocityY,
                ((targetY - curY) * multiply) + m_CursorMovementVelocityX,
                (targetZ - curZ) * multiply);
            m_PlayerCameraTransform.localEulerAngles = m_TargetRotation;
        }

        private void LateUpdate()
        {
            if (!CanBeControlled)
            {
                return;
            }

            float z = 0f;
            bool moveLeft = Input.GetKey(KeyCode.A);
            bool onlyRightLeg = m_Owner.IsDamaged(MechBodyPartType.RightLeg);
            bool moveRight = Input.GetKey(KeyCode.D);
            bool onlyLeftLeg = m_Owner.IsDamaged(MechBodyPartType.LeftLeg);
            if (!LockZ && XOR(moveLeft, moveRight))
            {
                z = moveLeft ? TiltMoveHorizontal : -TiltMoveHorizontal;
            }
            if (!LockZ && TiltWhenOneLegged && XOR(onlyLeftLeg, onlyRightLeg))
            {
                z += onlyLeftLeg ? -TiltToAddWhenOneLegged : TiltToAddWhenOneLegged;
            }

            float x = 0f;
            bool moveForward = Input.GetKey(KeyCode.W);
            bool moveBackward = Input.GetKey(KeyCode.S);
            if (!LockX && XOR(moveForward, moveBackward))
            {
                x = moveForward ? TiltMoveHorizontal : -TiltMoveHorizontal;
            }
            if (!LockX && TiltWhenJumping && (m_Owner.IsJumping() || m_Owner.IsFreeFallingWithNoGroundInSight()))
            {
                x += 1f;
            }

            UpdateRotation(x + AdditionalXOffset, 0f, z + AdditionalZOffset);
        }

        private static bool XOR(bool a, bool b)
        {
            return (a || b) && !(a && b);
        }

        public static void UpdateViewBobbing()
        {
            if (CharacterTracker.Instance == null) return;
            FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
            if (player != null && player.GetPrivateField<bool>("_isMovingForward"))
            {
                AdditionalOffsetMultiplier = 2.1f;
            }
            else
            {
                AdditionalOffsetMultiplier = 0.6f;
            }
            AdditionalXOffset = Mathf.Sin(Time.time * AdditionalOffsetMultiplier) * 0.4f;
            AdditionalZOffset = Mathf.Sin((Time.time + 0.2f) * AdditionalOffsetMultiplier * 1.2f) * 0.5f;
        }
    }
}