using CDOverhaul.Gameplay;
using CDOverhaul.HUD;
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
        #region Settings

        [OverhaulSettingAttribute("Graphics.Rendering.Deferred rendering", false, false, "Improve lightning\n(Many effects work incorrectly when this setting is enabled)")]
        public static bool DefferedRenderer;

        [OverhaulSetting("Graphics.Post effects.Set vanilla settings", SettingsController.SettingEventDispatcherFlag, false, null, null, null, null)]
        public static SettingEventDispatcher SetBloomVanilla = new SettingEventDispatcher();
        [OverhaulSettingAttribute("Graphics.Post effects.Enable bloom", true, false, "Make every light shine better", "Bloom.png")]
        public static bool BloomEnabled;
        [SettingSliderParameters(true, 1, 10)]
        [OverhaulSettingAttribute("Graphics.Post effects.Bloom iterations", 10, false, "How many times bloom effect should be applied?\n(Very low performance impact)", null, null, "Graphics.Post effects.Enable bloom")]
        public static int BloomIterations;
        [SettingSliderParameters(false, 0.1f, 2f)]
        [OverhaulSettingAttribute("Graphics.Post effects.Bloom intensity", 0.7f, false, null, null, null, "Graphics.Post effects.Enable bloom")]
        public static float BloomIntensity;
        [SettingSliderParameters(false, 0.85f, 2f)]
        [OverhaulSettingAttribute("Graphics.Post effects.Bloom Threshold", 1.25f, false, null, null, null, "Graphics.Post effects.Enable bloom")]
        public static float BloomThreshold;

        [OverhaulSettingAttribute("Graphics.Shaders.Vignette", true, false, "Shade screen edges")]
        public static bool VignetteEnabled;
        [SettingSliderParameters(false, -0.2f, 0.3f)]
        [OverhaulSettingAttribute("Graphics.Shaders.Vignette Intensity", 0.05f, false, null, null, null, "Graphics.Shaders.Vignette")]
        public static float VignetteIntensity;
        [OverhaulSettingAttribute("Graphics.Shaders.Blur edges", false, false, "I don't really like it, but you may turn this setting on for fun, I guess")]
        public static bool BlurEdgesEnabled;

        [OverhaulSettingAttribute("Graphics.Shaders.Chromatic Aberration", false, false, "Give things colored edges..?", "Chromatic Aberration.png")]
        public static bool ChromaticAberrationEnabled;
        [SettingSliderParameters(false, 0f, 0.001f)]
        [OverhaulSettingAttribute("Graphics.Shaders.Chromatic Aberration intensity", 0.0002f, false, null, null, null, "Graphics.Shaders.Chromatic Aberration")]
        public static float ChromaticAberrationIntensity;

        [OverhaulSettingAttribute("Graphics.Amplify Occlusion.Enable", true, false, "Add shadows to everything", "AmbientOcc.png")]
        public static bool AOEnabled;
        [SettingSliderParameters(false, 0.7f, 1.3f)]
        [OverhaulSettingAttribute("Graphics.Amplify Occlusion.Intensity", 0.95f, false, null, null, null, "Graphics.Amplify Occlusion.Enable")]
        public static float AOIntensity;
        [SettingDropdownParameters("Low@Medium@High@Very high")]
        [OverhaulSettingAttribute("Graphics.Amplify Occlusion.Sample Count", 1, false, null, null, null, "Graphics.Amplify Occlusion.Enable")]
        public static int AOSampleCount;

        [OverhaulSetting("Graphics.Amplify color.Apply \"Film bright\" preset", SettingsController.SettingEventDispatcherFlag, false, null, null, null, null)]
        public static SettingEventDispatcher ApplyAmplifyColorPreset1 = new SettingEventDispatcher();
        [OverhaulSetting("Graphics.Amplify color.Apply \"Film dark\" preset", SettingsController.SettingEventDispatcherFlag, false, null, null, null, null)]
        public static SettingEventDispatcher ApplyAmplifyColorPreset2 = new SettingEventDispatcher();
        [OverhaulSetting("Graphics.Amplify color.Apply default preset", SettingsController.SettingEventDispatcherFlag, false, null, null, null, null)]
        public static SettingEventDispatcher ApplyAmplifyColorPresetDefault = new SettingEventDispatcher();
        [SettingDropdownParameters("Default@More Blend@More Exposure@More B+E@Less Blend@Less Exposure@Less B+E@+B -E@-B +E")]
        [OverhaulSettingAttribute("Graphics.Amplify color.Amplify color preset", 0, false)]
        public static int AmplifyColorMode;
        [SettingDropdownParameters("Disabled@Photographic@FilmicACES")]
        [OverhaulSettingAttribute("Graphics.Amplify color.Tonemapper", 0, false)]
        public static int AmplifyColorTonemapper;
        [OverhaulSettingAttribute("Graphics.Amplify color.Enable depth mask usage", false, false)]
        public static bool AmplifyColorUseDepthMask;

        #endregion

        #region Some stuff

        private static readonly List<Bloom> m_BloomEffects = new List<Bloom>();
        private static readonly List<AmplifyOcclusionEffect> m_AOEffects = new List<AmplifyOcclusionEffect>();

        private static Material m_VignetteMaterial;
        private static Material m_ChromaMaterial;
        private static Material m_EdgeBlur;

        private static readonly Func<bool> m_EnableVignetteFunc = new System.Func<bool>(() => VignetteEnabled);
        private static readonly Func<bool> m_EnableCAFunc = new System.Func<bool>(() => ChromaticAberrationEnabled);
        private static readonly Func<bool> m_EnableBEFunc = new System.Func<bool>(() => BlurEdgesEnabled);

        private static bool m_ConfiguredEventButtons;

        public static bool DisallowChangeFPSLimit;

        public static OverhaulCameraController CameraController { get; private set; }

        private static readonly string[] m_IgnoredCameras = new string[]
        {
            "TitleScreenLogoCamera",
            "UICamera",
            "ArenaCamera"
        };

        public static bool IgnoreCamera(Camera camera) { return camera == null || m_IgnoredCameras.Contains(camera.gameObject.name); }

        #endregion

        public static void Initialize()
        {
            CameraController = OverhaulController.AddController<OverhaulCameraController>();
            _ = OverhaulEventsController.AddEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, PatchCamera);
            _ = OverhaulEventsController.AddEventListener(SettingsController.SettingChangedEventString, patchAllCameras);

            m_ChromaMaterial = AssetsController.GetAsset<Material>("M_IE_ChromaticAb", OverhaulAssetsPart.Part2);
            m_VignetteMaterial = AssetsController.GetAsset<Material>("M_IE_Spotlight", OverhaulAssetsPart.Part2);
            m_VignetteMaterial.SetFloat("_CenterY", -0.14f);
            m_EdgeBlur = AssetsController.GetAsset<Material>("M_SnapshotTest", OverhaulAssetsPart.Part2);
            patchAllCameras();

            if (!m_ConfiguredEventButtons)
            {
                m_ConfiguredEventButtons = true;
                SetBloomVanilla.EventAction = delegate
                {
                    BloomEnabled = true;
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Post effects.Enable bloom", true), true);
                    BloomIterations = 2;
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Post effects.Bloom iterations", true), 2);
                    BloomIntensity = 0.5f;
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Post effects.Bloom intensity", true), 0.5f);
                    BloomThreshold = 0.9f;
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Post effects.Bloom Threshold", true), 0.9f);

                    OverhaulParametersMenu menu = OverhaulController.GetController<OverhaulParametersMenu>();
                    if (menu != null && menu.gameObject.activeSelf)
                    {
                        menu.PopulateCategory(menu.SelectedCategory, true);
                    }
                };

                ApplyAmplifyColorPreset1.EventAction = delegate
                {
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Amplify color.Amplify color preset", true), 5);
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Amplify color.Tonemapper", true), 2);
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Amplify color.Enable depth mask usage", true), false);

                    OverhaulParametersMenu menu = OverhaulController.GetController<OverhaulParametersMenu>();
                    if (menu != null && menu.gameObject.activeSelf)
                    {
                        menu.PopulateCategory(menu.SelectedCategory, true);
                    }
                };

                ApplyAmplifyColorPreset2.EventAction = delegate
                {
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Amplify color.Amplify color preset", true), 2);
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Amplify color.Tonemapper", true), 1);
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Amplify color.Enable depth mask usage", true), true);

                    OverhaulParametersMenu menu = OverhaulController.GetController<OverhaulParametersMenu>();
                    if (menu != null && menu.gameObject.activeSelf)
                    {
                        menu.PopulateCategory(menu.SelectedCategory, true);
                    }
                };

                ApplyAmplifyColorPresetDefault.EventAction = delegate
                {
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Amplify color.Amplify color preset", true), 0);
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Amplify color.Tonemapper", true), 0);
                    SettingInfo.SavePref(SettingsController.GetSetting("Graphics.Amplify color.Enable depth mask usage", true), false);

                    OverhaulParametersMenu menu = OverhaulController.GetController<OverhaulParametersMenu>();
                    if (menu != null && menu.gameObject.activeSelf)
                    {
                        menu.PopulateCategory(menu.SelectedCategory, true);
                    }
                };
            }
        }

        public static void PatchCamera(Camera camera)
        {
            if (camera == null || camera.orthographic)
            {
                return;
            }
            if (!camera.name.Equals("TitleScreenLogoCamera")) camera.renderingPath = !DefferedRenderer ? RenderingPath.UsePlayerSettings : RenderingPath.DeferredShading;

            PatchBloom(camera.GetComponent<Bloom>());
            PatchAmplifyColor(camera.GetComponent<AmplifyColorBase>());
            refreshAmplifyOcclusionOnCamera(camera);
            addShaderPassesToCamera(camera);
            refreshShaderMaterials();

            //camera.gameObject.AddComponent<MotionBlur>().shader = AssetsController.GetAsset<Shader>("MotionBlur", OverhaulAssetsPart.Part2);
        }

        public static void PatchBloom(Bloom bloom)
        {
            if (bloom == null)
            {
                return;
            }

            bloom.bloomBlurIterations = BloomIterations;
            bloom.bloomIntensity = BloomIntensity;
            bloom.bloomThreshold = BloomThreshold;
            //bloom.bloomThresholdColor = new Color(1, 1, 0.75f, 1);
            if (!bloom.gameObject.name.Equals("ArenaCamera")) bloom.enabled = BloomEnabled;
            if (!m_BloomEffects.Contains(bloom)) m_BloomEffects.Add(bloom);
        }

        public static void PatchAmplifyColor(AmplifyColorBase effect)
        {
            if (effect == null)
            {
                return;
            }

            effect.UseDepthMask = AmplifyColorUseDepthMask;
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
            PatchAmplifyColorMode(effect);
        }

        public static void PatchAmplifyColorMode(AmplifyColorBase effect)
        {
            if (effect == null)
            {
                return;
            }
            LevelLightSettings activeLightSettings = null;
            if (LevelEditorLightManager.Instance != null)
            {
                activeLightSettings = LevelEditorLightManager.Instance.GetActiveLightSettings();
                if (activeLightSettings != null)
                {
                    effect.Exposure = activeLightSettings.CameraExposure;
                    effect.BlendAmount = 1f - activeLightSettings.CameraColorBlend;
                }
            }

            switch (AmplifyColorMode)
            {
                case 1:
                    effect.BlendAmount = 1f;
                    break;
                case 2:
                    if (activeLightSettings != null)
                    {
                        effect.Exposure = activeLightSettings.CameraExposure + 0.2f;
                    }
                    break;
                case 3:
                    if (activeLightSettings != null)
                    {
                        effect.Exposure = activeLightSettings.CameraExposure + 0.2f;
                    }
                    effect.BlendAmount = 1f;
                    break;

                case 4:
                    effect.BlendAmount = 0f;
                    break;
                case 5:
                    if (activeLightSettings != null)
                    {
                        effect.Exposure = Math.Max(activeLightSettings.CameraExposure - 0.2f, 0f);
                    }
                    break;
                case 6:
                    if (activeLightSettings != null)
                    {
                        effect.Exposure = Math.Max(activeLightSettings.CameraExposure - 0.2f, 0f);
                    }
                    effect.BlendAmount = 0f;
                    break;
                case 7:
                    if (activeLightSettings != null)
                    {
                        effect.Exposure = Math.Max(activeLightSettings.CameraExposure - 0.2f, 0f);
                    }
                    effect.BlendAmount = 1f;
                    break;
                case 8:
                    if (activeLightSettings != null)
                    {
                        effect.Exposure = activeLightSettings.CameraExposure + 0.2f;
                    }
                    effect.BlendAmount = 0f;
                    break;
            }
        }

        private static void addShaderPassesToCamera(Camera camera)
        {
            if (IgnoreCamera(camera) || camera.GetComponent<OverhaulPostProcessBehaviour>() != null)
            {
                return;
            }

            OverhaulPostProcessBehaviour.AddPostProcessEffect(camera, m_EdgeBlur, m_EnableBEFunc);
            OverhaulPostProcessBehaviour.AddPostProcessEffect(camera, m_ChromaMaterial, m_EnableCAFunc);
            OverhaulPostProcessBehaviour.AddPostProcessEffect(camera, m_VignetteMaterial, m_EnableVignetteFunc);
        }

        private static void refreshAmplifyOcclusionOnCamera(Camera camera, bool updateList = true)
        {
            if (IgnoreCamera(camera) || camera != Camera.main)
            {
                return;
            }

            AmplifyOcclusionEffect effect = camera.GetComponent<AmplifyOcclusionEffect>();
            if (!effect)
            {
                effect = camera.gameObject.AddComponent<AmplifyOcclusionEffect>();
                m_AOEffects.Add(effect);
            }
            refreshAmplifyOcclusion(effect, updateList);
        }

        private static void refreshAmplifyOcclusion(AmplifyOcclusionEffect effect, bool updateList = true)
        {
            effect.Bias = 0f;
            effect.BlurSharpness = 4f;
            effect.FilterResponse = 0.7f;
            effect.Bias = 0.2f;
            effect.SampleCount = (AmplifyOcclusion.SampleCountLevel)AOSampleCount;
            effect.Intensity = AOIntensity;
            effect.ApplyMethod = DefferedRenderer ? AmplifyOcclusionEffect.ApplicationMethod.Deferred : AmplifyOcclusionEffect.ApplicationMethod.PostEffect;
            effect.enabled = AOEnabled;

            // Remove destroyed instances
            if (!updateList || m_AOEffects.IsNullOrEmpty())
            {
                return;
            }

            int i = m_AOEffects.Count - 1;
            do
            {
                if (m_AOEffects[i] == null)
                {
                    m_AOEffects.RemoveAt(i);
                }
                i--;

            } while (i > -1);
        }


        private static void patchAllCameras()
        {
            refreshApplicationTargetFramerate();
            foreach (Camera cam in CameraController.GetAllCameras())
            {
                PatchCamera(cam);
            }
        }

        private static void refreshShaderMaterials()
        {
            if (!m_AOEffects.IsNullOrEmpty())
            {
                for (int i = m_AOEffects.Count - 1; i > -1; i--)
                {
                    AmplifyOcclusionEffect b = m_AOEffects[i];
                    if (!b)
                    {
                        m_AOEffects.RemoveAt(i);
                        continue;
                    }

                    refreshAmplifyOcclusion(b, false);
                }
            }
            if (!m_BloomEffects.IsNullOrEmpty())
            {
                for (int i = m_BloomEffects.Count - 1; i > -1; i--)
                {
                    Bloom b = m_BloomEffects[i];
                    if (!b)
                    {
                        m_BloomEffects.RemoveAt(i);
                        continue;
                    }

                    PatchBloom(b);
                }
            }
            if (m_VignetteMaterial != null)
            {
                m_VignetteMaterial.SetFloat("_Radius", Mathf.Clamp(0.35f - (VignetteIntensity * 0.1f), 0.01f, 0.5f));
            }
            if (m_ChromaMaterial != null)
            {
                m_ChromaMaterial.SetFloat("_RedX", -0.0007f - ChromaticAberrationIntensity);
                m_ChromaMaterial.SetFloat("_BlueX", 0.0007f + ChromaticAberrationIntensity);
            }
        }

        private static void refreshApplicationTargetFramerate()
        {
            try
            {
                DisallowChangeFPSLimit = true;
                SettingsManager.Instance.SetVsyncOn(false);
                DisallowChangeFPSLimit = false;
            }
            catch { }
            switch (CameraRollingBehaviour.TargetFPS)
            {
                case 1:
                    Application.targetFrameRate = 30;
                    break;
                case 2:
                    Application.targetFrameRate = 60;
                    try
                    {
                        DisallowChangeFPSLimit = true;
                        SettingsManager.Instance.SetVsyncOn(true);
                        DisallowChangeFPSLimit = false;
                    }
                    catch { }
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
                    Application.targetFrameRate = 240;
                    break;
                default:
                    Application.targetFrameRate = -1;
                    break;
            }
        }
    }
}
