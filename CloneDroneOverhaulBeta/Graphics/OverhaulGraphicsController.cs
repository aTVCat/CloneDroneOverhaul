using CDOverhaul.Gameplay;
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

        [SettingDropdownParameters("Unlimited@30@60@75@90@120@144@240")]
        [OverhaulSettingAttribute("Graphics.Settings.Target framerate", 2, false, null)]
        public static int TargetFPS;

        [OverhaulSettingAttribute("Graphics.Rendering.Deffered rendering", false, false, "Improve lights renderer\nMedium performance impact!")]
        public static bool DefferedRenderer;

        [OverhaulSettingAttribute("Graphics.Post effects.Bloom", true, false, "Make everything glow")]
        public static bool BloomEnabled;

        [SettingSliderParameters(true, 1, 10)]
        [OverhaulSettingAttribute("Graphics.Post effects.Bloom iterations", 10, false, null, null, null, "Graphics.Post effects.Bloom")]
        public static int BloomIterations;

        [SettingSliderParameters(false, 0.1f, 2f)]
        [OverhaulSettingAttribute("Graphics.Post effects.Bloom intensity", 0.7f, false, null, null, null, "Graphics.Post effects.Bloom")]
        public static float BloomIntensity;

        [SettingSliderParameters(false, 0.85f, 2f)]
        [OverhaulSettingAttribute("Graphics.Post effects.Bloom Threshold", 1.25f, false, null, null, null, "Graphics.Post effects.Bloom")]
        public static float BloomThreshold;

        [OverhaulSettingAttribute("Graphics.Shaders.Vignette", true, false, "Shade screen edges")]
        public static bool VignetteEnabled;

        [OverhaulSettingAttribute("Graphics.Shaders.Blur edges", false, false, "I don't really like it, but you may turn this setting on for some fun, I guess")]
        public static bool BlurEdgesEnabled;

        [SettingSliderParameters(false, -0.2f, 0.3f)]
        [OverhaulSettingAttribute("Graphics.Shaders.Vignette Intensity", 0.05f, false, null, null, null, "Graphics.Shaders.Vignette")]
        public static float VignetteIntensity;

        [OverhaulSettingAttribute("Graphics.Shaders.Chromatic Aberration", false, false, "All things on the screen will get colored edges", "Chromatic Aberration.png")]
        public static bool ChromaticAberrationEnabled;

        [SettingSliderParameters(false, 0f, 0.001f)]
        [OverhaulSettingAttribute("Graphics.Shaders.Chromatic Aberration intensity", 0.0002f, false, null, null, null, "Graphics.Shaders.Chromatic Aberration")]
        public static float ChromaticAberrationIntensity;

        [OverhaulSettingAttribute("Graphics.Amplify Occlusion.Enable", true, false, "Add more shadows to everything")]
        public static bool AOEnabled;

        [SettingSliderParameters(false, 0.7f, 1.3f)]
        [OverhaulSettingAttribute("Graphics.Amplify Occlusion.Intensity", 1.1f, false, null, null, null, "Graphics.Amplify Occlusion.Enable")]
        public static float AOIntensity;

        [SettingDropdownParameters("Low@Medium@High@Very high")]
        [OverhaulSettingAttribute("Graphics.Amplify Occlusion.Sample Count", 2, false, null, null, null, "Graphics.Amplify Occlusion.Enable")]
        public static int AOSampleCount;

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
            _ = OverhaulEventManager.AddEventListener<Camera>(OverhaulGameplayCoreController.MainCameraSwitchedEventString, PatchCamera);
            _ = OverhaulEventManager.AddEventListener(SettingsController.SettingChangedEventString, patchAllCameras);

            m_ChromaMaterial = AssetsController.GetAsset<Material>("M_IE_ChromaticAb", OverhaulAssetsPart.Part2);
            m_VignetteMaterial = AssetsController.GetAsset<Material>("M_IE_Spotlight", OverhaulAssetsPart.Part2);
            m_VignetteMaterial.SetFloat("_CenterY", -0.14f);
            m_EdgeBlur = AssetsController.GetAsset<Material>("M_SnapshotTest", OverhaulAssetsPart.Part2);
            patchAllCameras();
        }

        public static void PatchCamera(Camera camera)
        {
            if (camera == null || camera.orthographic)
            {
                return;
            }
            if(!camera.name.Equals("TitleScreenLogoCamera")) camera.renderingPath = !DefferedRenderer ? RenderingPath.UsePlayerSettings : RenderingPath.DeferredShading;

            PatchBloom(camera.GetComponent<Bloom>());
            refreshAmplifyOcclusionOnCamera(camera);
            addShaderPassesToCamera(camera);
            refreshShaderMaterials();

            //camera.gameObject.AddComponent<MotionBlur>().shader = AssetsController.GetAsset<Shader>("MotionBlur", OverhaulAssetsPart.Part2);
        }

        public static void PatchBloom(Bloom bloom)
        {
            if(bloom == null)
            {
                return;
            }

            bloom.bloomBlurIterations = BloomIterations;
            bloom.bloomIntensity = BloomIntensity;
            bloom.bloomThreshold = BloomThreshold;
            //bloom.bloomThresholdColor = new Color(1, 1, 0.75f, 1);
            bloom.enabled = BloomEnabled;
            if(!m_BloomEffects.Contains(bloom)) m_BloomEffects.Add(bloom);
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

            if (!OverhaulVersion.TechDemo2Enabled)
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
            effect.Bias = 0.8f;
            effect.SampleCount = (AmplifyOcclusion.SampleCountLevel)AOSampleCount;
            effect.Intensity = AOIntensity;
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

        private static void refreshShaderMaterials() // Todo: Make PatchBloom method
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
            if(m_VignetteMaterial != null)
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
                    Application.targetFrameRate = 240;
                    break;
                default:
                    Application.targetFrameRate = -1;
                    break;
            }
        }
    }
}
