using CDOverhaul.Gameplay;
using OverhaulAPI.SharedMonoBehaviours;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace CDOverhaul.Graphics
{
    public static class OverhaulGraphicsController
    {
        public static Camera UICamera { get; private set; }
        public static Camera MainCamera { get; private set; }

        [SettingDropdownParameters("Unlimited@30@60@75@90@120@144@240")]
        [OverhaulSettingAttribute("Graphics.Settings.Target framerate", 2, false, null)]
        public static int TargetFPS;

        [OverhaulSettingAttribute("Graphics.Rendering.Deffered rendering", false, false, "Improve lights renderer\nMedium performance impact!")]
        public static bool DefferedRenderer;

        [SettingSliderParameters(true, 1, 10)]
        [OverhaulSettingAttribute("Graphics.Post effects.Bloom iterations", 10, false, null)]
        public static int BloomIterations;

        [OverhaulSettingAttribute("Graphics.Shaders.Vignette", true, false, "Shade screen edges")]
        public static bool VignetteEnabled;

        [SettingSliderParameters(false, -0.2f, 0.3f)]
        [OverhaulSettingAttribute("Graphics.Shaders.Vignette Intensity", 0.05f, false, null, null, null, "Graphics.Shaders.Vignette")]
        public static float VignetteIntensity;

        [OverhaulSettingAttribute("Graphics.Shaders.Chromatic Aberration", true, false, "All things on the screen will get colored edges", "Chromatic Aberration.png")]
        public static bool ChromaticAberrationEnabled;

        [SettingSliderParameters(false, 0f, 0.001f)]
        [OverhaulSettingAttribute("Graphics.Shaders.Chromatic Aberration intensity", 0.0002f, false, null, null, null, "Graphics.Shaders.Chromatic Aberration")]
        public static float ChromaticAberrationIntensity;

        private static readonly List<Bloom> m_BloomEffects = new List<Bloom>();

        private static Material m_VignetteMaterial;
        private static Material m_ChromaMaterial;

        public static void Initialize()
        {
            _ = OverhaulEventManager.AddEventListener<Camera>(MainGameplayController.MainCameraSwitchedEventString, patchCamera);
            _ = OverhaulEventManager.AddEventListener(OverhaulMod.ModDeactivatedEventString, onModDisabled);
            _ = OverhaulEventManager.AddEventListener(SettingsController.SettingChangedEventString, patchAllCameras);

            UICamera = GameUIRoot.Instance.GetComponent<Canvas>().worldCamera;
            addPostProcessingToCamera(UICamera);
            refreshMaterials();
        }

        private static void addPostProcessingToCamera(Camera camera)
        {
            patchAndSetCamera(camera, false);
            if (camera == null)
            {
                return;
            }

            m_ChromaMaterial = AssetController.GetAsset<Material>("M_IE_ChromaticAb", Enumerators.OverhaulAssetsPart.Part2);
            m_VignetteMaterial = AssetController.GetAsset<Material>("M_IE_Spotlight", Enumerators.OverhaulAssetsPart.Part2);

            OverhaulPostProcessBehaviour.AddPostProcessEffect(camera, m_ChromaMaterial, new System.Func<bool>(() => MainCamera != null && ChromaticAberrationEnabled));
            OverhaulPostProcessBehaviour.AddPostProcessEffect(camera, m_VignetteMaterial, new System.Func<bool>(() => MainCamera != null && VignetteEnabled));
        }

        private static void patchCamera(Camera camera)
        {
            patchAndSetCamera(camera, true);
        }

        private static void patchAndSetCamera(Camera camera, bool setCamera)
        {
            if (setCamera)
            {
                MainCamera = camera;
            }

            if (camera == null)
            {
                return;
            }
            camera.useOcclusionCulling = true;
            camera.renderingPath = !DefferedRenderer ? RenderingPath.UsePlayerSettings : RenderingPath.DeferredShading;

            Bloom bloom = camera.GetComponent<Bloom>();
            if (bloom != null)
            {
                bloom.bloomBlurIterations = BloomIterations;
                bloom.bloomIntensity = 0.7f;
                bloom.bloomThreshold = 1.25f;
                bloom.bloomThresholdColor = new Color(1, 1, 0.75f, 1);
                m_BloomEffects.Add(bloom);
            }
        }

        private static void patchAllCameras()
        {
            refreshTargetFramerate();
            refreshMaterials();
            foreach (Camera cam in Camera.allCameras)
            {
                patchAndSetCamera(cam, false);
            }
        }

        private static void refreshMaterials()
        {
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
                    b.bloomBlurIterations = BloomIterations;
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

        private static void refreshTargetFramerate()
        {
            SettingsManager.Instance.SetVsyncOn(false);
            switch (TargetFPS)
            {
                case 1:
                    Application.targetFrameRate = 30;
                    break;
                case 2:
                    Application.targetFrameRate = 60;
                    SettingsManager.Instance.SetVsyncOn(true);
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

        private static void onModDisabled()
        {
            foreach (OverhaulPostProcessBehaviour b in UICamera.GetComponents<OverhaulPostProcessBehaviour>())
            {
                Object.Destroy(b);
            }
        }
    }
}
