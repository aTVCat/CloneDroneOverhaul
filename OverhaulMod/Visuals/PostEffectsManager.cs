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

        private CameraManager m_cameraManager;

        private float m_timeLeftToRefreshReflectionProbe;

        private bool m_reflectionProbeEnabled;

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

            if (!ModFeatures.IsEnabled(ModFeatures.FeatureType.ReflectionProbe) || !ModFeatures.IsEnabled(ModFeatures.FeatureType.MoreImageEffects))
                return;

            createReflectionProbe();
            m_reflectionProbeEnabled = true;
        }

        private void Update()
        {
            if (!m_reflectionProbeEnabled)
                return;

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
