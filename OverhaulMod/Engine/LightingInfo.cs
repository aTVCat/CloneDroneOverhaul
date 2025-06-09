using UnityEngine;

namespace OverhaulMod.Engine
{
    public class LightingInfo
    {
        public bool EnableDirectionalLight;

        public float DirectionalRotationX;

        public float DirectionalRotationY;

        public Color DirectionalColor;

        public float DirectionalIntensity;

        public float DirectionalShadowStrength;

        public bool AmbientUsesSkybox;

        public Color AmbientColor;

        public int SkyboxIndex;

        public string AdditonalSkybox;

        public float SunSize;

        public float SunSizeConvergence;

        public float AtmosphereThickness;

        public Color SkyTint;

        public Color GroundTint;

        public float SkyExposure;

        public Color SkyColor;

        public Color HorizonColor;

        public Color GroundColor;

        public float SkyTopExponent;

        public float SkyBottomExponent;

        public float SkyIntensity;

        public bool FogEnabled;

        public Color FogColor;

        public float FogStartDistance;

        public float FogEndDistance;

        public int CameraColorGrading;

        public float CameraColorBlend;

        public float CameraExposure;

        public LightingInfo()
        {

        }

        public LightingInfo(LevelLightSettings levelLightSettings)
        {
            SetValues(levelLightSettings);
        }

        public void SetValues(LevelLightSettings levelLightSettings)
        {
            EnableDirectionalLight = levelLightSettings.EnableDirectionalLight;
            DirectionalRotationX = levelLightSettings.DirectionalRotationX;
            DirectionalRotationY = levelLightSettings.DirectionalRotationY;
            DirectionalColor = levelLightSettings.DirectionalColor;
            DirectionalIntensity = levelLightSettings.DirectionalIntensity;
            DirectionalShadowStrength = levelLightSettings.DirectionalShadowStrength;
            AmbientUsesSkybox = levelLightSettings.AmbientUsesSkybox;
            AmbientColor = levelLightSettings.AmbientColor;
            SkyboxIndex = levelLightSettings.SkyboxIndex;
            SunSize = levelLightSettings.SunSize;
            SunSizeConvergence = levelLightSettings.SunSizeConvergence;
            AtmosphereThickness = levelLightSettings.AtmosphereThickness;
            SkyTint = levelLightSettings.SkyTint;
            GroundTint = levelLightSettings.GroundTint;
            SkyExposure = levelLightSettings.SkyExposure;
            SkyColor = levelLightSettings.SkyColor;
            HorizonColor = levelLightSettings.HorizonColor;
            GroundColor = levelLightSettings.GroundColor;
            SkyTopExponent = levelLightSettings.SkyTopExponent;
            SkyBottomExponent = levelLightSettings.SkyBottomExponent;
            SkyIntensity = levelLightSettings.SkyIntensity;
            FogEnabled = levelLightSettings.FogEnabled;
            FogColor = levelLightSettings.FogColor;
            FogStartDistance = levelLightSettings.FogStartDistance;
            FogEndDistance = levelLightSettings.FogEndDistance;
            CameraColorGrading = levelLightSettings.CameraColorGrading;
            CameraColorBlend = levelLightSettings.CameraColorBlend;
            CameraExposure = levelLightSettings.CameraExposure;

            AdditionalSkyboxSettings realisticLightUserSettings = levelLightSettings.GetComponent<AdditionalSkyboxSettings>();
            if (realisticLightUserSettings)
                AdditonalSkybox = realisticLightUserSettings.Skybox;
            else
                AdditonalSkybox = null;
        }

        public void SetValuesUsingEnvironmentSettings()
        {
            EnableDirectionalLight = DirectionalLightManager.Instance.DirectionalLight.gameObject.activeInHierarchy;
            DirectionalRotationX = DirectionalLightManager.Instance.DirectionalLight.transform.eulerAngles.x;
            DirectionalRotationY = DirectionalLightManager.Instance.DirectionalLight.transform.eulerAngles.y;
            DirectionalColor = DirectionalLightManager.Instance.DirectionalLight.color;
            DirectionalIntensity = DirectionalLightManager.Instance.DirectionalLight.intensity;
            DirectionalShadowStrength = DirectionalLightManager.Instance.DirectionalLight.shadowStrength;
            AmbientUsesSkybox = RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Skybox;
            AmbientColor = RenderSettings.ambientLight;
            FogEnabled = RenderSettings.fog;
            FogColor = RenderSettings.fogColor;
            FogStartDistance = RenderSettings.fogStartDistance;
            FogEndDistance = RenderSettings.fogEndDistance;

            SkyBoxManager skyBoxManager = SkyBoxManager.Instance;
            Material material = skyBoxManager._customSkybox;
            if (material)
            {
                SunSize = material.GetFloat("_SunSize");
                SunSizeConvergence = material.GetFloat("_SunSizeConvergence");
                AtmosphereThickness = material.GetFloat("_AtmosphereThickness");
                SkyTint = material.GetColor("_SkyTint");
                GroundTint = material.GetColor("_GroundColor");
                SkyExposure = material.GetFloat("_Exposure");
            }

            Material material2 = skyBoxManager._gradientSkybox;
            if (material2)
            {
                SkyColor = material2.GetColor("_Color1");
                HorizonColor = material2.GetColor("_Color2");
                GroundColor = material2.GetColor("_Color3");
                SkyTopExponent = material2.GetFloat("_Exponent1");
                SkyBottomExponent = material2.GetFloat("_Exponent2");
                SkyIntensity = material2.GetFloat("_Intensity");
            }

            LevelLightSettings levelLightSettings = LevelEditorLightManager.Instance?.GetActiveLightSettings();
            if (!levelLightSettings)
                return;

            CameraColorGrading = levelLightSettings.CameraColorGrading;
            CameraColorBlend = levelLightSettings.CameraColorBlend;
            CameraExposure = levelLightSettings.CameraExposure;
        }

        public void ApplyValues(LevelLightSettings levelLightSettings)
        {
            if (!levelLightSettings)
                return;

            levelLightSettings.EnableDirectionalLight = EnableDirectionalLight;
            levelLightSettings.DirectionalRotationX = DirectionalRotationX;
            levelLightSettings.DirectionalRotationY = DirectionalRotationY;
            levelLightSettings.DirectionalColor = DirectionalColor;
            levelLightSettings.DirectionalIntensity = DirectionalIntensity;
            levelLightSettings.DirectionalShadowStrength = DirectionalShadowStrength;
            levelLightSettings.AmbientUsesSkybox = AmbientUsesSkybox;
            levelLightSettings.AmbientColor = AmbientColor;
            levelLightSettings.SkyboxIndex = SkyboxIndex;
            levelLightSettings.SunSize = SunSize;
            levelLightSettings.SunSizeConvergence = SunSizeConvergence;
            levelLightSettings.AtmosphereThickness = AtmosphereThickness;
            levelLightSettings.SkyTint = SkyTint;
            levelLightSettings.GroundTint = GroundTint;
            levelLightSettings.SkyExposure = SkyExposure;
            levelLightSettings.SkyColor = SkyColor;
            levelLightSettings.HorizonColor = HorizonColor;
            levelLightSettings.GroundColor = GroundColor;
            levelLightSettings.SkyTopExponent = SkyTopExponent;
            levelLightSettings.SkyBottomExponent = SkyBottomExponent;
            levelLightSettings.SkyIntensity = SkyIntensity;
            levelLightSettings.FogEnabled = FogEnabled;
            levelLightSettings.FogColor = FogColor;
            levelLightSettings.FogStartDistance = FogStartDistance;
            levelLightSettings.FogEndDistance = FogEndDistance;
            levelLightSettings.CameraColorGrading = CameraColorGrading;
            levelLightSettings.CameraColorBlend = CameraColorBlend;
            levelLightSettings.CameraExposure = CameraExposure;

            ObjectPlacedInLevel opil = levelLightSettings.GetComponent<ObjectPlacedInLevel>();
            if (opil)
            {
                opil.SetCustomInspectorStringValue(nameof(AdditionalSkyboxSettings), nameof(AdditionalSkyboxSettings.Skybox), AdditonalSkybox);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (!(obj is LightingInfo lightingInfo))
                return false;

            if (EnableDirectionalLight != lightingInfo.EnableDirectionalLight)
                return false;
            if (DirectionalRotationX != lightingInfo.DirectionalRotationX)
                return false;
            if (DirectionalRotationY != lightingInfo.DirectionalRotationY)
                return false;
            if (DirectionalColor != lightingInfo.DirectionalColor)
                return false;
            if (DirectionalIntensity != lightingInfo.DirectionalIntensity)
                return false;
            if (DirectionalShadowStrength != lightingInfo.DirectionalShadowStrength)
                return false;
            if (AmbientUsesSkybox != lightingInfo.AmbientUsesSkybox)
                return false;
            if (AmbientColor != lightingInfo.AmbientColor)
                return false;
            if (SkyboxIndex != lightingInfo.SkyboxIndex)
                return false;
            if (SunSize != lightingInfo.SunSize)
                return false;
            if (SunSizeConvergence != lightingInfo.SunSizeConvergence)
                return false;
            if (AtmosphereThickness != lightingInfo.AtmosphereThickness)
                return false;
            if (SkyTint != lightingInfo.SkyTint)
                return false;
            if (GroundTint != lightingInfo.GroundTint)
                return false;
            if (SkyExposure != lightingInfo.SkyExposure)
                return false;
            if (SkyColor != lightingInfo.SkyColor)
                return false;
            if (HorizonColor != lightingInfo.HorizonColor)
                return false;
            if (GroundColor != lightingInfo.GroundColor)
                return false;
            if (SkyTopExponent != lightingInfo.SkyTopExponent)
                return false;
            if (SkyBottomExponent != lightingInfo.SkyBottomExponent)
                return false;
            if (SkyIntensity != lightingInfo.SkyIntensity)
                return false;
            if (FogEnabled != lightingInfo.FogEnabled)
                return false;
            if (FogColor != lightingInfo.FogColor)
                return false;
            if (FogStartDistance != lightingInfo.FogStartDistance)
                return false;
            if (FogEndDistance != lightingInfo.FogEndDistance)
                return false;
            if (CameraColorGrading != lightingInfo.CameraColorGrading)
                return false;
            if (CameraColorBlend != lightingInfo.CameraColorBlend)
                return false;
            if (CameraExposure != lightingInfo.CameraExposure)
                return false;

            return true;
        }
    }
}
