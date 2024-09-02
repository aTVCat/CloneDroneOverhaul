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

        [ModSetting(ModSettingsConstants.ENABLE_DOF, false)]
        public static bool EnableDoF;

        [ModSetting(ModSettingsConstants.ENABLE_BLOOM, true)]
        public static bool EnableBloom;

        [ModSetting(ModSettingsConstants.TWEAK_BLOOM, true)]
        public static bool TweakBloom;

        [ModSetting(ModSettingsConstants.ENABLE_VIGNETTE, true)]
        public static bool EnableVignette;

        [ModSetting(ModSettingsConstants.ENABLE_DITHERING, false)]
        public static bool EnableDithering;

        [ModSetting(ModSettingsConstants.ENABLE_SUN_SHAFTS, false)]
        public static bool EnableSunShafts;

        [ModSetting(ModSettingsConstants.ENABLE_GLOBAL_ILLUMINATION, false)]
        public static bool EnableGlobalIllumination;

        [ModSetting(ModSettingsConstants.ENABLE_REFLECTION_PROBE, false)]
        public static bool EnableReflectionProbe;

        private CameraManager m_cameraManager;

        private float m_timeLeftToRefreshReflectionProbe;

        private ReflectionProbe m_reflectionProbe;

        private Transform m_reflectionProbeTransform;

        public static List<Dropdown.OptionData> ColorBlindnessOptions = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("Normal vision"),
            new Dropdown.OptionData("Protanopia"),
            new Dropdown.OptionData("Deuteranopia"),
            new Dropdown.OptionData("Tritanopia"),
        };

        public static List<Dropdown.OptionData> PresetOptions = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("Select preset..."),
            new Dropdown.OptionData("Very low"),
            new Dropdown.OptionData("Low"),
            new Dropdown.OptionData("Standard (Vanilla)"),
            new Dropdown.OptionData("Standard (Overhaul)"),
            new Dropdown.OptionData("High"),
            new Dropdown.OptionData("Very high"),
            new Dropdown.OptionData("Extreme"),
        };

        public override void Awake()
        {
            base.Awake();
            ModCore.OnCameraSwitched += onCameraSwitched;
        }

        private void Start()
        {
            m_cameraManager = CameraManager.Instance;

            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.MoreImageEffects))
                return;

            createReflectionProbe();
        }

        private void Update()
        {
            m_timeLeftToRefreshReflectionProbe -= Time.unscaledDeltaTime;
            if (m_timeLeftToRefreshReflectionProbe > 0f)
                return;

            m_timeLeftToRefreshReflectionProbe = 1f;

            Transform rp = m_reflectionProbeTransform;
            if (rp)
            {
                Camera camera = m_cameraManager.mainCamera;
                if (camera)
                {
                    rp.position = camera.transform.position;
                }
            }

            ReflectionProbe reflectionProbe = m_reflectionProbe;
            if (reflectionProbe)
            {
                reflectionProbe.enabled = EnableReflectionProbe;
            }
        }

        private void OnDestroy()
        {
            ModCore.OnCameraSwitched -= onCameraSwitched;
            RemovePostEffectsFromCamera(Camera.main);
        }

        public void RefreshReflectionProbeNextFrame()
        {
            m_timeLeftToRefreshReflectionProbe = 0f;
        }

        private void createReflectionProbe()
        {
            GameObject reflectionProbe = new GameObject("Reflection Probe");
            reflectionProbe.transform.SetParent(base.transform);
            m_reflectionProbe = reflectionProbe.AddComponent<ReflectionProbe>();
            m_reflectionProbe.size = Vector3.one * 500f;
            m_reflectionProbe.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
            m_reflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
            m_reflectionProbe.enabled = false;
            m_reflectionProbeTransform = reflectionProbe.transform;
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
            if (!camera)
                return;

            bool isNotOrthographic = !camera.orthographic;
            bool moreEffectsEnabled = ModFeatures.IsEnabled(ModFeatures.FeatureType.MoreImageEffects);
            bool overrideSettings = AdvancedPhotoModeManager.Settings.overrideSettings;

            GameObject cameraGameObject = camera.gameObject;

            if (isNotOrthographic)
            {
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
            }

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

            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.ColorBlindnessOptions))
            {
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

            if (isNotOrthographic)
            {
                OverhaulChromaticAberration chromaticAberration = camera.GetComponent<OverhaulChromaticAberration>();
                if (!chromaticAberration)
                    chromaticAberration = cameraGameObject.AddComponent<OverhaulChromaticAberration>();

                chromaticAberration.power = ChromaticAberrationIntensity;
                chromaticAberration.center = ChromaticAberrationOnScreenEdges ? 0.5f : 0f;
                chromaticAberration.enabled = EnableChromaticAberration;

                NoiseAndGrain noiseAndGrain = camera.GetComponent<NoiseAndGrain>();
                if (!noiseAndGrain)
                {
                    noiseAndGrain = cameraGameObject.AddComponent<NoiseAndGrain>();
                    noiseAndGrain.noiseShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "NoiseAndGrain");
                    noiseAndGrain.dx11NoiseShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "NoiseAndGrainDX11");
                    noiseAndGrain.noiseTexture = ModResources.Texture2D(AssetBundleConstants.IMAGE_EFFECTS, "Noise");
                    noiseAndGrain.generalIntensity = 0.15f;
                    noiseAndGrain.intensityMultiplier = 0.2f;
                    noiseAndGrain.whiteIntensity = 0.75f;
                    noiseAndGrain.blackIntensity = 1f;
                    noiseAndGrain.midGrey = 0.4f;
                    noiseAndGrain.softness = 0.15f;
                    noiseAndGrain.midGrey = 0.2f;
                }
                noiseAndGrain.enabled = overrideSettings ? AdvancedPhotoModeManager.Settings.EnableDithering : EnableDithering;

                VignetteAndChromaticAberration vignetteAndChromaticAberration = camera.GetComponent<VignetteAndChromaticAberration>();
                if (!vignetteAndChromaticAberration)
                {
                    vignetteAndChromaticAberration = cameraGameObject.AddComponent<VignetteAndChromaticAberration>();
                    vignetteAndChromaticAberration.vignetteShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "VignettingShader");
                    vignetteAndChromaticAberration.chromAberrationShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "ChromaticAberrationShader");
                    vignetteAndChromaticAberration.separableBlurShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "SeparableBlur");
                    vignetteAndChromaticAberration.chromaticAberration = 0f;
                }
                vignetteAndChromaticAberration.intensity = overrideSettings ? AdvancedPhotoModeManager.Settings.VignetteIntensity : 0.23f;
                vignetteAndChromaticAberration.enabled = overrideSettings ? AdvancedPhotoModeManager.Settings.EnableVignette : EnableVignette;

                if (!moreEffectsEnabled)
                    return;

                /*
                ScreenSpaceAmbientOcclusion screenSpaceAmbientOcclusion = camera.GetComponent<ScreenSpaceAmbientOcclusion>();
                if (!screenSpaceAmbientOcclusion)
                {
                    screenSpaceAmbientOcclusion = cameraGameObject.AddComponent<ScreenSpaceAmbientOcclusion>();
                    screenSpaceAmbientOcclusion.m_SSAOShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "SSAOShader");
                    screenSpaceAmbientOcclusion.m_RandomTexture = ModResources.Texture2D(AssetBundleConstants.IMAGE_EFFECTS, "RandomVectors");
                }*/

                /*
                GlobalFog globalFog = camera.GetComponent<GlobalFog>();
                if (!globalFog)
                {
                    globalFog = cameraGameObject.AddComponent<GlobalFog>();
                    globalFog.fogShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "GlobalFog");
                }*/

                SEGICascaded segiCascaded = camera.GetComponent<SEGICascaded>();
                if (!segiCascaded)
                {
                    segiCascaded = cameraGameObject.AddComponent<SEGICascaded>();
                    segiCascaded.sun = DirectionalLightManager.Instance.DirectionalLight;
                    segiCascaded.ApplyPreset(new SEGICascadedPreset());
                }
                segiCascaded.enabled = EnableGlobalIllumination;

                BlurOptimized blurOptimized = camera.GetComponent<BlurOptimized>();
                if (!blurOptimized)
                {
                    blurOptimized = cameraGameObject.AddComponent<BlurOptimized>();
                    blurOptimized.blurShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "MobileBlur");
                    blurOptimized.enabled = false;
                }

                /*
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
                antialiasing.enabled = AntialiasingMode != 0 || MSAAPlusCustom;*/

                DepthOfField depthOfField = camera.GetComponent<DepthOfField>();
                if (!depthOfField)
                {
                    depthOfField = cameraGameObject.AddComponent<DepthOfField>();
                    depthOfField.dofHdrShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "DepthOfFieldScatter");
                    depthOfField.dx11BokehShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "DepthOfFieldDX11");
                    depthOfField.focalLength = 9;
                    depthOfField.focalSize = 0.9f;
                    depthOfField.blurSampleCount = DepthOfField.BlurSampleCount.Low;
                    depthOfField.nearBlur = true;
                }
                depthOfField.enabled = EnableDoF;

                SunShafts sunShafts = camera.GetComponent<SunShafts>();
                if (!sunShafts)
                {
                    sunShafts = cameraGameObject.AddComponent<SunShafts>();
                    sunShafts.simpleClearShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "SimpleClear");
                    sunShafts.sunShaftsShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "SunShaftsComposite");
                    sunShafts.sunThreshold = new Color(0.8f, 0.9f, 0.8f, 1f);
                    sunShafts.resolution = SunShafts.SunShaftsResolution.Low;
                    sunShafts.sunShaftIntensity = 1.2f;
                }
                sunShafts.enabled = EnableSunShafts;

                /*
                BloomAndFlares bloomAndFlares = camera.GetComponent<BloomAndFlares>();
                if (!bloomAndFlares)
                {
                    bloomAndFlares = cameraGameObject.AddComponent<BloomAndFlares>();
                    bloomAndFlares.separableBlurShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "SeparableBlurPlus");
                    bloomAndFlares.screenBlendShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "Blend");
                    bloomAndFlares.brightPassFilterShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "BrightPassFilter");
                    bloomAndFlares.addBrightStuffOneOneShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "BlendOneOne");
                    bloomAndFlares.hollywoodFlaresShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "MultiPassHollywoodFlares");
                    bloomAndFlares.lensFlareShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "LensFlareCreate");
                    bloomAndFlares.vignetteShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "VignetteShader");
                    bloomAndFlares.lensFlareVignetteMask = ModResources.Texture2D(AssetBundleConstants.IMAGE_EFFECTS, "VignetteMask");
                    bloomAndFlares.lensflares = true;
                    bloomAndFlares.lensflareMode = LensflareStyle34.Ghosting;   
                    bloomAndFlares.bloomBlurIterations = 4;
                    bloomAndFlares.bloomIntensity = 0.5f;
                    bloomAndFlares.bloomThreshold = 1f;
                }*/

                /*
                EdgeDetection edgeDetection = camera.GetComponent<EdgeDetection>();
                if (!edgeDetection)
                {
                    edgeDetection = cameraGameObject.AddComponent<EdgeDetection>();
                    edgeDetection.edgeDetectShader= ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "EdgeDetectNormals");
                }*/

                /*
                ContrastEnhance contrastEnhance = camera.GetComponent<ContrastEnhance>();
                if (!contrastEnhance)
                {
                    contrastEnhance = cameraGameObject.AddComponent<ContrastEnhance>();
                    contrastEnhance.contrastCompositeShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "ContrastComposite");
                    contrastEnhance.separableBlurShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "SeparableBlur");
                }*/
            }
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
