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
        [ModSetting(ModSettingIDs.ENABLE_SSAO, true)]
        public static bool EnableSSAO;

        [ModSetting(ModSettingIDs.SSAO_SAMPLE_COUNT, 2)]
        public static int SSAOSampleCount;

        [ModSetting(ModSettingIDs.SSAO_INTENSITY, 0.6f)]
        public static float SSAOIntensity;

        [ModSetting(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, false)]
        public static bool EnableChromaticAberration;

        [ModSetting(ModSettingIDs.CHROMATIC_ABERRATION_INTENSITY, 0.12f)]
        public static float ChromaticAberrationIntensity;

        [ModSetting(ModSettingIDs.CHROMATIC_ABERRATION_ON_SCREEN_EDGES, true)]
        public static bool ChromaticAberrationOnScreenEdges;

        [ModSetting(ModSettingIDs.COLOR_BLINDNESS_MODE, 0)]
        public static int ColorBlindnessMode;

        [ModSetting(ModSettingIDs.COLOR_BLINDNESS_AFFECT_UI, true)]
        public static bool ColorBlindnessAffectUI;

        [ModSetting(ModSettingIDs.ENABLE_DOF, false)]
        public static bool EnableDoF;

        [ModSetting(ModSettingIDs.BLOOM_MODE, (int)BloomMode.Fancy)]
        public static int Bloom;

        [ModSetting(ModSettingIDs.ENABLE_VIGNETTE, false)]
        public static bool EnableVignette;

        [ModSetting(ModSettingIDs.ENABLE_DITHERING, false)]
        public static bool EnableDithering;

        [ModSetting(ModSettingIDs.ENABLE_SUN_SHAFTS, false)]
        public static bool EnableSunShafts;

        [ModSetting(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION, false)]
        public static bool EnableGlobalIllumination;

        [ModSetting(ModSettingIDs.DISABLE_SCREEN_SHAKING, false)]
        public static bool DisableScreenShaking;

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

        public static List<Dropdown.OptionData> BloomOptions = new List<Dropdown.OptionData>()
        {
            new DropdownIntOptionData() { text = "Disabled", IntValue = (int)BloomMode.Disabled },
            new DropdownIntOptionData() { text = "Vanilla", IntValue = (int)BloomMode.Vanilla },
            new DropdownIntOptionData() { text = "Fancy", IntValue = (int)BloomMode.Fancy },
            new DropdownIntOptionData() { text = "Fanciest", IntValue = (int)BloomMode.Fanciest },
        };

        private List<ModSettingsPreset> _graphicsPresets;

        private bool _refreshEffectsNextFrame;

        public override void Awake()
        {
            base.Awake();
            createGraphicsPresets();
        }

        private void Update()
        {
            if (_refreshEffectsNextFrame)
            {
                _refreshEffectsNextFrame = false;
                RefreshCameraPostEffects();
            }
        }
        private void OnDestroy()
        {
            RemovePostEffectsFromCamera(Camera.main);
        }

        public void ApplyGraphicsPreset(int index)
        {
            ModSettingsPreset modSettingsPreset = _graphicsPresets[index];
            modSettingsPreset.Apply();
        }

        private void createGraphicsPresets()
        {
            List<ModSettingsPreset> list = new List<ModSettingsPreset>();
            _graphicsPresets = list;

            ModSettingsPreset lowest = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Fast,
                AntiAliasingMode = AntiAliasingMode.NoAntiAliasing
            };
            lowest.AddValue(ModSettingIDs.ENABLE_SSAO, false);
            lowest.AddValue(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION, false);
            lowest.AddValue(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, false);
            lowest.AddValue(ModSettingIDs.ENABLE_DITHERING, false);
            lowest.AddValue(ModSettingIDs.ENABLE_VIGNETTE, false);
            lowest.AddValue(ModSettingIDs.BLOOM_MODE, (int)BloomMode.Disabled);
            lowest.AddValue(ModSettingIDs.ENABLE_DOF, false);
            lowest.AddValue(ModSettingIDs.ENABLE_SUN_SHAFTS, false);
            list.Add(lowest);

            ModSettingsPreset low = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Fast,
                AntiAliasingMode = AntiAliasingMode.NoAntiAliasing
            };
            low.AddValue(ModSettingIDs.ENABLE_SSAO, false);
            low.AddValue(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION, false);
            low.AddValue(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, false);
            low.AddValue(ModSettingIDs.ENABLE_DITHERING, false);
            low.AddValue(ModSettingIDs.ENABLE_VIGNETTE, false);
            low.AddValue(ModSettingIDs.BLOOM_MODE, (int)BloomMode.Vanilla);
            low.AddValue(ModSettingIDs.ENABLE_DOF, false);
            low.AddValue(ModSettingIDs.ENABLE_SUN_SHAFTS, false);
            list.Add(low);

            ModSettingsPreset mid = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Good,
                AntiAliasingMode = AntiAliasingMode.MSAA2X
            };
            mid.AddValue(ModSettingIDs.ENABLE_SSAO, false);
            mid.AddValue(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION, false);
            mid.AddValue(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, false);
            mid.AddValue(ModSettingIDs.ENABLE_DITHERING, false);
            mid.AddValue(ModSettingIDs.ENABLE_VIGNETTE, false);
            mid.AddValue(ModSettingIDs.BLOOM_MODE, (int)BloomMode.Vanilla);
            mid.AddValue(ModSettingIDs.ENABLE_DOF, false);
            mid.AddValue(ModSettingIDs.ENABLE_SUN_SHAFTS, false);
            list.Add(mid);

            ModSettingsPreset standardVanilla = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Beautiful,
                AntiAliasingMode = AntiAliasingMode.MSAA4X
            };
            standardVanilla.AddValue(ModSettingIDs.ENABLE_SSAO, false);
            standardVanilla.AddValue(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION, false);
            standardVanilla.AddValue(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, false);
            standardVanilla.AddValue(ModSettingIDs.ENABLE_DITHERING, false);
            standardVanilla.AddValue(ModSettingIDs.ENABLE_VIGNETTE, false);
            standardVanilla.AddValue(ModSettingIDs.BLOOM_MODE, (int)BloomMode.Vanilla);
            standardVanilla.AddValue(ModSettingIDs.ENABLE_DOF, false);
            standardVanilla.AddValue(ModSettingIDs.ENABLE_SUN_SHAFTS, false);
            list.Add(standardVanilla);

            ModSettingsPreset standardOverhaul = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Beautiful,
                AntiAliasingMode = AntiAliasingMode.MSAA8X
            };
            standardOverhaul.AddValue(ModSettingIDs.ENABLE_SSAO, true);
            standardOverhaul.AddValue(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION, false);
            standardOverhaul.AddValue(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, false);
            standardOverhaul.AddValue(ModSettingIDs.ENABLE_DITHERING, false);
            standardOverhaul.AddValue(ModSettingIDs.ENABLE_VIGNETTE, true);
            standardOverhaul.AddValue(ModSettingIDs.BLOOM_MODE, (int)BloomMode.Fancy);
            standardOverhaul.AddValue(ModSettingIDs.ENABLE_DOF, false);
            standardOverhaul.AddValue(ModSettingIDs.ENABLE_SUN_SHAFTS, false);
            list.Add(standardOverhaul);

            ModSettingsPreset high = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Beautiful,
                AntiAliasingMode = AntiAliasingMode.MSAA8X
            };
            high.AddValue(ModSettingIDs.ENABLE_SSAO, true);
            high.AddValue(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION, false);
            high.AddValue(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, true);
            high.AddValue(ModSettingIDs.ENABLE_DITHERING, true);
            high.AddValue(ModSettingIDs.ENABLE_VIGNETTE, true);
            high.AddValue(ModSettingIDs.BLOOM_MODE, (int)BloomMode.Fancy);
            high.AddValue(ModSettingIDs.ENABLE_DOF, false);
            high.AddValue(ModSettingIDs.ENABLE_SUN_SHAFTS, false);
            list.Add(high);

            ModSettingsPreset veryHigh = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Beautiful,
                AntiAliasingMode = AntiAliasingMode.MSAA8X
            };
            veryHigh.AddValue(ModSettingIDs.ENABLE_SSAO, true);
            veryHigh.AddValue(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION, false);
            veryHigh.AddValue(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, true);
            veryHigh.AddValue(ModSettingIDs.ENABLE_DITHERING, true);
            veryHigh.AddValue(ModSettingIDs.ENABLE_VIGNETTE, true);
            veryHigh.AddValue(ModSettingIDs.BLOOM_MODE, (int)BloomMode.Fanciest);
            veryHigh.AddValue(ModSettingIDs.ENABLE_DOF, true);
            veryHigh.AddValue(ModSettingIDs.ENABLE_SUN_SHAFTS, true);
            list.Add(veryHigh);

            ModSettingsPreset extreme = new ModSettingsPreset(true)
            {
                QualityLevel = CloneDroneQualityLevels.Beautiful,
                AntiAliasingMode = AntiAliasingMode.MSAA8X
            };
            extreme.AddValue(ModSettingIDs.ENABLE_SSAO, true);
            extreme.AddValue(ModSettingIDs.ENABLE_GLOBAL_ILLUMINATION, true);
            extreme.AddValue(ModSettingIDs.ENABLE_CHROMATIC_ABERRATION, true);
            extreme.AddValue(ModSettingIDs.ENABLE_DITHERING, true);
            extreme.AddValue(ModSettingIDs.ENABLE_VIGNETTE, true);
            extreme.AddValue(ModSettingIDs.BLOOM_MODE, (int)BloomMode.Fanciest);
            extreme.AddValue(ModSettingIDs.ENABLE_DOF, true);
            extreme.AddValue(ModSettingIDs.ENABLE_SUN_SHAFTS, true);
            list.Add(extreme);
        }

        public void OnCameraSwitched(Camera oldCamera, Camera newCamera)
        {
            RemovePostEffectsFromCamera(oldCamera);
            AddPostEffectsToCamera(newCamera);
        }

        public void RefreshCameraPostEffects(bool refreshMainCamera)
        {
            CameraManager manager = CameraManager.Instance;
            manager.RefreshMainCamera();
            AddPostEffectsToCamera(manager.MainCamera);
        }

        public void RefreshCameraPostEffects()
        {
            AddPostEffectsToCamera(CameraManager.Instance.MainCamera);
        }

        public void RefreshCameraPostEffectsNextFrame()
        {
            _refreshEffectsNextFrame = true;
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

            Camera uiCamera = ModCache.UIRootCamera;
            OverhaulColorBlindness overhaulColorBlindnessOverUI = uiCamera.GetComponent<OverhaulColorBlindness>();
            if (!overhaulColorBlindnessOverUI && ColorBlindnessAffectUI && ColorBlindnessMode >= 1 && ColorBlindnessMode <= 3)
                overhaulColorBlindnessOverUI = uiCamera.gameObject.AddComponent<OverhaulColorBlindness>();

            if (overhaulColorBlindnessOverUI)
            {
                overhaulColorBlindnessOverUI.type = ColorBlindnessMode;
                overhaulColorBlindnessOverUI.enabled = ColorBlindnessAffectUI && ColorBlindnessMode >= 1 && ColorBlindnessMode <= 3;
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