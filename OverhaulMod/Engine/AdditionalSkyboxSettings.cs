using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Engine
{
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
                        if(value == nameof(Skybox))
                        {
                            SkyBoxManager.Instance.RefreshSkyboxAmbientLightAndFog(LevelEditorLightManager.Instance.GetActiveLightSettings());
                        }
                    });
                }
            }
        }

        private void OnDestroy()
        {
            SkyBoxManager.Instance.RefreshSkyboxAmbientLightAndFog(LevelEditorLightManager.Instance.GetActiveLightSettings());
        }

        public string GetSkybox()
        {
            if (GameModeManager.IsInLevelEditor())
            {
                return Skybox;
            }
            else
            {
                RealisticLightingManager realisticLightingManager = RealisticLightingManager.Instance;
                RealisticLightingInfo realisticLightingInfo = realisticLightingManager.GetCurrentRealisticLightingInfo();
                if (realisticLightingInfo != null)
                {
                    return realisticLightingInfo.SkyboxName;
                }
                return Skybox;
            }
        }

        public List<Dropdown.OptionData> GetDropdownOptions(string fieldName)
        {
            if(fieldName == nameof(Skybox))
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
