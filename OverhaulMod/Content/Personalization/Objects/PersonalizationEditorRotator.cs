using OverhaulMod.UI;
using OverhaulMod.UI.Elements;
using UnityEngine;

namespace OverhaulMod.Content.Personalization.Objects
{
    public class PersonalizationEditorRotator : PersonalizationEditorComponent
    {
        private Vector3 _direction;
        public Vector3 Direction
        {
            get => _direction;
            set
            {
                SetPropertyValue(nameof(PersonalizationEditorRotator), nameof(Direction), value);
                _direction = value;
            }
        }

        public bool PreviewInEditor;

        public override string GetDisplayName() => "Rotate over time";

        private void Start()
        {
            _direction = GetPropertyValue(nameof(PersonalizationEditorRotator), nameof(Direction), Vector3.zero);
        }

        private void Update()
        {
            if (PersonalizationEditorManager.IsInEditorMode() && !PreviewInEditor) return;

            base.transform.rotation *= Quaternion.Euler(_direction * Time.deltaTime);
        }

        public override void InstantiateSettingsForInspector(UIElementPEInspectorGroup group, UIElementPEInspectorFieldsStore fieldsStore)
        {
            ModdedObject rotatorSettingsObject = Instantiate(fieldsStore.RotatorSettings, group.GetContainer());

            UIElementPEInspectorRotatorSettings rotatorSettings = rotatorSettingsObject.gameObject.AddComponent<UIElementPEInspectorRotatorSettings>();
            rotatorSettings.InitializeAsElement();
            rotatorSettings.SetComponent(this);
            rotatorSettings.SetValuesFromComponent();
        }

        public void ResetRotation()
        {
            base.transform.localEulerAngles = PlacedObject.SerializedEulerAngles;
        }
    }
}
