using AmplifyOcclusion;
using CloneDroneOverhaul.PooledPrefabs;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;

namespace CloneDroneOverhaul.Modules
{
    public class VisualsModule : ModuleBase
    {
        private AmplifyOcclusionEffect Occlusion;
        private ParticleSystem worldDustMS1;
        private ParticleSystem worldDustMS0;
        private ParticleSystem worldDustNormal;
        private Camera lastSpottedCamera;
        private List<AmplifyOcclusionEffect> effects = new List<AmplifyOcclusionEffect>();

        private bool _AOEnabled;
        private int _AOSampleCount;
        private float _AOIntensity;

        public float NoiseMultipler;
        private bool _dustEnabled;
        public bool NoiseEnabled;

        private int _shadowResolution;
        private int _shadowBias;
        private int _shadowDistance;
        private bool _softShadows;

        private bool _bloomEnabled;
        private int _bloomIterations;
        private float _bloomThreshold;
        private float _bloomIntensity;


        public bool OverrideSettings;

        public bool Override_AOEnabled;
        public int Override_AOSampleCount;
        public float Override_AOIntensity;

        public float Override_NoiseMultipler;
        public bool Override_NoiseEnabled;

        public bool Override_DustEnabled;

        public int Override_ShadowResolution;
        public int Override_ShadowBias;
        private int Override_ShadowDistance;
        public bool Override_SoftShadows;

        public bool Override_BloomEnabled;
        public int Override_BloomIterations;
        public float Override_BloomThreshold;
        public float Override_BloomIntensity;

        public override void Start()
        {
            worldDustMS1 = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustM1")).GetComponent<ParticleSystem>();
            worldDustMS0 = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustM0")).GetComponent<ParticleSystem>();
            worldDustNormal = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustNormal")).GetComponent<ParticleSystem>();

            RefreshDustMaterials();
        }

        public bool IsDustEnabled()
        {
            return OverrideSettings ? Override_DustEnabled : _dustEnabled;
        }

        public override void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false)
        {
            if (ID == "Graphics.Additions.Noise Multipler")
            {
                NoiseMultipler = (float)value;
            }
            if (ID == "Graphics.Additions.Sample count")
            {
                _AOSampleCount = (int)value;
            }
            if (ID == "Graphics.Additions.Amplify occlusion")
            {
                _AOEnabled = (bool)value;
            }
            if (ID == "Graphics.World.Floating dust")
            {
                _dustEnabled = (bool)value;
            }
            if (ID == "Graphics.Additions.Occlusion intensity")
            {
                _AOIntensity = (float)value;
            }
            if (ID == "Graphics.Settings.Shadow resolution")
            {
                _shadowResolution = (int)value;
            }
            if (ID == "Graphics.Settings.Shadow bias")
            {
                _shadowBias = (int)value;
            }
            if (ID == "Graphics.Settings.Soft shadows")
            {
                _softShadows = (bool)value;
            }
            if (ID == "Graphics.Settings.Shadow distance")
            {
                _shadowDistance = (int)value;
            }
            RefreshVisuals();
        }

        public void RefreshVisuals()
        {
            UI.VisualEffectsUI.Instance.RefreshEffects();
            if (Occlusion != null)
            {
                Occlusion.Intensity = (OverrideSettings ? Override_AOIntensity : _AOIntensity);
                Occlusion.enabled = (OverrideSettings ? Override_AOEnabled : _AOEnabled);
                Occlusion.SampleCount = (SampleCountLevel)(OverrideSettings ? Override_AOSampleCount : _AOSampleCount);
            }
            Light light = DirectionalLightManager.Instance.DirectionalLight;
            if (light != null)
            {
                ShadowResolution enumRes = (ShadowResolution)(OverrideSettings ? Override_ShadowResolution : _shadowResolution);
                switch (enumRes)
                {
                    case ShadowResolution.Low:
                        light.shadowCustomResolution = 1000;
                        break;
                    case ShadowResolution.Default:
                        light.shadowCustomResolution = -1;
                        break;
                    case ShadowResolution.High:
                        light.shadowCustomResolution = 5000;
                        break;
                    case ShadowResolution.ExtremlyHigh:
                        light.shadowCustomResolution = 10000;
                        break;
                }

                ShadowBias shadowBias = (ShadowBias)(OverrideSettings ? Override_ShadowBias : _shadowBias);
                switch (shadowBias)
                {
                    case ShadowBias.Minimum:
                        light.shadowBias = 0;
                        break;
                    case ShadowBias.Low:
                        light.shadowBias = 0.2f;
                        break;
                    case ShadowBias.Default:
                        light.shadowBias = 1f;
                        break;
                }

                ShadowDistance shadowDistance = (ShadowDistance)(OverrideSettings ? Override_ShadowDistance : _shadowDistance);
                switch (shadowDistance)
                {
                    case ShadowDistance.Default:
                        QualitySettings.shadowDistance = 300;
                        break;
                    case ShadowDistance.VeryLow:
                        QualitySettings.shadowDistance = 100;
                        break;
                    case ShadowDistance.Low:
                        QualitySettings.shadowDistance = 200;
                        break;
                    case ShadowDistance.High:
                        QualitySettings.shadowDistance = 500;
                        break;
                    case ShadowDistance.VeryHigh:
                        QualitySettings.shadowDistance = 750;
                        break;
                    case ShadowDistance.ExctremlyHigh:
                        QualitySettings.shadowDistance = 1000;
                        break;
                }

                LightShadows shadowsMode = LightShadows.Soft;
                int qualityLevel = SettingsManager.Instance.GetSavedQualityIndex();
                if (qualityLevel == 0)
                {
                    shadowsMode = LightShadows.None;
                }
                else
                {
                    shadowsMode = (OverrideSettings ? Override_SoftShadows : _softShadows) ? LightShadows.Soft : LightShadows.Hard;
                }
                light.shadows = shadowsMode;
            }
            if (!_dustEnabled)
            {
                worldDustNormal.Clear();
                worldDustMS0.Clear();
                worldDustMS1.Clear();
            }
        }

        public override void OnNewFrame()
        {
            Camera newCam = PlayerCameraManager.Instance.GetMainCamera();
            if (lastSpottedCamera != newCam)
            {
                if (newCam != null)
                {
                    Bloom bloom = newCam.GetComponent<Bloom>();
                    bloom.bloomBlurIterations = 10;
                    bloom.bloomIntensity = 0.7f;
                    bloom.bloomThreshold = 1.25f;
                    bloom.bloomThresholdColor = new Color(1, 1, 0.75f, 1);

                    AmplifyOcclusionEffect acc = newCam.gameObject.AddComponent<AmplifyOcclusionEffect>();
                    acc.Intensity = OverhaulMain.GetSetting<float>("Graphics.Additions.Occlusion intensity");
                    acc.BlurSharpness = 4f;
                    acc.FilterResponse = 0.7f;
                    acc.SampleCount = (SampleCountLevel)(OverrideSettings ? Override_AOSampleCount : _AOSampleCount);
                    acc.Bias = 1;
                    Occlusion = acc;
                    acc.enabled = (OverrideSettings ? Override_AOEnabled : _AOEnabled);

                    if (effects.Count != 0)
                    {
                        for (int i = effects.Count - 1; i > -1; i--)
                        {
                            if (effects[i] == null || effects[i].gameObject == null)
                            {
                                effects.RemoveAt(i);
                            }
                            else
                            {
                                if (effects[i].GetComponent<Camera>() != PlayerCameraManager.Instance.GetMainCamera())
                                {
                                    GameObject.Destroy(effects[i]);
                                }
                            }

                        }
                    }
                    effects.Add(acc);
                }
            }
            lastSpottedCamera = newCam;
        }
        public override void OnManagedUpdate()
        {
            if (Occlusion != null)
            {
                Occlusion.enabled = !GameModeManager.IsInLevelEditor() && (OverrideSettings ? Override_AOEnabled : _AOEnabled);
            }
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 pos = cam.transform.position;
                worldDustMS1.gameObject.transform.position = pos;
                worldDustMS0.gameObject.transform.position = pos;
                worldDustNormal.gameObject.transform.position = pos;
            }
            else
            {
                worldDustMS1.gameObject.transform.position = Vector3.zero;
                worldDustMS0.gameObject.transform.position = Vector3.zero;
                worldDustNormal.gameObject.transform.position = Vector3.zero;
            }
        }
        public override void OnSecond(float time)
        {
            if (time == 0.5f)
            {
                RefreshDustMaterials();
            }
        }

        private void RefreshDustMaterials()
        {
            worldDustNormal.Stop();
            worldDustMS0.Stop();
            worldDustMS1.Stop();

            if (!_dustEnabled)
            {
                return;
            }

            RobotShortInformation info = PlayerUtilities.GetPlayerRobotInfo();
            bool useMindspace = false;
            if (!info.IsNull)
            {
                useMindspace = info.IsFPMMindspace;
            }

            if (useMindspace)
            {
                worldDustMS0.Play();
                worldDustMS1.Play();
            }
            else
            {
                worldDustNormal.Play();
            }
        }
    }

    public class PointLightDust : MonoBehaviour
    {
        public Transform Target;
        private Transform Dust;

        private void Start()
        {
            Dust = Instantiate<Transform>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustLight").transform);
        }

        private void FixedUpdate()
        {
            Dust.transform.position = Target.position;
        }

        private void OnDestroy()
        {
            if (Dust != null)
            {
                Destroy(Dust.gameObject);
            }
        }

        private void OnDisable()
        {
            if (Dust != null)
            {
                Dust.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (Dust != null)
            {
                Dust.gameObject.SetActive(true);
            }
        }
    }

    public enum ShadowResolution
    {
        Low,
        Default,
        High,
        ExtremlyHigh
    }

    public enum ShadowBias
    {
        Minimum,
        Low,
        Default,
    }

    public enum ShadowDistance
    {
        VeryLow,
        Low,
        Default,
        High,
        VeryHigh,
        ExctremlyHigh
    }

    public enum LightLimit
    {
        Low,
        Default,
        High,
        ExtremlyHigh
    }

    public enum AntialiasingLevel
    {
        Off,
        X2,
        X4,
        X8
    }
}
