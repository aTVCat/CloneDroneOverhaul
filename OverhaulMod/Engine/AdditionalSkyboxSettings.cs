using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
    [LevelEditorInspectorDisplayNameOverride("[Overhaul mod] Realistic skybox")]
    public class AdditionalSkyboxSettings : MonoBehaviour, IDropdownOptions, ICustomHideFields
    {
        [IncludeInLevelEditor]
        public string Skybox;

        private void Start()
        {
            if (GameModeManager.IsInLevelEditor())
            {
                ObjectPlacedInLevel objectPlacedInLevel = base.GetComponent<ObjectPlacedInLevel>();
                if (objectPlacedInLevel)
                {
                    objectPlacedInLevel.AddValueChangedListener(delegate (string value)
                    {
                        if (value == nameof(Skybox))
                        {
                            SkyBoxManager.Instance.RefreshSkyboxAmbientLightAndFog(LevelEditorLightManager.Instance.GetActiveLightSettings());
                        }
                    });
                }
            }
            else
            {
                RealisticLightingInfo realisticLightingInfo = RealisticLightingManager.Instance.GetCurrentRealisticLightingInfo();
                if (realisticLightingInfo != null)
                {
                    Skybox = realisticLightingInfo.SkyboxName;
                }
            }
        }

        private void OnDestroy()
        {
            SkyBoxManager.Instance.RefreshSkyboxAmbientLightAndFog(LevelEditorLightManager.Instance.GetActiveLightSettings());
        }

        public List<Dropdown.OptionData> GetDropdownOptions(string fieldName)
        {
            if (fieldName == nameof(Skybox))
            {
                return AdditionalSkyboxesManager.Instance.GetSkyboxOptionsForLevelEditor(Skybox);
            }
            return null;
        }

        public bool HasDropDownForValue(string fieldName)
        {
            return fieldName == nameof(Skybox);
        }

        public bool ShouldShowDropdownOptions(string fieldName)
        {
            return fieldName == nameof(Skybox);
        }

        public bool ShouldHideField(string fieldName)
        {
            return false;
        }
    }
}
