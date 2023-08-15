using CDOverhaul.DevTools;
using CDOverhaul.Gameplay;
using ModLibrary;
using System.Diagnostics;
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

        [OverhaulSettingDropdownParameters("Unlimited@30@60@75@90@120@144@165@240")]
        [OverhaulSettingAttribute("Graphics.Settings.Target framerate", 2, false, "Limit maximum frames per second")]
        public static int TargetFPS;
        [OverhaulSetting("Graphics.Settings.VSync", false)]
        public static bool VSyncEnabled;

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

        private FirstPersonMover m_Owner;
        private Camera m_PlayerCamera;
        private Transform m_PlayerCameraTransform;
        private SettingsManager m_SettingsManager;

        private Vector3 m_TargetRotation;
        private float m_CursorMovementVelocityX;
        private float m_CursorMovementVelocityY;

        private LevelEditorCinematicCamera m_CinematicCamera;
        private bool m_CineCameraOn;
        private bool m_HasRefreshedCinematicCameraOnStart;

        public bool CanBeControlled => !IsDisposedOrDestroyed() && m_Owner && m_Owner.IsMainPlayer() && m_PlayerCamera && m_SettingsManager && !PhotoManager.Instance.IsInPhotoMode();
        public bool ForceZero => !EnableCameraRolling || !CanBeControlled || Cursor.visible || !m_Owner.IsMainPlayer() || m_Owner.IsAimingBow() || !m_Owner.IsPlayerInputEnabled();

        protected override void OnDisposed()
        {
            m_Owner = null;
            m_PlayerCamera = null;
            m_PlayerCameraTransform = null;
            m_SettingsManager = null;
            OverhaulEventsController.RemoveEventListener<Character>(GlobalEvents.CharacterKilled, onDied, true);
            OverhaulEventsController.RemoveEventListener<LevelEditorCinematicCamera>(GlobalEvents.CinematicCameraTurnedOn, OnCinematicCameraTurnedOn, true);
        }

        public void Initialize(FirstPersonMover firstPersonMover)
        {
            Camera playerCamera = base.GetComponent<Camera>();
            m_Owner = firstPersonMover;
            m_PlayerCamera = playerCamera;
            m_PlayerCameraTransform = playerCamera.transform;
            m_SettingsManager = SettingsManager.Instance;

            _ = OverhaulEventsController.AddEventListener<Character>(GlobalEvents.CharacterKilled, onDied, true);
            _ = OverhaulEventsController.AddEventListener<LevelEditorCinematicCamera>(GlobalEvents.CinematicCameraTurnedOn, OnCinematicCameraTurnedOn, true);
        }

        private void onDied(Character character)
        {
            if (character == null)
                return;

            if (m_Owner == null || Equals(m_Owner.GetInstanceID(), character.GetInstanceID()))
                DestroyBehaviour();
        }

        public void UpdateRotation(float targetX, float targetY, float targetZ)
        {
            Stopwatch stopwatch = OverhaulProfiler.StartTimer();
            if (CanBeControlled)
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
                float multiply = Multiplier * deltaTime * TiltMultiplier * 20f;

                float cursorX = forceZero ? 0f : Input.GetAxis("Mouse X") * multiply * (InvertAxis ? -1 : 1f);
                float cursorY = forceZero ? 0f : Input.GetAxis("Mouse Y") * (m_SettingsManager.GetInvertMouse() ? 1f : -1f) * multiply * (InvertAxis ? -1 : 1f);

                m_CursorMovementVelocityX = Mathf.Lerp(m_CursorMovementVelocityX, cursorX * 0.8f, deltaTimeMultiplied);
                m_CursorMovementVelocityY = Mathf.Lerp(m_CursorMovementVelocityY, cursorY * 0.8f, deltaTimeMultiplied);

                bool isOnFloorFirstPersonMode = !m_CineCameraOn && m_Owner.IsOnFloorFromKick() && !m_Owner.IsGettingUpFromKick() && ViewModesController.IsFirstPersonModeEnabled;
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

        private void LateUpdate()
        {
            if (!CanBeControlled)
                return;

            if (!m_HasRefreshedCinematicCameraOnStart)
            {
                RefreshCinematicCameraOnStart();
                m_HasRefreshedCinematicCameraOnStart = true;
            }

            if (m_CineCameraOn)
            {
                if (!m_CinematicCamera || !m_CinematicCamera.HasTakenOverPlayerCamera())
                    OnCinematicCameraTurnedOff();
            }

            Stopwatch stopwatch = OverhaulProfiler.StartTimer();
            float z = 0f;
            bool moveLeft = Input.GetKey(KeyCode.A);
            bool onlyRightLeg = m_Owner.IsDamaged(MechBodyPartType.RightLeg);
            bool moveRight = Input.GetKey(KeyCode.D);
            bool onlyLeftLeg = m_Owner.IsDamaged(MechBodyPartType.LeftLeg);
            if (!LockZ && xor(moveLeft, moveRight))
                z = moveLeft ? TiltMoveHorizontal : -TiltMoveHorizontal;
            if (!LockZ && TiltWhenOneLegged && xor(onlyLeftLeg, onlyRightLeg))
                z += onlyLeftLeg ? TiltToAddWhenOneLegged : -TiltToAddWhenOneLegged;

            float x = 0f;
            bool moveForward = Input.GetKey(KeyCode.W);
            bool moveBackward = Input.GetKey(KeyCode.S);
            if (!LockX && xor(moveForward, moveBackward))
                x = moveForward ? TiltMoveHorizontal : -TiltMoveHorizontal;
            if (!LockX && TiltWhenJumping && (m_Owner.IsJumping() || m_Owner.IsFreeFallingWithNoGroundInSight()))
                x += 1f;

            UpdateRotation(x + AdditionalXOffset, 0f, z + AdditionalZOffset);
            stopwatch.StopTimer("CameraRollingBehaviour.LateUpdate");
        }

        private static bool xor(bool a, bool b) => (a || b) && !(a && b);

        public static void UpdateViewBobbing()
        {
            Stopwatch stopwatch = OverhaulProfiler.StartTimer();
            if (CharacterTracker.Instance != null)
            {
                bool firstPerson = ViewModesController.IsFirstPersonModeEnabled;
                FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
                AdditionalOffsetMultiplier = player != null && player.GetPrivateField<bool>("_isMovingForward") ? (firstPerson ? 7f : 2.1f) : (firstPerson ? 3f : 0.6f);

                if (ViewModesController.IsFirstPersonModeEnabled)
                {
                    AdditionalXOffset = Mathf.Sin(Time.time * AdditionalOffsetMultiplier) * 0.7f;
                    AdditionalZOffset = Mathf.Sin((Time.time + 0.2f) * AdditionalOffsetMultiplier * 1.2f) * 0.4f;
                }
                else
                {
                    AdditionalXOffset = Mathf.Sin(Time.time * AdditionalOffsetMultiplier) * 0.4f;
                }
            }
            stopwatch.StopTimer("CameraRollingBehaviour.VB");
        }

        public void RefreshCinematicCameraOnStart()
        {
            if (!base.transform.parent || !base.transform.parent.parent)
                return;

            Transform cinematicCameraTransform = base.transform.parent.parent;
            if (cinematicCameraTransform.gameObject.name.Contains("CinematicCamera"))
            {
                m_CinematicCamera = cinematicCameraTransform.GetComponent<LevelEditorCinematicCamera>();
                m_CineCameraOn = m_CinematicCamera;
            }
        }

        public void OnCinematicCameraTurnedOn(LevelEditorCinematicCamera cam)
        {
            m_CinematicCamera = cam;
            m_CineCameraOn = true;
        }

        public void OnCinematicCameraTurnedOff()
        {
            m_CinematicCamera = null;
            m_CineCameraOn = false;
        }
    }
}