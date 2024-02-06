using UnityEngine;

namespace OverhaulMod.Engine
{
    public class LightningInfo
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

        public LightningInfo()
        {

        }

        public LightningInfo(LevelLightSettings levelLightSettings)
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
        }
    }
}
