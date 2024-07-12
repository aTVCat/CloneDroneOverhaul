using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    public class RealisticLightUserSettings : MonoBehaviour, IDropdownOptions, ICustomHideFields
    {
        public static List<RealisticLightUserSettings> InstantiatedRealisticLightUserSettings = new List<RealisticLightUserSettings>();

        private LevelLightSettings m_levelLightSettings;

        [IncludeInLevelEditor]
        public bool EnableRealisticSkybox;

        [IncludeInLevelEditor]
        public int RealisticSkyBox;

        [IncludeInLevelEditor]
        public string AddonMessage;

        private void Awake()
        {
            m_levelLightSettings = base.GetComponent<LevelLightSettings>();
            if (!m_levelLightSettings)
            {
                base.enabled = false;
            }
            else
            {
                InstantiatedRealisticLightUserSettings.Add(this);
            }
        }

        private void OnDestroy()
        {
            _ = InstantiatedRealisticLightUserSettings.Remove(this);
        }

        private void Start()
        {
            AddonMessage = "(Overhaul Mod) This option requires \"Realistic skyboxes\" addon installed to work";
            if (GameModeManager.IsInLevelEditor())
            {
                ObjectPlacedInLevel objectPlacedInLevel = base.GetComponent<ObjectPlacedInLevel>();
                if (objectPlacedInLevel)
                {
                    objectPlacedInLevel.AddValueChangedListener(delegate (string value)
                    {
                        if (value == nameof(RealisticSkyBox) || value == nameof(EnableRealisticSkybox))
                        {

                        }
                    });
                }
            }
        }

        public bool IsActive()
        {
            return base.gameObject && base.gameObject.activeInHierarchy && EnableRealisticSkybox && m_levelLightSettings && (m_levelLightSettings.IsActive || !m_levelLightSettings.IsOverrideSettings);
        }

        public List<Dropdown.OptionData> GetDropdownOptions(string fieldName)
        {
            return null;
        }

        public bool HasDropDownForValue(string fieldName)
        {
            return false;
        }

        public bool ShouldShowDropdownOptions(string fieldName)
        {
            return false;
        }

        public bool ShouldHideField(string fieldName)
        {
            if (fieldName == nameof(EnableRealisticSkybox))
                return false;
            else if (fieldName == nameof(RealisticSkyBox))
            {
                return !EnableRealisticSkybox;
            }
            else if (fieldName == nameof(AddonMessage))
            {
                return !EnableRealisticSkybox;
            }
            return false;
        }

        public static RealisticLightUserSettings GetActiveSettings()
        {
            foreach (RealisticLightUserSettings settings in InstantiatedRealisticLightUserSettings)
            {
                if (settings && settings.IsActive())
                    return settings;
            }
            return null;
        }
    }
}
