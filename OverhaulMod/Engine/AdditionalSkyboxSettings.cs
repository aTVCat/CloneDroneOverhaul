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

        [IncludeInLevelEditor]
        public Color Tint = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        [IncludeInLevelEditor]
        public float Rotation = 0f;

        private void Awake()
        {
            if (!GameModeManager.IsInLevelEditor()) return;
            base.GetComponent<ObjectPlacedInLevel>().AddValueChangedListener(onValueChanged);
        }

        private void onValueChanged(string fieldName)
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

        public bool HasDropDownForValue(string fieldName) => fieldName == nameof(Skybox);

        public bool ShouldShowDropdownOptions(string fieldName) => fieldName == nameof(Skybox);

        public bool ShouldHideField(string fieldName) => false;
    }
}