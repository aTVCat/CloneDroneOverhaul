using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class AdvancedPhotoModeManager : Singleton<AdvancedPhotoModeManager>
    {
        private List<AdvancedPhotoModeProperty> m_properties;

        private void Start()
        {
            m_properties = new List<AdvancedPhotoModeProperty>
            {
                new AdvancedPhotoModeProperty("Fog start distance", "Environment", true, typeof(RenderSettings), nameof(RenderSettings.fogStartDistance)),
                new AdvancedPhotoModeProperty("Fog end distance", "Environment", true, typeof(RenderSettings), nameof(RenderSettings.fogEndDistance))
            };
        }

        public void SaveEnvironmentSettings()
        {
            foreach(var property in m_properties)
            {
                if(!property.disallowSavingValue)
                    property.SaveValue();
            }
        }

        public void RecoverEnvironmentSettings()
        {
            LevelEditorLightManager.Instance.RefreshLightInScene();
            foreach (var property in m_properties)
            {
                if (!property.disallowSavingValue)
                    property.RestoreValue();
            }
        }
    }
}
