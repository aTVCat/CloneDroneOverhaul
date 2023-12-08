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

        [OverhaulSetting("Graphics.Post effects.Set vanilla settings", OverhaulSettingsController.SettingEventDispatcherFlag, false, null, null)]
        public static OverhaulSettingWithEvent SetBloomVanilla = new OverhaulSettingWithEvent();
        [OverhaulSettingAttribute("Graphics.Post effects.Enable bloom", true, false, "Make every light shine better", "Bloom.png")]
        public static bool BloomEnabled;
        [OverhaulSettingSliderParameters(true, 1, 10)]
        [OverhaulSettingAttribute("Graphics.Post effects.Bloom iterations", 10, false, "How many times bloom effect should be applied?\n(Very low performance impact)", "Graphics.Post effects.Enable bloom")]
        public static int BloomIterations;
        [OverhaulSettingSliderParameters(false, 0.1f, 2f)]
        [OverhaulSettingAttribute("Graphics.Post effects.Bloom intensity", 0.7f, false, null, "Graphics.Post effects.Enable bloom")]
        public static float BloomIntensity;
        [OverhaulSettingSliderParameters(false, 0.85f, 2f)]
        [OverhaulSettingAttribute("Graphics.Post effects.Bloom Threshold", 1.25f, false, null, "Graphics.Post effects.Enable bloom")]
        public static float BloomThreshold;

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

        [OverhaulSettingAttribute("Graphics.Amplify Occlusion.Enable", true, false, "Add shadows to everything", "AmbientOcc.png")]
        public static bool AOEnabled;
        [OverhaulSettingSliderParameters(false, 0.6f, 1.3f)]
        [OverhaulSettingAttribute("Graphics.Amplify Occlusion.Intensity", 0.65f, false, null, "Graphics.Amplify Occlusion.Enable")]
        public static float AOIntensity;
        [OverhaulSettingDropdownParameters("Low@Medium@High@Very high")]
        [OverhaulSettingAttribute("Graphics.Amplify Occlusion.Sample Count", 1, false, null, "Graphics.Amplify Occlusion.Enable")]
        public static int AOSampleCount;

        [OverhaulSetting("Graphics.Amplify color.Apply \"Film bright\" preset", OverhaulSettingsController.SettingEventDispatcherFlag, false, null, null)]
        public static OverhaulSettingWithEvent ApplyAmplifyColorPreset1 = new OverhaulSettingWithEvent();
        [OverhaulSetting("Graphics.Amplify color.Apply \"Film dark\" preset", OverhaulSettingsController.SettingEventDispatcherFlag, false, null, null)]
        public static OverhaulSettingWithEvent ApplyAmplifyColorPreset2 = new OverhaulSettingWithEvent();
        [OverhaulSetting("Graphics.Amplify color.Apply default preset", OverhaulSettingsController.SettingEventDispatcherFlag, false, null, null)]
        public static OverhaulSettingWithEvent ApplyAmplifyColorPresetDefault = new OverhaulSettingWithEvent();
        [OverhaulSettingDropdownParameters("Default@More Blend@More Exposure@More B+E@Less Blend@Less Exposure@Less B+E@+B -E@-B +E")]
        [OverhaulSettingAttribute("Graphics.Amplify color.Amplify color presetV2", 5, false)]
        public static int AmplifyColorMode;
        [OverhaulSettingDropdownParameters("Disabled@Photographic@FilmicACES")]
        [OverhaulSettingAttribute("Graphics.Amplify color.TonemapperV2", 2, false)]
        public static int AmplifyColorTonemapper;

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
            if(!PooledPrefabController.HasCreatedEntry(GenericSparksVFX))
                PooledPrefabController.CreateNewEntry<WeaponSkinCustomVFXInstance>(OverhaulAssetsController.GetAsset("VFX_Sparks", OverhaulAssetPart.Part2).transform, 10, GenericSparksVFX);

            CameraController = OverhaulController.AddController<OverhaulCameraController>();
            OverhaulEvents.AddEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, PatchCamera);
            OverhaulEvents.AddEventListener(OverhaulSettingsController.SettingChangedEventString, PatchAllCameras);

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

            if (!m_ConfiguredEventButtons)
            {
                m_ConfiguredEventButtons = true;
                SetBloomVanilla.EventAction = delegate
                {
                    BloomEnabled = true;
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Post effects.Enable bloom", true), true);
                    BloomIterations = 2;
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Post effects.Bloom iterations", true), 2);
                    BloomIntensity = 0.5f;
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Post effects.Bloom intensity", true), 0.5f);
                    BloomThreshold = 0.9f;
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Post effects.Bloom Threshold", true), 0.9f);

                    ParametersMenu menu = OverhaulController.GetController<ParametersMenu>();
                    if (menu != null && menu.gameObject.activeSelf)
                        menu.PopulateCategory(menu.SelectedCategory, true);
                };

                ApplyAmplifyColorPreset1.EventAction = delegate
                {
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.Amplify color presetV2", true), 5);
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.TonemapperV2", true), 2);

                    ParametersMenu menu = OverhaulController.GetController<ParametersMenu>();
                    if (menu != null && menu.gameObject.activeSelf)
                        menu.PopulateCategory(menu.SelectedCategory, true);
                };

                ApplyAmplifyColorPreset2.EventAction = delegate
                {
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.Amplify color presetV2", true), 2);
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.TonemapperV2", true), 1);

                    ParametersMenu menu = OverhaulController.GetController<ParametersMenu>();
                    if (menu != null && menu.gameObject.activeSelf)
                        menu.PopulateCategory(menu.SelectedCategory, true);
                };

                ApplyAmplifyColorPresetDefault.EventAction = delegate
                {
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.Amplify color presetV2", true), 0);
                    SettingInfo.SavePref(OverhaulSettingsController.GetSetting("Graphics.Amplify color.TonemapperV2", true), 0);

                    ParametersMenu menu = OverhaulController.GetController<ParametersMenu>();
                    if (menu != null && menu.gameObject.activeSelf)
                        menu.PopulateCategory(menu.SelectedCategory, true);
                };
            }
        }

        public static void PatchCamera(Camera camera)
        {
            if (!camera || camera.orthographic)
                return;

            if (!camera.name.Equals("TitleScreenLogoCamera"))
                camera.renderingPath = RenderingPath.UsePlayerSettings;

            PatchBloom(camera.GetComponent<Bloom>());
            PatchAmplifyColor(camera.GetComponent<AmplifyColorBase>());
            refreshAmplifyOcclusionOnCamera(camera);
            addImageEffectsToCamera(camera);
            refreshShaderMaterials();
        }

        public static void PatchBloom(Bloom bloom)
        {
            if (!bloom)
                return;

            bloom.bloomBlurIterations = BloomIterations;
            bloom.bloomIntensity = BloomIntensity;
            bloom.bloomThreshold = BloomThreshold;

            if (!bloom.gameObject.name.Equals("ArenaCamera")) 
                bloom.enabled = BloomEnabled;

            if (!m_BloomEffects.Contains(bloom))
                m_BloomEffects.Add(bloom);
        }

        public static void PatchAmplifyColor(AmplifyColorBase effect)
        {
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
            PatchAmplifyColorMode(effect);
        }

        public static void PatchAmplifyColorMode(AmplifyColorBase effect)
        {
            if (effect == null)
                return;

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

        private static void addImageEffectsToCamera(Camera camera)
        {
            if (IgnoreCamera(camera) || camera.GetComponent<OverhaulImageEffect>() != null)
                return;

            OverhaulImageEffect.AddEffect(camera, s_EdgeBlur, m_EnableBEFunc);
            OverhaulImageEffect.AddEffect(camera, s_ChromaMaterial, m_EnableCAFunc);
            OverhaulImageEffect.AddEffect(camera, s_VignetteMaterial, m_EnableVignetteFunc);
        }

        private static void refreshAmplifyOcclusionOnCamera(Camera camera, bool updateList = true)
        {
            if (IgnoreCamera(camera) || camera != Camera.main || Recommendations.GetSSAORecommendation() == RecommendationLevel.Unsupported)
                return;

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
            bool dontOverrideSettings = !AdvancedPhotomodeSettings.IsOverridingSettings;

            effect.PerPixelNormals = AmplifyOcclusionEffect.PerPixelNormalSource.None;
            effect.Bias = 0f;
            effect.BlurSharpness = 4f;
            effect.FilterResponse = 0.7f;
            effect.Bias = 0.2f;
            effect.SampleCount = dontOverrideSettings ? (AmplifyOcclusion.SampleCountLevel)AOSampleCount : (AmplifyOcclusion.SampleCountLevel)AdvancedPhotomodeSettings.SSAOSampleCount;
            effect.Intensity = dontOverrideSettings ? AOIntensity : AdvancedPhotomodeSettings.SSAOIntensity;
            effect.ApplyMethod = DeferredRenderer ? AmplifyOcclusionEffect.ApplicationMethod.Deferred : AmplifyOcclusionEffect.ApplicationMethod.PostEffect;
            effect.enabled = dontOverrideSettings ? AOEnabled : AdvancedPhotomodeSettings.SSAOEnable;

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


        public static void PatchAllCameras()
        {
            refreshApplicationTargetFramerate();

            if (!CameraController)
                return;

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
    }
}
