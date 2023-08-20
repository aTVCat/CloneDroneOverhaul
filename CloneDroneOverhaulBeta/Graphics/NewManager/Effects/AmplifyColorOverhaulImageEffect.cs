using System;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class AmplifyColorOverhaulImageEffect : OverhaulImageEffectBehaviour
    {
        [OverhaulSetting("Graphics.Amplify color.Apply \"Film bright\" preset", OverhaulSettingsController.SettingEventDispatcherFlag, false, null, null)]
        public static OverhaulSettingWithEvent ApplyAmplifyColorPreset1 = new OverhaulSettingWithEvent();

        [OverhaulSetting("Graphics.Amplify color.Apply \"Film dark\" preset", OverhaulSettingsController.SettingEventDispatcherFlag, false, null, null)]
        public static OverhaulSettingWithEvent ApplyAmplifyColorPreset2 = new OverhaulSettingWithEvent();

        [OverhaulSetting("Graphics.Amplify color.Apply default preset", OverhaulSettingsController.SettingEventDispatcherFlag, false, null, null)]
        public static OverhaulSettingWithEvent ApplyAmplifyColorPresetDefault = new OverhaulSettingWithEvent();

        [OverhaulSettingDropdownParameters("Default@More Blend@More Exposure@More B+E@Less Blend@Less Exposure@Less B+E@+B -E@-B +E")]
        [OverhaulSetting("Graphics.Amplify color.Amplify color presetV2", 5, false)]
        public static int AmplifyColorMode;

        [OverhaulSettingDropdownParameters("Disabled@Photographic@FilmicACES")]
        [OverhaulSetting("Graphics.Amplify color.TonemapperV2", 2, false)]
        public static int AmplifyColorTonemapper;

        public override void PatchCamera(Camera camera)
        {
            base.PatchCamera(camera);
            if (!camera)
                return;

            AmplifyColorBase effect = camera.GetComponent<AmplifyColorBase>();
            if (!effect)
                return;

            switch (AmplifyColorTonemapper)
            {
                case 0:
                    effect.Tonemapper = AmplifyColor.Tonemapping.Disabled;
                    break;
                case 1:
                    effect.Tonemapper = AmplifyColor.Tonemapping.Photographic;
                    break;
                case 2:
                    effect.Tonemapper = AmplifyColor.Tonemapping.FilmicACES;
                    break;
            }

            LevelLightSettings activeLightSettings = null;
            if (LevelEditorLightManager.Instance != null)
                activeLightSettings = LevelEditorLightManager.Instance.GetActiveLightSettings();

            if (!activeLightSettings)
                return;

            effect.Exposure = activeLightSettings.CameraExposure;
            effect.BlendAmount = 1f - activeLightSettings.CameraColorBlend;
            switch (AmplifyColorMode)
            {
                case 1:
                    effect.BlendAmount = 1f;
                    break;
                case 2:
                    effect.Exposure = activeLightSettings.CameraExposure + 0.2f;
                    break;
                case 3:
                    effect.Exposure = activeLightSettings.CameraExposure + 0.2f;
                    effect.BlendAmount = 1f;
                    break;

                case 4:
                    effect.BlendAmount = 0f;
                    break;
                case 5:
                    effect.Exposure = Math.Max(activeLightSettings.CameraExposure - 0.2f, 0f);
                    break;
                case 6:
                    effect.Exposure = Math.Max(activeLightSettings.CameraExposure - 0.2f, 0f);
                    effect.BlendAmount = 0f;
                    break;
                case 7:
                    effect.Exposure = Math.Max(activeLightSettings.CameraExposure - 0.2f, 0f);
                    effect.BlendAmount = 1f;
                    break;
                case 8:
                    effect.Exposure = activeLightSettings.CameraExposure + 0.2f;
                    effect.BlendAmount = 0f;
                    break;
            }
        }
    }
}
