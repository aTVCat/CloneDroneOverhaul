using AmplifyOcclusion;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using OverhaulMod.Visuals.ImageEffects;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace OverhaulMod.Visuals
{
    public class PostEffectsContainer : MonoBehaviour
    {
        private Camera m_camera;

        public Bloom Bloom;

        public AmplifyOcclusionEffect AmplifyOcclusion;

        public SEGICascaded GlobalIllumination;

        public DepthOfField DoF;

        public SunShafts SunShafts;

        public OverhaulChromaticAberration ChromaticAberration;

        public NoiseAndGrain Dithering;

        public VignetteAndChromaticAberration Vignette;

        public OverhaulColorBlindness ColorBlindness;

        public void Initialize(Camera camera)
        {
            m_camera = camera;
            RefreshEffects();
        }

        public void ResetEffects()
        {
            if (AmplifyOcclusion)
                Destroy(AmplifyOcclusion);

            AmplifyOcclusion = null;

            if (GlobalIllumination)
                Destroy(GlobalIllumination);

            GlobalIllumination = null;

            if (DoF)
                Destroy(DoF);

            DoF = null;

            if (ChromaticAberration)
                Destroy(ChromaticAberration);

            ChromaticAberration = null;

            if (Dithering)
                Destroy(Dithering);

            Dithering = null;

            if (Vignette)
                Destroy(Vignette);

            Vignette = null;

            if (ColorBlindness)
                Destroy(ColorBlindness);

            ColorBlindness = null;

            Bloom bloom = Bloom;
            if (!bloom)
            {
                bloom = base.GetComponent<Bloom>();
                Bloom = bloom;
            }

            if (bloom)
            {
                bloom.bloomBlurIterations = 2;
                bloom.bloomIntensity = 0.5f;
                bloom.bloomThreshold = 0.9f;
                bloom.enabled = true;
            }
        }

        public void RefreshEffects()
        {
            bool shouldEnableEffects = ShouldEnableEffects();
            bool overrideSettings = AdvancedPhotoModeManager.Settings.overrideSettings;

            GameObject cameraObject = m_camera.gameObject;

            refreshBloom(PostEffectsManager.EnableBloom, cameraObject);
            refreshAmplifyOcclusion(shouldEnableEffects && (overrideSettings ? AdvancedPhotoModeManager.Settings.EnableSSAO : PostEffectsManager.EnableSSAO), cameraObject);
            refreshGlobalIllumination(shouldEnableEffects && (overrideSettings ? AdvancedPhotoModeManager.Settings.EnableGlobalIllumination : PostEffectsManager.EnableGlobalIllumination), cameraObject);
            refreshDoF(shouldEnableEffects && (overrideSettings ? AdvancedPhotoModeManager.Settings.EnableDoF : PostEffectsManager.EnableDoF), cameraObject);
            refreshSunShafts(shouldEnableEffects && (overrideSettings ? AdvancedPhotoModeManager.Settings.EnableSunShafts : PostEffectsManager.EnableSunShafts), cameraObject);
            refreshChromaticAberration(shouldEnableEffects && (overrideSettings ? AdvancedPhotoModeManager.Settings.EnableCA : PostEffectsManager.EnableChromaticAberration), cameraObject);
            refreshDithering(shouldEnableEffects && (overrideSettings ? AdvancedPhotoModeManager.Settings.EnableDithering : PostEffectsManager.EnableDithering), cameraObject);
            refreshVignette(shouldEnableEffects && (overrideSettings ? AdvancedPhotoModeManager.Settings.EnableVignette : PostEffectsManager.EnableVignette), cameraObject);
            refreshColorBlindness(!PostEffectsManager.ColorBlindnessAffectUI && PostEffectsManager.ColorBlindnessMode >= 1 && PostEffectsManager.ColorBlindnessMode <= 3, cameraObject);
        }

        private void refreshBloom(bool enable, GameObject cameraObject)
        {
            Bloom bloom = Bloom;
            if (!bloom)
            {
                bloom = cameraObject.GetComponent<Bloom>();
                Bloom = bloom;
            }

            if (bloom)
            {
                if (PostEffectsManager.TweakBloom)
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
                bloom.enabled = enable;
            }
        }

        private void refreshAmplifyOcclusion(bool enable, GameObject cameraObject)
        {
            AmplifyOcclusionEffect amplifyOcclusionEffect = AmplifyOcclusion;
            if (!amplifyOcclusionEffect)
            {
                amplifyOcclusionEffect = cameraObject.GetComponent<AmplifyOcclusionEffect>();
                if (!amplifyOcclusionEffect && enable)
                {
                    amplifyOcclusionEffect = cameraObject.AddComponent<AmplifyOcclusionEffect>();
                    amplifyOcclusionEffect.ApplyMethod = AmplifyOcclusionEffect.ApplicationMethod.PostEffect;
                    amplifyOcclusionEffect.FadeEnabled = true;
                    amplifyOcclusionEffect.FadeStart = 0f;
                    amplifyOcclusionEffect.PerPixelNormals = AmplifyOcclusionEffect.PerPixelNormalSource.None;
                    amplifyOcclusionEffect.Bias = 0f;
                    amplifyOcclusionEffect.BlurSharpness = 4f;
                    amplifyOcclusionEffect.FilterResponse = 0.7f;
                    amplifyOcclusionEffect.Bias = 0.2f;
                }

                AmplifyOcclusion = amplifyOcclusionEffect;
            }

            if (amplifyOcclusionEffect)
            {
                amplifyOcclusionEffect.SampleCount = (SampleCountLevel)PostEffectsManager.SSAOSampleCount;
                amplifyOcclusionEffect.Intensity = PostEffectsManager.SSAOIntensity;
                amplifyOcclusionEffect.FadeLength = Mathf.Min(550f, RenderSettings.fogEndDistance);
                amplifyOcclusionEffect.enabled = enable;
            }
        }

        private void refreshGlobalIllumination(bool enable, GameObject cameraObject)
        {
            SEGICascaded segiCascaded = GlobalIllumination;
            if (!segiCascaded)
            {
                segiCascaded = cameraObject.GetComponent<SEGICascaded>();
                if (!segiCascaded && enable)
                {
                    segiCascaded = cameraObject.AddComponent<SEGICascaded>();
                    segiCascaded.sun = DirectionalLightManager.Instance.DirectionalLight;
                    segiCascaded.ApplyPreset(new SEGICascadedPreset());
                }

                GlobalIllumination = segiCascaded;
            }

            if (segiCascaded)
                segiCascaded.enabled = enable;
        }

        private void refreshDoF(bool enable, GameObject cameraObject)
        {
            DepthOfField depthOfField = DoF;
            if (!depthOfField)
            {
                depthOfField = cameraObject.GetComponent<DepthOfField>();
                if (!depthOfField && enable)
                {
                    depthOfField = cameraObject.AddComponent<DepthOfField>();
                    depthOfField.dofHdrShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "DepthOfFieldScatter");
                    depthOfField.dx11BokehShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "DepthOfFieldDX11");
                    depthOfField.blurSampleCount = DepthOfField.BlurSampleCount.Low;
                    depthOfField.focalSize = 0.9f;
                }

                DoF = depthOfField;
            }

            if (depthOfField)
            {
                bool fpm = CameraManager.EnableFirstPersonMode;

                depthOfField.nearBlur = !fpm;
                depthOfField.focalLength = fpm ? 1f : 9f;
                depthOfField.enabled = !CameraManager.Instance.isCameraControlledByCutscene && enable;
            }
        }

        private void refreshSunShafts(bool enable, GameObject cameraObject)
        {
            SunShafts sunShafts = SunShafts;
            if (!sunShafts)
            {
                sunShafts = cameraObject.GetComponent<SunShafts>();
                if (!sunShafts && enable)
                {
                    sunShafts = cameraObject.AddComponent<SunShafts>();
                    sunShafts.simpleClearShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "SimpleClear");
                    sunShafts.sunShaftsShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "SunShaftsComposite");
                    sunShafts.sunThreshold = new Color(0.8f, 0.9f, 0.8f, 1f);
                    sunShafts.resolution = SunShafts.SunShaftsResolution.Low;
                    sunShafts.sunShaftIntensity = 1.2f;
                }

                SunShafts = sunShafts;
            }

            if (sunShafts)
                sunShafts.enabled = enable;
        }

        private void refreshChromaticAberration(bool enable, GameObject cameraObject)
        {
            OverhaulChromaticAberration chromaticAberration = ChromaticAberration;
            if (!chromaticAberration)
            {
                chromaticAberration = cameraObject.GetComponent<OverhaulChromaticAberration>();
                if (!chromaticAberration && enable)
                {
                    chromaticAberration = cameraObject.AddComponent<OverhaulChromaticAberration>();
                }

                ChromaticAberration = chromaticAberration;
            }

            if (chromaticAberration)
            {
                chromaticAberration.power = PostEffectsManager.ChromaticAberrationIntensity;
                chromaticAberration.center = PostEffectsManager.ChromaticAberrationOnScreenEdges ? 0.5f : 0f;
                chromaticAberration.enabled = enable;
            }
        }

        private void refreshDithering(bool enable, GameObject cameraObject)
        {
            NoiseAndGrain dithering = Dithering;
            if (!dithering)
            {
                dithering = cameraObject.GetComponent<NoiseAndGrain>();
                if (!dithering && enable)
                {
                    dithering = cameraObject.AddComponent<NoiseAndGrain>();
                    dithering.noiseShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "NoiseAndGrain");
                    dithering.dx11NoiseShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "NoiseAndGrainDX11");
                    dithering.noiseTexture = ModResources.Texture2D(AssetBundleConstants.IMAGE_EFFECTS, "Noise");
                    dithering.generalIntensity = 0.15f;
                    dithering.intensityMultiplier = 0.2f;
                    dithering.whiteIntensity = 0.75f;
                    dithering.blackIntensity = 1f;
                    dithering.midGrey = 0.4f;
                    dithering.softness = 0.15f;
                    dithering.midGrey = 0.2f;
                }

                Dithering = dithering;
            }

            if (dithering)
                dithering.enabled = enable;
        }

        private void refreshVignette(bool enable, GameObject cameraObject)
        {
            VignetteAndChromaticAberration vignetteAndChromaticAberration = Vignette;
            if (!vignetteAndChromaticAberration)
            {
                vignetteAndChromaticAberration = cameraObject.GetComponent<VignetteAndChromaticAberration>();
                if (!vignetteAndChromaticAberration && enable)
                {
                    vignetteAndChromaticAberration = cameraObject.AddComponent<VignetteAndChromaticAberration>();
                    vignetteAndChromaticAberration.vignetteShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "VignettingShader");
                    vignetteAndChromaticAberration.chromAberrationShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "ChromaticAberrationShader");
                    vignetteAndChromaticAberration.separableBlurShader = ModResources.Shader(AssetBundleConstants.IMAGE_EFFECTS, "SeparableBlur");
                    vignetteAndChromaticAberration.chromaticAberration = 0f;
                }

                Vignette = vignetteAndChromaticAberration;
            }

            if (vignetteAndChromaticAberration)
            {
                vignetteAndChromaticAberration.intensity = AdvancedPhotoModeManager.Settings.overrideSettings ? AdvancedPhotoModeManager.Settings.VignetteIntensity : 0.2f;
                vignetteAndChromaticAberration.enabled = enable;
            }
        }

        private void refreshColorBlindness(bool enable, GameObject cameraObject)
        {
            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.ColorBlindnessOptions))
                return;

            OverhaulColorBlindness overhaulColorBlindness = ColorBlindness;
            if (!overhaulColorBlindness)
            {
                overhaulColorBlindness = cameraObject.GetComponent<OverhaulColorBlindness>();
                if (!overhaulColorBlindness && enable)
                {
                    overhaulColorBlindness = cameraObject.AddComponent<OverhaulColorBlindness>();
                }

                ColorBlindness = overhaulColorBlindness;
            }

            if (overhaulColorBlindness)
            {
                overhaulColorBlindness.type = PostEffectsManager.ColorBlindnessMode;
                overhaulColorBlindness.enabled = enable;
            }
        }

        public bool ShouldEnableEffects()
        {
            return !GameModeManager.IsInLevelEditor() && m_camera && !m_camera.orthographic;
        }
    }
}
