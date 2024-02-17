using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class AdvancedPhotoModeManager : Singleton<AdvancedPhotoModeManager>, IGameLoadListener
    {
        private List<AdvancedPhotoModeProperty> m_properties;

        public static bool directionalLight
        {
            get
            {
                return DirectionalLightManager.Instance.DirectionalLight.gameObject.activeInHierarchy;
            }
            set
            {
                DirectionalLightManager.Instance.DirectionalLight.gameObject.SetActive(value);
            }
        }

        public static Color directionalLightColor
        {
            get
            {
                return DirectionalLightManager.Instance.DirectionalLight.color;
            }
            set
            {
                DirectionalLightManager.Instance.DirectionalLight.color = value;
            }
        }

        public static float directionalLightX
        {
            get
            {
                return DirectionalLightManager.Instance.DirectionalLight.transform.localEulerAngles.x;
            }
            set
            {
                DirectionalLightManager.Instance.DirectionalLight.transform.localEulerAngles = new Vector3(value, directionalLightY, 0f);
            }
        }

        public static float directionalLightY
        {
            get
            {
                return DirectionalLightManager.Instance.DirectionalLight.transform.localEulerAngles.y;
            }
            set
            {
                DirectionalLightManager.Instance.DirectionalLight.transform.localEulerAngles = new Vector3(directionalLightX, value, 0f);
            }
        }

        public static float directionalLightIntensity
        {
            get
            {
                return DirectionalLightManager.Instance.DirectionalLight.intensity;
            }
            set
            {
                DirectionalLightManager.Instance.DirectionalLight.intensity = value;
            }
        }

        public static float directionalLightShadows
        {
            get
            {
                return DirectionalLightManager.Instance.DirectionalLight.shadowStrength;
            }
            set
            {
                DirectionalLightManager.Instance.DirectionalLight.shadowStrength = value;
            }
        }

        private static int s_skyBoxIndex;
        public static int skyBoxIndex
        {
            get
            {
                return s_skyBoxIndex;
            }
            set
            {
                SkyBoxManager skyBoxManager = SkyBoxManager.Instance;
                Material material;
                if (value == 1)
                {
                    if (!skyBoxManager._customSkybox)
                    {
                        skyBoxManager._customSkybox = new Material(skyBoxManager.LevelConfigurableSkyboxes[1]);
                    }
                    material = skyBoxManager._customSkybox;
                }
                else if (value == 3)
                {
                    if (!skyBoxManager._gradientSkybox)
                    {
                        skyBoxManager._gradientSkybox = new Material(skyBoxManager.LevelConfigurableSkyboxes[3]);
                    }
                    material = skyBoxManager._gradientSkybox;
                }
                else
                {
                    material = skyBoxManager.LevelConfigurableSkyboxes[value];
                }
                RenderSettings.skybox = material;
                s_skyBoxIndex = value;
            }
        }

        private static bool s_useRealisticSkyBoxes;
        public static bool useRealisticSkyBoxes
        {
            get
            {
                return s_useRealisticSkyBoxes;
            }
            set
            {
                if (value)
                    RealisticLightningManager.Instance.SetSkybox(realisticSkyBoxIndex);

                s_useRealisticSkyBoxes = value;
            }
        }

        private static int s_realisticSkyBoxIndex;
        public static int realisticSkyBoxIndex
        {
            get
            {
                return s_realisticSkyBoxIndex;
            }
            set
            {
                if (useRealisticSkyBoxes)
                    RealisticLightningManager.Instance.SetSkybox(value);
                s_realisticSkyBoxIndex = Mathf.Clamp(value, 0, 14);
            }
        }

        private void Start()
        {
            m_properties = new List<AdvancedPhotoModeProperty>
            {
                new AdvancedPhotoModeProperty(typeof(RenderSettings), nameof(RenderSettings.fogStartDistance)),
                new AdvancedPhotoModeProperty(typeof(RenderSettings), nameof(RenderSettings.fogEndDistance)),
                new AdvancedPhotoModeProperty(typeof(RenderSettings), nameof(RenderSettings.fog)),
                new AdvancedPhotoModeProperty(typeof(RenderSettings), nameof(RenderSettings.fogColor)),
                new AdvancedPhotoModeProperty(typeof(AdvancedPhotoModeManager), nameof(directionalLight)),
                new AdvancedPhotoModeProperty(typeof(AdvancedPhotoModeManager), nameof(directionalLightColor)),
                new AdvancedPhotoModeProperty(typeof(AdvancedPhotoModeManager), nameof(directionalLightX)),
                new AdvancedPhotoModeProperty(typeof(AdvancedPhotoModeManager), nameof(directionalLightY)),
                new AdvancedPhotoModeProperty(typeof(AdvancedPhotoModeManager), nameof(directionalLightIntensity)),
                new AdvancedPhotoModeProperty(typeof(AdvancedPhotoModeManager), nameof(directionalLightShadows)),
                new AdvancedPhotoModeProperty(typeof(AdvancedPhotoModeManager), nameof(skyBoxIndex)),
                new AdvancedPhotoModeProperty(typeof(AdvancedPhotoModeManager), nameof(useRealisticSkyBoxes)),
                new AdvancedPhotoModeProperty(typeof(AdvancedPhotoModeManager), nameof(realisticSkyBoxIndex))
            };
        }

        public void OnGameLoaded()
        {
            GlobalEventManager.Instance.AddEventListener("EnteredPhotoMode", onEnteredPhotoMode);
            GlobalEventManager.Instance.AddEventListener("ExitedPhotoMode", onExitedPhotoMode);
        }

        public void SetPropertyValue(Type type, string memberName, object value)
        {
            if (m_properties.IsNullOrEmpty())
                return;

            foreach (AdvancedPhotoModeProperty p in m_properties)
            {
                if (p.classType == type && p.classMemberName == memberName)
                {
                    p.SetValue(value);
                    return;
                }
            }
        }

        public T GetPropertyModdedValue<T>(Type type, string memberName)
        {
            if (m_properties.IsNullOrEmpty())
                return default;

            foreach (AdvancedPhotoModeProperty p in m_properties)
                if (p.classType == type && p.classMemberName == memberName)
                {
                    return p.moddedValue == null ? (T)p.GetValue() : (T)p.moddedValue;
                }

            return default;
        }

        private void onEnteredPhotoMode()
        {
        }

        private void onExitedPhotoMode()
        {
            RealisticLightningManager.Instance.PatchLightning(true);
            RecoverEnvironmentSettings();
        }

        public void SetDefaultValues()
        {
            foreach (AdvancedPhotoModeProperty property in m_properties)
                property.moddedValue = property.GetValue();
        }

        public void RecoverEnvironmentSettings()
        {
            LevelEditorLightManager.Instance.RefreshLightInScene();
        }

        public void TemporaryRecoverEnvironmentSettings(bool value)
        {
            if (value)
                RecoverEnvironmentSettings();
            else
                foreach (AdvancedPhotoModeProperty p in m_properties)
                    p.SetModdedValue();
        }
    }
}
