using OverhaulMod.Engine;
using OverhaulMod.Utils;
using OverhaulMod.Visuals.ImageEffects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [ModSetting(ModSettingsConstants.CHROMATIC_ABERRATION_INTENSITY, 0.12f)]
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

        private List<ModSettingsPreset> m_graphicsPresets;

        private CameraManager m_cameraManager;

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
            new Dropdown.OptionData("Medium"),
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
            createGraphicsPresets();
        }

        private void OnDestroy()
        {
            ModCore.OnCameraSwitched -= onCameraSwitched;
            RemovePostEffectsFromCamera(Camera.main);
        }

        public void ApplyGraphicsPreset(int index)
        {
            ModSettingsPreset modSettingsPreset = m_graphicsPresets[index];
            modSettingsPreset.Apply();
        }

        private void createGraphicsPresets()
        {
            List<ModSettingsPreset> list = new List<ModSettingsPreset>();
            m_graphicsPresets = list;

            ModSettingsPreset lowest = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Fast,
                AntiAliasingMode = AntiAliasingMode.NoAntiAliasing
            };
            lowest.AddValue(ModSettingsConstants.ENABLE_SSAO, false);
            lowest.AddValue(ModSettingsConstants.ENABLE_GLOBAL_ILLUMINATION, false);
            lowest.AddValue(ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION, false);
            lowest.AddValue(ModSettingsConstants.ENABLE_DITHERING, false);
            lowest.AddValue(ModSettingsConstants.ENABLE_VIGNETTE, false);
            lowest.AddValue(ModSettingsConstants.ENABLE_BLOOM, false);
            lowest.AddValue(ModSettingsConstants.ENABLE_DOF, false);
            lowest.AddValue(ModSettingsConstants.ENABLE_SUN_SHAFTS, false);
            list.Add(lowest);

            ModSettingsPreset low = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Fast,
                AntiAliasingMode = AntiAliasingMode.NoAntiAliasing
            };
            low.AddValue(ModSettingsConstants.ENABLE_SSAO, false);
            low.AddValue(ModSettingsConstants.ENABLE_GLOBAL_ILLUMINATION, false);
            low.AddValue(ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION, false);
            low.AddValue(ModSettingsConstants.ENABLE_DITHERING, false);
            low.AddValue(ModSettingsConstants.ENABLE_VIGNETTE, false);
            low.AddValue(ModSettingsConstants.ENABLE_BLOOM, true);
            low.AddValue(ModSettingsConstants.ENABLE_DOF, false);
            low.AddValue(ModSettingsConstants.ENABLE_SUN_SHAFTS, false);
            list.Add(low);

            ModSettingsPreset mid = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Good,
                AntiAliasingMode = AntiAliasingMode.MSAA2X
            };
            mid.AddValue(ModSettingsConstants.ENABLE_SSAO, false);
            mid.AddValue(ModSettingsConstants.ENABLE_GLOBAL_ILLUMINATION, false);
            mid.AddValue(ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION, false);
            mid.AddValue(ModSettingsConstants.ENABLE_DITHERING, false);
            mid.AddValue(ModSettingsConstants.ENABLE_VIGNETTE, false);
            mid.AddValue(ModSettingsConstants.ENABLE_BLOOM, true);
            mid.AddValue(ModSettingsConstants.ENABLE_DOF, false);
            mid.AddValue(ModSettingsConstants.ENABLE_SUN_SHAFTS, false);
            list.Add(mid);

            ModSettingsPreset standardVanilla = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Beautiful,
                AntiAliasingMode = AntiAliasingMode.MSAA4X
            };
            standardVanilla.AddValue(ModSettingsConstants.ENABLE_SSAO, false);
            standardVanilla.AddValue(ModSettingsConstants.ENABLE_GLOBAL_ILLUMINATION, false);
            standardVanilla.AddValue(ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION, false);
            standardVanilla.AddValue(ModSettingsConstants.ENABLE_DITHERING, false);
            standardVanilla.AddValue(ModSettingsConstants.ENABLE_VIGNETTE, false);
            standardVanilla.AddValue(ModSettingsConstants.ENABLE_BLOOM, true);
            standardVanilla.AddValue(ModSettingsConstants.ENABLE_DOF, false);
            standardVanilla.AddValue(ModSettingsConstants.ENABLE_SUN_SHAFTS, false);
            list.Add(standardVanilla);

            ModSettingsPreset standardOverhaul = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Beautiful,
                AntiAliasingMode = AntiAliasingMode.MSAA8X
            };
            standardOverhaul.AddValue(ModSettingsConstants.ENABLE_SSAO, true);
            standardOverhaul.AddValue(ModSettingsConstants.ENABLE_GLOBAL_ILLUMINATION, false);
            standardOverhaul.AddValue(ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION, false);
            standardOverhaul.AddValue(ModSettingsConstants.ENABLE_DITHERING, false);
            standardOverhaul.AddValue(ModSettingsConstants.ENABLE_VIGNETTE, true);
            standardOverhaul.AddValue(ModSettingsConstants.ENABLE_BLOOM, true);
            standardOverhaul.AddValue(ModSettingsConstants.ENABLE_DOF, false);
            standardOverhaul.AddValue(ModSettingsConstants.ENABLE_SUN_SHAFTS, false);
            list.Add(standardOverhaul);

            ModSettingsPreset high = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Beautiful,
                AntiAliasingMode = AntiAliasingMode.MSAA8X
            };
            high.AddValue(ModSettingsConstants.ENABLE_SSAO, true);
            high.AddValue(ModSettingsConstants.ENABLE_GLOBAL_ILLUMINATION, false);
            high.AddValue(ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION, true);
            high.AddValue(ModSettingsConstants.ENABLE_DITHERING, true);
            high.AddValue(ModSettingsConstants.ENABLE_VIGNETTE, true);
            high.AddValue(ModSettingsConstants.ENABLE_BLOOM, true);
            high.AddValue(ModSettingsConstants.ENABLE_DOF, false);
            high.AddValue(ModSettingsConstants.ENABLE_SUN_SHAFTS, false);
            list.Add(high);

            ModSettingsPreset veryHigh = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Beautiful,
                AntiAliasingMode = AntiAliasingMode.MSAA8X
            };
            veryHigh.AddValue(ModSettingsConstants.ENABLE_SSAO, true);
            veryHigh.AddValue(ModSettingsConstants.ENABLE_GLOBAL_ILLUMINATION, false);
            veryHigh.AddValue(ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION, true);
            veryHigh.AddValue(ModSettingsConstants.ENABLE_DITHERING, true);
            veryHigh.AddValue(ModSettingsConstants.ENABLE_VIGNETTE, true);
            veryHigh.AddValue(ModSettingsConstants.ENABLE_BLOOM, true);
            veryHigh.AddValue(ModSettingsConstants.ENABLE_DOF, true);
            veryHigh.AddValue(ModSettingsConstants.ENABLE_SUN_SHAFTS, true);
            list.Add(veryHigh);

            ModSettingsPreset extreme = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Beautiful,
                AntiAliasingMode = AntiAliasingMode.MSAA8X
            };
            extreme.AddValue(ModSettingsConstants.ENABLE_SSAO, true);
            extreme.AddValue(ModSettingsConstants.ENABLE_GLOBAL_ILLUMINATION, true);
            extreme.AddValue(ModSettingsConstants.ENABLE_CHROMATIC_ABERRATION, true);
            extreme.AddValue(ModSettingsConstants.ENABLE_DITHERING, true);
            extreme.AddValue(ModSettingsConstants.ENABLE_VIGNETTE, true);
            extreme.AddValue(ModSettingsConstants.ENABLE_BLOOM, true);
            extreme.AddValue(ModSettingsConstants.ENABLE_DOF, true);
            extreme.AddValue(ModSettingsConstants.ENABLE_SUN_SHAFTS, true);
            list.Add(extreme);
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
            if (!camera) return;

            PostEffectsContainer postEffectsContainer = camera.GetComponent<PostEffectsContainer>();
            if (!postEffectsContainer)
            {
                postEffectsContainer = camera.gameObject.AddComponent<PostEffectsContainer>();
                postEffectsContainer.Initialize(camera);
            }
            else
            {
                postEffectsContainer.RefreshEffects();
            }

            if (ModFeatures.IsEnabled(ModFeatures.FeatureType.ColorBlindnessOptions))
            {
                Camera uiCamera = ModCache.gameUIRootCamera;
                OverhaulColorBlindness overhaulColorBlindnessOverUI = uiCamera.GetComponent<OverhaulColorBlindness>();
                if (!overhaulColorBlindnessOverUI && ColorBlindnessAffectUI && ColorBlindnessMode >= 1 && ColorBlindnessMode <= 3)
                    overhaulColorBlindnessOverUI = uiCamera.gameObject.AddComponent<OverhaulColorBlindness>();

                if (overhaulColorBlindnessOverUI)
                {
                    overhaulColorBlindnessOverUI.type = ColorBlindnessMode;
                    overhaulColorBlindnessOverUI.enabled = ColorBlindnessAffectUI && ColorBlindnessMode >= 1 && ColorBlindnessMode <= 3;
                }
            }
        }

        public void RemovePostEffectsFromCamera(Camera camera)
        {
            if (!camera)
                return;

            PostEffectsContainer postEffectsContainer = camera.GetComponent<PostEffectsContainer>();
            if (postEffectsContainer)
            {
                postEffectsContainer.ResetEffects();
            }
        }
    }
}
