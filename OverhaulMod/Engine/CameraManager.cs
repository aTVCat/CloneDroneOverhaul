﻿using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class CameraManager : Singleton<CameraManager>
    {
        public const string CINEMATIC_CAMERA_TURNED_OFF_EVENT = "CinematicCameraTurnedOff";

        public const string FIRST_PERSON_MODE_SWITCHED_EVENT = "FirstPersonModeSwitched";

        [ModSetting(ModSettingsConstants.ENABLE_FIRST_PERSON_MODE, false)]
        public static bool EnableFirstPersonMode;

        [ModSetting(ModSettingsConstants.CAMERA_MODE_TOGGLE_KEYBIND, KeyCode.Y)]
        public static KeyCode CameraModeToggleKeyBind;

        public bool isCameraControlledByCutscene
        {
            get;
            private set;
        }

        public bool enableThirdPerson
        {
            get;
            set;
        }

        public bool enableForceFOVOffset
        {
            get;
            set;
        }

        public float forceFOVOffset
        {
            get;
            set;
        }

        public Camera mainCamera
        {
            get;
            private set;
        }

        private void Start()
        {
            GlobalEventManager.Instance.AddEventListener(GlobalEvents.CinematicCameraTurnedOn, onCinematicCameraEnabled);
            GlobalEventManager.Instance.AddEventListener(CINEMATIC_CAMERA_TURNED_OFF_EVENT, onCinematicCameraDisabled);
        }

        private void OnDestroy()
        {
            GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.CinematicCameraTurnedOn, onCinematicCameraEnabled);
            GlobalEventManager.Instance.RemoveEventListener(CINEMATIC_CAMERA_TURNED_OFF_EVENT, onCinematicCameraDisabled);
        }

        private void onCinematicCameraEnabled()
        {
            isCameraControlledByCutscene = true;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        private void onCinematicCameraDisabled()
        {
            isCameraControlledByCutscene = false;
            PostEffectsManager.Instance.RefreshCameraPostEffects();
        }

        public void ResetCameraRect()
        {
            SetCameraRect(new Rect(0f, 0f, Screen.width, Screen.height));
        }

        public void SetCameraRect(Rect rect)
        {
            Camera camera = mainCamera;
            if (!camera)
                return;

            camera.rect = rect;
        }

        public void ResetCameraHolderPosition(FirstPersonMover firstPersonMover = null)
        {
            if (!firstPersonMover)
            {
                firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
                if (!firstPersonMover)
                    return;
            }

            SetCameraHolderPosition(firstPersonMover._cameraHolderDefaultPosition, firstPersonMover);
        }

        public void SetCameraHolderPosition(Vector3 position, FirstPersonMover firstPersonMover = null)
        {
            if (!firstPersonMover)
            {
                firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
                if (!firstPersonMover)
                    return;
            }

            if (!firstPersonMover.IsAlive())
                return;

            Transform transform = firstPersonMover._cameraHolderTransform;
            if (!transform)
                return;

            transform.localPosition = position;
        }

        public void ResetCameraHolderEulerAngles(FirstPersonMover firstPersonMover = null)
        {
            if (!firstPersonMover)
            {
                firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
                if (!firstPersonMover)
                    return;
            }

            if (!firstPersonMover.IsAlive())
                return;

            SetCameraHolderEulerAngles(Vector3.zero, firstPersonMover);
        }

        public void SetCameraHolderEulerAngles(Vector3 eulerAngles, FirstPersonMover firstPersonMover = null)
        {
            if (!firstPersonMover)
            {
                firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
                if (!firstPersonMover)
                    return;
            }

            Transform transform = firstPersonMover._cameraHolderTransform;
            if (!transform)
                return;

            transform.localEulerAngles = eulerAngles;
        }

        public Vector3 GetCameraHolderEulerAngles(FirstPersonMover firstPersonMover = null)
        {
            if (!firstPersonMover)
            {
                firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
                if (!firstPersonMover)
                    return Vector3.zero;
            }

            Transform transform = firstPersonMover._cameraHolderTransform;
            if (!transform)
                return Vector3.zero;

            return transform.localEulerAngles;
        }

        public void SetCameraReducedWidth(float value, bool alignToRight)
        {
            Camera camera = mainCamera;
            if (!camera)
                return;

            value = Mathf.Abs(value);
            Rect rect = camera.pixelRect;
            if (alignToRight)
                rect.xMin = value;
            else
                rect.xMin = -value;

            rect.width = Screen.width - value;
        }

        public void AddControllers(Camera camera, FirstPersonMover firstPersonMover)
        {
            PlayerCameraMover playerCameraMover = camera.GetComponent<PlayerCameraMover>();
            if (!playerCameraMover)
                return;

            Animator animator = firstPersonMover._cameraHolderAnimator;
            if (!animator)
                return;

            CameraFOVController cameraFovController = camera.GetComponent<CameraFOVController>();
            if (!cameraFovController)
            {
                cameraFovController = camera.gameObject.AddComponent<CameraFOVController>();
            }
            cameraFovController.Initialize(this, camera, animator, firstPersonMover);

            CameraModeController cameraModeController = camera.GetComponent<CameraModeController>();
            if (!cameraModeController)
            {
                cameraModeController = camera.gameObject.AddComponent<CameraModeController>();
            }
            cameraModeController.Initialize(this, firstPersonMover, playerCameraMover, animator);

            CameraRollingController cameraRollingController = camera.GetComponent<CameraRollingController>();
            if (!cameraRollingController)
            {
                cameraRollingController = camera.gameObject.AddComponent<CameraRollingController>();
            }
            cameraRollingController.Initialize(camera, firstPersonMover);
        }

        private void Update()
        {
            Camera oldCamera = mainCamera;
            Camera camera = Camera.main;
            if (camera != oldCamera)
            {
                mainCamera = camera;
                ModCore.TriggerOnCameraSwitchedEvent(oldCamera, camera);
            }

            bool isKeyDown = Input.GetKeyDown(CameraModeToggleKeyBind);
            if (!isKeyDown)
                return;

            if (ModGameUtils.IsGamePausedOrCursorVisible())
                return;

            bool value = !EnableFirstPersonMode;
            ModSettingsManager.SetBoolValue(ModSettingsConstants.ENABLE_FIRST_PERSON_MODE, value);

            EnergyUI energyUI = ModCache.gameUIRoot?.EnergyUI;
            if (energyUI)
            {
                if (value)
                {
                    energyUI.SetErrorLabelVisible(LocalizationManager.Instance.GetTranslatedString("fpm_enabled"));
                }
                else
                {
                    energyUI.SetErrorLabelVisible(LocalizationManager.Instance.GetTranslatedString("fpm_disabled"));
                }
            }
        }
    }
}
