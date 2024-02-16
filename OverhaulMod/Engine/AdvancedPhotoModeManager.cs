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
                new AdvancedPhotoModeProperty(typeof(AdvancedPhotoModeManager), nameof(directionalLightY))
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

            foreach(var p in m_properties)
            {
                if(p.classType == type && p.classMemberName == memberName)
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

            foreach (var p in m_properties)
                if (p.classType == type && p.classMemberName == memberName)
                {
                    if (p.moddedValue == null)
                        return (T)p.GetValue();
                    else
                        return (T)p.moddedValue;
                }

            return default;
        }

        private void onEnteredPhotoMode()
        {
        }

        private void onExitedPhotoMode()
        {
            LevelEditorLightManager.Instance.RefreshLightInScene();
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
                foreach (var p in m_properties)
                    p.SetModdedValue();
        }
    }
}
