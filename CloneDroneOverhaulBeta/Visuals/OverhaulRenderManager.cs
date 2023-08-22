using CDOverhaul.Device;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.QualityOfLife;
using CDOverhaul.HUD;
using OverhaulAPI;
using OverhaulAPI.SharedMonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace CDOverhaul.Visuals
{
    public class OverhaulRenderManager : OverhaulManager<OverhaulRenderManager>
    {
        [OverhaulSettingDropdownParameters("Unlimited@30@60@75@90@120@144@165@240")]
        [OverhaulSetting("Graphics.Settings.Target framerate", 2, false, "Limit maximum frames per second")]
        public static int TargetFPS;

        [OverhaulSetting("Graphics.Settings.VSync", false)]
        public static bool VSyncEnabled;

        public override void Initialize()
        {
            RefreshLightsCount();
            RefreshFrameRate();
            OverhaulEventsController.AddEventListener(OverhaulSettingsController.SettingChangedEventString, RefreshFrameRate);
            OverhaulDebug.Log("RenderManager initialized", EDebugType.ModInit);
        }

        public override void OnSceneReloaded()
        {
            OverhaulEventsController.AddEventListener(OverhaulSettingsController.SettingChangedEventString, RefreshFrameRate);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            OverhaulEventsController.RemoveEventListener(OverhaulSettingsController.SettingChangedEventString, RefreshFrameRate);
        }

        public static void RefreshFrameRate()
        {
            if (SettingsManager.Instance && SettingsManager.Instance._data != null)
                SettingsManager.Instance.SetVsyncOn(VSyncEnabled);

            switch (TargetFPS)
            {
                case 1:
                    Application.targetFrameRate = 30;
                    break;
                case 2:
                    Application.targetFrameRate = 60;
                    break;
                case 3:
                    Application.targetFrameRate = 75;
                    break;
                case 4:
                    Application.targetFrameRate = 90;
                    break;
                case 5:
                    Application.targetFrameRate = 120;
                    break;
                case 6:
                    Application.targetFrameRate = 144;
                    break;
                case 7:
                    Application.targetFrameRate = 165;
                    break;
                case 8:
                    Application.targetFrameRate = 240;
                    break;
                default:
                    Application.targetFrameRate = -1;
                    break;
            }
        }

        public static void RefreshLightsCount()
        {
            switch (QualitySettings.GetQualityLevel())
            {
                case 2:
                    QualitySettings.pixelLightCount = 18;
                    break;
                case 1:
                    QualitySettings.pixelLightCount = 6;
                    break;
                default:
                    QualitySettings.pixelLightCount = 2;
                    break;
            }
        }
    }
}
