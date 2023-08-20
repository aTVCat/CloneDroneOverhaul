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

namespace CDOverhaul.Graphics
{
    public static class OverhaulGraphicsController
    {
        public const string GenericSparksVFX = "GenericSparks";

        #region Settings

        [OverhaulSettingAttribute("Graphics.Rendering.Deferred renderingV2", false, false, "Improve lightning\n(Many effects work incorrectly when this setting is enabled)")]
        public static bool DeferredRenderer;

        [OverhaulSettingAttribute("Graphics.Shaders.Vignette", true, false, "Shade screen edges")]
        public static bool VignetteEnabled;
        [OverhaulSettingSliderParameters(false, -0.2f, 0.3f)]
        [OverhaulSettingAttribute("Graphics.Shaders.Vignette Intensity", 0.05f, false, null, "Graphics.Shaders.Vignette")]
        public static float VignetteIntensity;
        [OverhaulSettingAttribute("Graphics.Shaders.Blur edges", false, false, "I don't really like it, but you may turn this setting on for fun, I guess")]
        public static bool BlurEdgesEnabled;

        [OverhaulSettingAttribute("Graphics.Shaders.Chromatic Aberration", false, false, "Give things colored edges..?", "Chromatic Aberration.png")]
        public static bool ChromaticAberrationEnabled;
        [OverhaulSettingSliderParameters(false, 0f, 0.001f)]
        [OverhaulSettingAttribute("Graphics.Shaders.Chromatic Aberration intensity", 0.0002f, false, null, "Graphics.Shaders.Chromatic Aberration")]
        public static float ChromaticAberrationIntensity;

        #endregion

        #region Some stuff

        private static readonly List<Bloom> m_BloomEffects = new List<Bloom>();
        private static readonly List<AmplifyOcclusionEffect> m_AOEffects = new List<AmplifyOcclusionEffect>();

        private static Material s_VignetteMaterial;
        private static Material s_ChromaMaterial;
        private static Material s_EdgeBlur;

        private static readonly Func<bool> m_EnableVignetteFunc = new System.Func<bool>(() => VignetteEnabled);
        private static readonly Func<bool> m_EnableCAFunc = new System.Func<bool>(() => ChromaticAberrationEnabled);
        private static readonly Func<bool> m_EnableBEFunc = new System.Func<bool>(() => BlurEdgesEnabled);

        private static bool m_ConfiguredEventButtons;

        private static readonly string[] m_IgnoredCameras = new string[]
        {
            "TitleScreenLogoCamera",
            "UICamera",
            "ArenaCamera"
        };

        public static bool IgnoreCamera(Camera camera) => !camera || m_IgnoredCameras.Contains(camera.gameObject.name);

        #endregion

        public static void Initialize()
        {
            if(!PooledPrefabController.HasCreatedEntry(GenericSparksVFX))
                PooledPrefabController.CreateNewEntry<WeaponSkinCustomVFXInstance>(OverhaulAssetsController.GetAsset("VFX_Sparks", OverhaulAssetPart.Part2).transform, 10, GenericSparksVFX);

            OverhaulEventsController.AddEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, PatchCamera);
            OverhaulEventsController.AddEventListener(OverhaulSettingsController.SettingChangedEventString, PatchAllCameras);

            if (!s_ChromaMaterial)
            {
                s_ChromaMaterial = OverhaulAssetsController.GetAsset<Material>("M_IE_ChromaticAb", OverhaulAssetPart.Part2);
            }
            if (!s_VignetteMaterial)
            {
                s_VignetteMaterial = OverhaulAssetsController.GetAsset<Material>("M_IE_Spotlight", OverhaulAssetPart.Part2);
                s_VignetteMaterial.SetFloat("_CenterY", -0.14f);
            }
            if (!s_EdgeBlur)
            {
                s_EdgeBlur = OverhaulAssetsController.GetAsset<Material>("M_SnapshotTest", OverhaulAssetPart.Part2);
            }

            PatchAllCameras();
            RefreshLightsCount();

            if (!m_ConfiguredEventButtons)
            {
                m_ConfiguredEventButtons = true;
            }
        }

        public static void PatchCamera(Camera camera)
        {
            if (!camera || camera.orthographic)
                return;

            if (!camera.name.Equals("TitleScreenLogoCamera"))
                camera.renderingPath = RenderingPath.UsePlayerSettings;

            addImageEffectsToCamera(camera);
            refreshShaderMaterials();
        }   

        private static void addImageEffectsToCamera(Camera camera)
        {
            if (IgnoreCamera(camera) || camera.GetComponent<OverhaulImageEffect>() != null)
                return;

            if(s_EdgeBlur && m_EnableBEFunc != null)
                OverhaulImageEffect.AddEffect(camera, s_EdgeBlur, m_EnableBEFunc);

            if(s_ChromaMaterial && m_EnableCAFunc != null)
                OverhaulImageEffect.AddEffect(camera, s_ChromaMaterial, m_EnableCAFunc);

            if(s_VignetteMaterial && m_EnableVignetteFunc != null)
                OverhaulImageEffect.AddEffect(camera, s_VignetteMaterial, m_EnableVignetteFunc);
        }

        public static void PatchAllCameras()
        {
            refreshApplicationTargetFramerate();
        }

        private static void refreshShaderMaterials()
        {
            if (s_VignetteMaterial != null)
            {
                s_VignetteMaterial.SetFloat("_Radius", Mathf.Clamp(0.35f - (VignetteIntensity * 0.1f), 0.01f, 0.5f));
            }
            if (s_ChromaMaterial != null)
            {
                s_ChromaMaterial.SetFloat("_RedX", -0.0007f - ChromaticAberrationIntensity);
                s_ChromaMaterial.SetFloat("_BlueX", 0.0007f + ChromaticAberrationIntensity);
            }
        }

        private static void refreshApplicationTargetFramerate()
        {
            try
            {
                SettingsManager.Instance.SetVsyncOn(CameraRollingBehaviour.VSyncEnabled);
            }
            catch { }

            switch (CameraRollingBehaviour.TargetFPS)
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
            if (DelegateScheduler.Instance == null)
                return;

            DelegateScheduler.Instance.Schedule(refreshLightsCount, 0.2f);
        }

        private static void refreshLightsCount()
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
