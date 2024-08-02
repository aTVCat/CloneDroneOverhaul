using AmplifyOcclusion;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using OverhaulMod.Visuals.ImageEffects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

namespace OverhaulMod.Visuals
{
    public class PostEffectsManager : Singleton<PostEffectsManager>
    {
        [ModSetting(ModSettingsConstants.ENABLE_SSAO, true)]
        public static bool EnableSSAO;

        [ModSetting(ModSettingsConstants.SSAO_SAMPLE_COUNT, 1)]
        public static int SSAOSampleCount;

        [ModSetting(ModSettingsConstants.SSAO_INTENSITY, 0.8f)]
        public static float SSAOIntensity;

        [ModSetting(ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION, true)]
        public static bool EnableChromaticAberration;

        [ModSetting(ModSettingsConstants.CHROMATIC_ABERRATION_INTENSITY, 0.2f)]
        public static float ChromaticAberrationIntensity;

        [ModSetting(ModSettingsConstants.CHROMATIC_ABERRATION_ON_SCREEN_EDGES, true)]
        public static bool ChromaticAberrationOnScreenEdges;

        [ModSetting(ModSettingsConstants.COLOR_BLINDNESS_MODE, 0)]
        public static int ColorBlindnessMode;

        [ModSetting(ModSettingsConstants.COLOR_BLINDNESS_AFFECT_UI, true)]
        public static bool ColorBlindnessAffectUI;

        [ModSetting(ModSettingsConstants.ANTIALIASING_MODE, 0)]
        public static int AntialiasingMode;

        [ModSetting(ModSettingsConstants.MSAA_PLUS_CUSTOM, false)]
        public static bool MSAAPlusCustom;

        [ModSetting(ModSettingsConstants.ENABLE_BLOOM, true)]
        public static bool EnableBloom;

        [ModSetting(ModSettingsConstants.TWEAK_BLOOM, true)]
        public static bool TweakBloom;

        public static List<Dropdown.OptionData> ColorBlindnessOptions = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("Normal vision"),
            new Dropdown.OptionData("Protanopia"),
            new Dropdown.OptionData("Deuteranopia"),
            new Dropdown.OptionData("Tritanopia"),
        };

        public override void Awake()
        {
            base.Awake();
            ModCore.OnCameraSwitched += onCameraSwitched;
        }

        private void OnDestroy()
        {
            ModCore.OnCameraSwitched -= onCameraSwitched;
            RemovePostEffectsFromCamera(Camera.main);
        }

        private void onCameraSwitched(Camera a, Camera b)
        {
            RemovePostEffectsFromCamera(a);
            AddPostEffectsToCamera(b);
        }

        public void RefreshCameraPostEffects()
        {
            AddPostEffectsToCamera(CameraManager.Instance.mainCamera);
        }

        public void AddPostEffectsToCamera(Camera camera)
        {
            if (!camera || camera.orthographic)
                return;

            bool overrideSettings = AdvancedPhotoModeManager.Settings.overrideSettings;

            GameObject cameraGameObject = camera.gameObject;

            AmplifyOcclusionEffect amplifyOcclusionEffect = camera.GetComponent<AmplifyOcclusionEffect>();
            if (!amplifyOcclusionEffect)
                amplifyOcclusionEffect = cameraGameObject.AddComponent<AmplifyOcclusionEffect>();

            amplifyOcclusionEffect.PerPixelNormals = AmplifyOcclusionEffect.PerPixelNormalSource.None;
            amplifyOcclusionEffect.Bias = 0f;
            amplifyOcclusionEffect.BlurSharpness = 4f;
            amplifyOcclusionEffect.FilterResponse = 0.7f;
            amplifyOcclusionEffect.Bias = 0.2f;
            amplifyOcclusionEffect.SampleCount = (SampleCountLevel)SSAOSampleCount;
            amplifyOcclusionEffect.Intensity = SSAOIntensity;
            amplifyOcclusionEffect.ApplyMethod = AmplifyOcclusionEffect.ApplicationMethod.PostEffect;
            amplifyOcclusionEffect.FadeEnabled = true;
            amplifyOcclusionEffect.FadeStart = 0f;
            amplifyOcclusionEffect.FadeLength = Mathf.Min(550f, RenderSettings.fogEndDistance);
            amplifyOcclusionEffect.enabled = overrideSettings ? AdvancedPhotoModeManager.Settings.EnableSSAO : EnableSSAO;

            Bloom bloom = camera.GetComponent<Bloom>();
            if (bloom)
            {
                if (TweakBloom)
                {
                    bloom.bloomBlurIterations = 4;
                    bloom.bloomIntensity = 0.5f;
                    bloom.bloomThreshold = 1f;
                }
                else
                {
                    bloom.bloomBlurIterations = 2;
                    bloom.bloomIntensity = 0.5f;
                    bloom.bloomThreshold = 0.9f;
                }
                bloom.enabled = EnableBloom;
            }

            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.ImageEffects))
                return;

            Antialiasing antialiasing = camera.GetComponent<Antialiasing>();
            if (!antialiasing)
            {
                antialiasing = cameraGameObject.AddComponent<Antialiasing>();
                antialiasing.dlaaShader = Shader.Find("Hidden/DLAA");
                antialiasing.nfaaShader = Shader.Find("Hidden/NFAA");
                antialiasing.ssaaShader = Shader.Find("Hidden/SSAA");
                antialiasing.shaderFXAAII = Shader.Find("Hidden/FXAA II");
                antialiasing.shaderFXAAIII = Shader.Find("Hidden/FXAA III (Console)");
                antialiasing.shaderFXAAPreset2 = Shader.Find("Hidden/FXAA Preset 2");
                antialiasing.shaderFXAAPreset3 = Shader.Find("Hidden/FXAA Preset 3");
            }

            switch ((OverhaulAntialiasingMode)AntialiasingMode)
            {
                case OverhaulAntialiasingMode.FXAA:
                    antialiasing.mode = AAMode.FXAA2;
                    break;
                case OverhaulAntialiasingMode.DLAA:
                    antialiasing.mode = AAMode.DLAA;
                    break;
                case OverhaulAntialiasingMode.NFAA:
                    antialiasing.mode = AAMode.NFAA;
                    break;
                case OverhaulAntialiasingMode.SSAA:
                    antialiasing.mode = AAMode.SSAA;
                    break;
            }
            antialiasing.enabled = AntialiasingMode != 0 || MSAAPlusCustom;

            OverhaulChromaticAberration chromaticAberration = camera.GetComponent<OverhaulChromaticAberration>();
            if (!chromaticAberration)
                chromaticAberration = cameraGameObject.AddComponent<OverhaulChromaticAberration>();

            chromaticAberration.power = ChromaticAberrationIntensity;
            chromaticAberration.center = ChromaticAberrationOnScreenEdges ? 0.5f : 0f;
            chromaticAberration.enabled = EnableChromaticAberration;

            OverhaulColorBlindness overhaulColorBlindness = camera.GetComponent<OverhaulColorBlindness>();
            if (!overhaulColorBlindness)
                overhaulColorBlindness = cameraGameObject.AddComponent<OverhaulColorBlindness>();

            overhaulColorBlindness.type = ColorBlindnessMode;
            overhaulColorBlindness.enabled = !ColorBlindnessAffectUI && ColorBlindnessMode >= 1 && ColorBlindnessMode <= 3;

            Camera uiCamera = ModCache.gameUIRootCamera;
            OverhaulColorBlindness overhaulColorBlindnessOverUI = uiCamera.GetComponent<OverhaulColorBlindness>();
            if (!overhaulColorBlindnessOverUI)
                overhaulColorBlindnessOverUI = uiCamera.gameObject.AddComponent<OverhaulColorBlindness>();

            overhaulColorBlindnessOverUI.type = ColorBlindnessMode;
            overhaulColorBlindnessOverUI.enabled = ColorBlindnessAffectUI && ColorBlindnessMode >= 1 && ColorBlindnessMode <= 3;
        }

        public void RemovePostEffectsFromCamera(Camera camera)
        {
            if (!camera)
                return;

            AmplifyOcclusionEffect amplifyOcclusionEffect = camera.GetComponent<AmplifyOcclusionEffect>();
            if (amplifyOcclusionEffect)
            {
                Destroy(amplifyOcclusionEffect);
            }

            Bloom bloom = camera.GetComponent<Bloom>();
            if (bloom)
            {
                bloom.bloomBlurIterations = 2;
                bloom.bloomIntensity = 0.5f;
                bloom.bloomThreshold = 0.9f;
                bloom.enabled = true;
            }
        }
    }
}
