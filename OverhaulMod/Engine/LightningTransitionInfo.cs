using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace OverhaulMod.Engine
{
    public class LightningTransitionInfo
    {
        public LightningInfo LightningA, LightningB;

        private float m_completion;
        public float completion
        {
            get
            {
                return m_completion;
            }
            set
            {
                m_completion = value;
                setTransitionProgress(value);
            }
        }

        public LightningTransitionInfo()
        {

        }

        public bool CanDoTransition()
        {
            return LightningA != null && LightningB != null;
        }

        private void setTransitionProgress(float d)
        {
            if (!CanDoTransition())
                return;

            LightningInfo a = LightningA;
            LightningInfo b = LightningB;

            float startAtmosphereThickness = a.SkyboxIndex == 3 && b.SkyboxIndex == 1 ? 1f : a.AtmosphereThickness;
            Color startSkyTint = a.SkyboxIndex == 3 && b.SkyboxIndex == 1 ? a.SkyColor : a.SkyTint;
            Color startSkyColor = a.SkyboxIndex == 1 && b.SkyboxIndex == 3 ? a.SkyTint : a.SkyColor;

            refreshAmbient(b.AmbientUsesSkybox,
                ModUnityUtils.LerpRGB(a.AmbientColor, b.AmbientColor, d));

            refreshSkybox(b.SkyboxIndex,
                Mathf.Lerp(a.SkyExposure, b.SkyExposure, d),
                Mathf.Lerp(a.SunSize, b.SunSize, d),
                Mathf.Lerp(a.SunSizeConvergence, b.SunSizeConvergence, d),
                Mathf.Lerp(startAtmosphereThickness, b.AtmosphereThickness, d),
                ModUnityUtils.LerpRGB(startSkyTint, b.SkyTint, d),
                ModUnityUtils.LerpRGB(a.GroundTint, b.GroundTint, d),
                ModUnityUtils.LerpRGB(startSkyColor, b.SkyColor, d),
                ModUnityUtils.LerpRGB(a.HorizonColor, b.HorizonColor, d),
                ModUnityUtils.LerpRGB(a.GroundColor, b.GroundColor, d),
                Mathf.Lerp(a.SkyTopExponent, b.SkyTopExponent, d),
                Mathf.Lerp(a.SkyBottomExponent, b.SkyBottomExponent, d),
                Mathf.Lerp(a.SkyIntensity, b.SkyIntensity, d));

            refreshFog(b.FogEnabled,
                Mathf.Lerp(a.FogEndDistance, b.FogEndDistance, d),
                Mathf.Lerp(a.FogStartDistance, b.FogStartDistance, d),
                ModUnityUtils.LerpRGB(a.FogColor, b.FogColor, d));

            refreshDirectionalLight(d < 0.5f ? a.EnableDirectionalLight : b.EnableDirectionalLight,
                ModUnityUtils.LerpRGB(a.DirectionalColor, b.DirectionalColor, d),
                d < 0.5f ? Mathf.Lerp(a.DirectionalIntensity, 0f, d * 2f) : Mathf.Lerp(0f, b.DirectionalIntensity, (d * 2f) - 1f),
                d < 0.5f ? a.DirectionalRotationX : b.DirectionalRotationX,
                d < 0.5f ? a.DirectionalRotationY : b.DirectionalRotationY,
                Mathf.Lerp(a.DirectionalShadowStrength, b.DirectionalShadowStrength, d));

            GlobalEventManager.Instance.Dispatch(GlobalEvents.LightSettingsRefreshed);
        }

        private void refreshAmbient(bool ambientUsesSkyBox, Color color)
        {
            if (ambientUsesSkyBox)
            {
                RenderSettings.ambientMode = AmbientMode.Skybox;
            }
            else
            {
                RenderSettings.ambientMode = AmbientMode.Flat;
                RenderSettings.ambientLight = color;
            }
        }

        private void refreshSkybox(int index,
            float skyExposure,
            float sunSize,
            float sunSizeConvergence,
            float atmosphereThickness,
            Color skyTint,
            Color groundTint,
            Color skyColor,
            Color horizonColor,
            Color groundColor,
            float skyTopExponent,
            float skyBottomExponent,
            float skyIntensity)
        {
            SkyBoxManager skyBoxManager = SkyBoxManager.Instance;
            Material material;

            Material customSkybox = skyBoxManager._customSkybox;
            if (!customSkybox)
            {
                customSkybox = new Material(skyBoxManager.LevelConfigurableSkyboxes[1]);
                skyBoxManager._customSkybox = customSkybox;
            }
            customSkybox.SetFloat("_SunSize", sunSize);
            customSkybox.SetFloat("_SunSizeConvergence", sunSizeConvergence);
            customSkybox.SetFloat("_AtmosphereThickness", atmosphereThickness);
            customSkybox.SetColor("_SkyTint", skyTint);
            customSkybox.SetColor("_GroundColor", groundTint);
            customSkybox.SetFloat("_Exposure", skyExposure);

            Material gradientSkybox = skyBoxManager._gradientSkybox;
            if (!gradientSkybox)
            {
                gradientSkybox = new Material(skyBoxManager.LevelConfigurableSkyboxes[3]);
                skyBoxManager._gradientSkybox = gradientSkybox;
            }
            gradientSkybox.SetColor("_Color1", skyColor);
            gradientSkybox.SetColor("_Color2", horizonColor);
            gradientSkybox.SetColor("_Color3", groundColor);
            gradientSkybox.SetFloat("_Exponent1", skyTopExponent);
            gradientSkybox.SetFloat("_Exponent2", skyBottomExponent);
            gradientSkybox.SetFloat("_Intensity", skyIntensity);

            material = index == 1 ? customSkybox : index == 3 ? gradientSkybox : skyBoxManager.LevelConfigurableSkyboxes[index];

            if (material != skyBoxManager._currentSkybox)
            {
                skyBoxManager._currentSkybox = material;
                RenderSettings.skybox = material;
            }
        }

        private void refreshFog(bool enable, float end, float start, Color color)
        {
            RenderSettings.fog = enable;
            RenderSettings.fogEndDistance = end;
            RenderSettings.fogStartDistance = start;
            RenderSettings.fogColor = color;
        }

        public void refreshDirectionalLight(bool enable, Color color, float intensity, float x, float y, float shadowStrength)
        {
            DirectionalLightManager directionalLightManager = DirectionalLightManager.Instance;

            Light light = directionalLightManager?.DirectionalLight;
            if (!light)
                return;

            light.gameObject.SetActive(enable);
            light.color = color;
            light.intensity = intensity;
            light.transform.localEulerAngles = new Vector3(x, y, 0f);
            light.shadowStrength = shadowStrength;
        }
    }
}
