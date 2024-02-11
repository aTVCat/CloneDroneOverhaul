using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class CameraManager : Singleton<CameraManager>
    {
        [ModSetting(ModSettingConstants.ENABLE_FIRST_PERSON_MODE, false)]
        public static bool EnableFirstPersonMode;

        [ModSetting(ModSettingConstants.CAMERA_MODE_TOGGLE_KEYBIND, KeyCode.Y)]
        public static KeyCode CameraModeToggleKeyBind;

        public bool refreshCameraMoverNextFrame
        {
            get;
            set;
        }

        public void AddControllers(Camera camera, FirstPersonMover firstPersonMover)
        {
            if (!camera || !firstPersonMover)
                return;

            CameraModeController cameraModeController = camera.GetComponent<CameraModeController>();
            if (!cameraModeController)
            {
                cameraModeController = camera.gameObject.AddComponent<CameraModeController>();
            }
            cameraModeController.SetOwner(firstPersonMover);

            CameraRollingController cameraRollingController = camera.GetComponent<CameraRollingController>();
            if (!cameraRollingController)
            {
                cameraRollingController = camera.gameObject.AddComponent<CameraRollingController>();
            }
            cameraRollingController.SetConfiguration(camera, firstPersonMover);
        }

        private void Update()
        {
            bool isKeyDown = Input.GetKeyDown(CameraModeToggleKeyBind);
            if (!isKeyDown)
                return;

            bool value = !EnableFirstPersonMode;
            ModSettingsManager.SetBoolValue(ModSettingConstants.ENABLE_FIRST_PERSON_MODE, value);

            EnergyUI energyUI = ModCache.gameUIRoot?.EnergyUI;
            if (energyUI)
            {
                string word = value ? "Enabled" : "Disabled";
                energyUI.SetErrorLabelVisible($"{word} first person mode");
            }

            refreshCameraMoverNextFrame = true;
        }
    }
}
